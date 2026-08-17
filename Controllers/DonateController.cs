using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            // Fetch active projects to display in the donation form
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

                // Auto-generate Section 18A Tax Certificate Reference Code
                if (string.IsNullOrEmpty(donation.TaxCertificateCode))
                {
                    donation.TaxCertificateCode = $"TAX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                }

                _context.Donations.Add(donation);

                // Update RaisedAmount on ReliefProject if selected
                if (selectedProjectId.HasValue)
                {
                    var project = await _context.ReliefProjects.FindAsync(selectedProjectId.Value);
                    if (project != null)
                    {
                        project.RaisedAmount += donation.Amount;
                        donation.ReliefProjectId = selectedProjectId.Value;
                    }
                }

                await _context.SaveChangesAsync();

                // Redirect to Confirmation screen with the new Donation ID
                return RedirectToAction(nameof(Confirmation), new { id = donation.Id });
            }

            ViewBag.Projects = _context.ReliefProjects.Where(p => p.Status == "Active").ToList();
            return View("Index", donation);
        }

        // GET: /Donate/Confirmation/{id}
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.ReliefProject)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }
    }
}