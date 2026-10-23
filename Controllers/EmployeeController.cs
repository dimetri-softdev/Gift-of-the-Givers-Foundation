using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers;

[Authorize(Roles = "Employee,Admin")]
public class EmployeeController : Controller
{
    private const long MaxImageSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.Ordinal)
    {
        "Active", "Planned", "Completed", "Pending"
    };

    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(
        ApplicationDbContext context,
        UserManager<User> userManager,
        IWebHostEnvironment environment,
        ILogger<EmployeeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalDonations = await _context.Donations
            .Where(d => d.Currency == "ZAR")
            .SumAsync(d => (decimal?)d.Amount) ?? 0m;
        ViewBag.ActiveProjects = await _context.ReliefProjects.CountAsync(p => p.Status == "Active");
        ViewBag.VolunteerCount = await _context.Volunteers.CountAsync();

        var volunteers = await _context.Volunteers
            .Include(v => v.AssignedProject)
            .OrderByDescending(v => v.VolunteerID)
            .ToListAsync();

        return View(volunteers);
    }

    [HttpGet]
    public IActionResult CreateProject()
    {
        return View(new CreateReliefProjectViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxImageSize + 64 * 1024)]
    public async Task<IActionResult> CreateProject(CreateReliefProjectViewModel model)
    {
        if (!AllowedStatuses.Contains(model.Status))
        {
            ModelState.AddModelError(nameof(model.Status), "Select a valid project status.");
        }

        if (!string.IsNullOrWhiteSpace(model.VideoUrl)
            && (!Uri.TryCreate(model.VideoUrl, UriKind.Absolute, out var videoUri)
                || videoUri.Scheme != Uri.UriSchemeHttps))
        {
            ModelState.AddModelError(nameof(model.VideoUrl), "Enter a valid HTTPS video link.");
        }

        var imageExtension = model.ImageFile is null ? null : Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
        if (model.ImageFile is not null)
        {
            if (model.ImageFile.Length == 0 || model.ImageFile.Length > MaxImageSize)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Choose an image smaller than 5 MB.");
            }
            else if (imageExtension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Choose a JPG, PNG, or WebP image.");
            }
            else if (!await HasValidImageSignatureAsync(model.ImageFile, imageExtension))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "The selected file is not a valid image.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? imagePath = null;
        if (model.ImageFile is not null && imageExtension is not null)
        {
            var imageDirectory = Path.Combine(_environment.WebRootPath, "images", "projects");
            var imageName = $"{Guid.NewGuid():N}{imageExtension}";
            var imageFilePath = Path.Combine(imageDirectory, imageName);

            try
            {
                Directory.CreateDirectory(imageDirectory);
                await using var stream = new FileStream(imageFilePath, FileMode.CreateNew, FileAccess.Write);
                await model.ImageFile.CopyToAsync(stream);
                imagePath = $"/images/projects/{imageName}";
            }
            catch (IOException exception)
            {
                _logger.LogError(exception, "Could not save the uploaded project image.");
                ModelState.AddModelError(nameof(model.ImageFile), "The project image could not be saved. Please try again.");
                return View(model);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, "The application cannot write project images to its web root.");
                ModelState.AddModelError(nameof(model.ImageFile), "The project image could not be saved. Please contact support.");
                return View(model);
            }
        }

        var user = await _userManager.GetUserAsync(User);
        var project = new ReliefProject
        {
            Title = model.Title.Trim(),
            Location = model.Location.Trim(),
            TargetAmount = model.TargetAmount,
            Status = model.Status,
            VideoUrl = string.IsNullOrWhiteSpace(model.VideoUrl) ? null : model.VideoUrl.Trim(),
            ImageUrl = imagePath,
            CreatedByUserID = user?.Id,
            CreatedDate = DateTime.UtcNow
        };

        _context.ReliefProjects.Add(project);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "The relief project was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    private static async Task<bool> HasValidImageSignatureAsync(IFormFile image, string extension)
    {
        var header = new byte[12];
        await using var stream = image.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header.AsMemory());

        return extension switch
        {
            ".jpg" or ".jpeg" => bytesRead >= 3
                && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => bytesRead >= 8
                && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" => bytesRead >= 12
                && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
                && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };
    }
}
