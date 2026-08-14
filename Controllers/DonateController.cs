using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class DonateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Donate
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Projects = _context.ReliefProjects.Where(p => p.Status == "Active").ToList();
            return View();
        }

        // POST: /Donate/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(Donation donation, int? selectedProjectId)
        {
            if (ModelState.IsValid)
            {
                donation.DonationDate = DateTime.Now;
                _context.Donations.Add(donation);

                // Update RaisedAmount on ReliefProject if selected
                if (selectedProjectId.HasValue)
                {
                    var project = await _context.ReliefProjects.FindAsync(selectedProjectId.Value);
                    if (project != null)
                    {
                        project.RaisedAmount += donation.Amount;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thank you for your generous donation!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Projects = _context.ReliefProjects.Where(p => p.Status == "Active").ToList();
            return View("Index", donation);
        }
    }
}