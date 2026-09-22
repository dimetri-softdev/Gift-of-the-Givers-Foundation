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
                    EntityID = model.VolunteerID, // FIX: assign int, not string
                    CreatedDate = DateTime.UtcNow
                };

                // FIX: Use Set<AuditLog>() since AuditLogs DbSet is missing
                _context.Set<AuditLog>().Add(auditLog);
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

    public class AuditLog
    {
        public int AuditLogID { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int EntityID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Details { get; set; }
        public string? Action { get; set; }
    }
}