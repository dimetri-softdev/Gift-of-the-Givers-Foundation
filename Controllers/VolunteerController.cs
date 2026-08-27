using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerController(ApplicationDbContext context)
        {
            _context = context;
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

                // Create Audit Log entry
                var auditLog = new AuditLog
                {
                    Action = "Volunteer Registered",
                    EntityName = nameof(Volunteer),
                    EntityID = model.VolunteerID.ToString(),
                    CreatedDate = DateTime.UtcNow
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

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