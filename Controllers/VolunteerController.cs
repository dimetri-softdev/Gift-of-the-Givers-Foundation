using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VolunteerController> _logger;

        public VolunteerController(ApplicationDbContext context, ILogger<VolunteerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Volunteer (or /Volunteer/Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await PopulateProjectsViewBagAsync();
            return View();
        }

        // GET: /Volunteer/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await PopulateProjectsViewBagAsync();
            return View("Index");
        }

        // POST: /Volunteer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Volunteer model)
        {
            if (ModelState.IsValid)
            {
                _context.Volunteers.Add(model);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Volunteer application {VolunteerId} was registered.", model.VolunteerID);

                TempData["SuccessMessage"] = "Thank you for registering! Your application has been submitted successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown if validation fails
            await PopulateProjectsViewBagAsync();
            return View("Index", model);
        }

        private async Task PopulateProjectsViewBagAsync()
        {
            ViewBag.Projects = await _context.ReliefProjects
                .Where(p => p.Status == "Active")
                .ToListAsync();
        }
    }
}