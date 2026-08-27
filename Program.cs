using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure Database Context with Resilience
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Identity with Custom User model and int primary key
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed Database (Migrations, Roles, Employees, Projects)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // 1. Ensure database migrations are applied
        await context.Database.MigrateAsync();

        // 2. Seed Identity Roles and Admin User
        await DbInitializer.SeedRolesAndAdminAsync(services);

        // 3. Seed Initial Relief Projects
        await DbInitializer.SeedReliefProjectsAsync(context);

        // 4. Ensure Employee Role & Employee User exist
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (!await roleManager.RoleExistsAsync("Employee"))
        {
            await roleManager.CreateAsync(new IdentityRole<int>("Employee"));
        }

        var testEmail = "employee@giftofthegivers.org";
        var user = await userManager.FindByEmailAsync(testEmail);

        if (user == null)
        {
            user = new User
            {
                UserName = testEmail,
                Email = testEmail,
                EmailConfirmed = true,
                FullName = "Default Employee",
                Role = "Employee"
            };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "Employee");
        }
        else
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, "Password123!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding Identity roles, users, or relief projects.");
    }
}

await app.RunAsync();