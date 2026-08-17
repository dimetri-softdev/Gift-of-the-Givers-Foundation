using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for financial amounts
            modelBuilder.Entity<ReliefProject>()
                .Property(p => p.TargetAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ReliefProject>()
                .Property(p => p.RaisedAmount)
                .HasColumnType("decimal(18,2)");

            // Set null delete behavior on volunteer project assignment
            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.AssignedProject)
                .WithMany()
                .HasForeignKey(v => v.AssignedProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:appr6312-pro.database.windows.net,1433;Initial Catalog=APPR6312_POE_DB;Persist Security Info=False;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }
    }
}