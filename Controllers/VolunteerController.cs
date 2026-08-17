using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Volunteer/Register (or /Volunteer)
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Projects = await _context.ReliefProjects
                .Where(p => p.Status == "Active")
                .ToListAsync();

            return View(); // Renders Views/Volunteer/Register.cshtml
        }

        // POST: /Volunteer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FullName,Region,SkillCategory,Availability,AssignedProjectId")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Volunteers.Add(volunteer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for registering as a volunteer!";
                return RedirectToAction("Index", "Project");
            }

            // If validation fails, re-populate dropdown and re-render the Register view
            ViewBag.Projects = await _context.ReliefProjects
                .Where(p => p.Status == "Active")
                .ToListAsync();

            return View(volunteer); // Renders Views/Volunteer/Register.cshtml with model errors
        }
    }
}