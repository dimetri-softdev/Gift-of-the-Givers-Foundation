using GiftOfTheGivers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            // Calculate KPI summary values
            ViewBag.ActiveProjectsCount = await _context.ReliefProjects.CountAsync(p => p.Status == "Active");
            ViewBag.TotalVolunteersCount = await _context.Volunteers.CountAsync();

            // Sum donations in the last 30 days using DonationDate
            ViewBag.MonthlyDonationsTotal = await _context.Donations
                .Where(d => d.DonationDate >= DateTime.Now.AddDays(-30))
                .SumAsync(d => (decimal?)d.Amount) ?? 0;

            // Fetch volunteers with assigned project
            var volunteers = await _context.Volunteers
                .Include(v => v.AssignedProject)
                .ToListAsync();

            ViewBag.Projects = await _context.ReliefProjects.ToListAsync();

            return View(volunteers);
        }
    }
}