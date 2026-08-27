using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Controllers
{
    public class DonateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DonateController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Donate/Index or /Donate?projectId=1
        [HttpGet]
        public async Task<IActionResult> Index(int? projectId)
        {
            var donation = new Donation
            {
                ReliefProjectId = projectId,
                Currency = "ZAR",
                Frequency = "One-Time"
            };

            // Auto-fill donor details if signed in
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    donation.UserID = user.Id;
                    donation.DonorName = user.FullName;
                    donation.DonorEmail = user.Email ?? string.Empty;
                }
            }

            // Populate active relief projects for the dropdown
            ViewBag.Projects = await _context.ReliefProjects.ToListAsync();

            return View(donation);
        }

        // POST: /Donate/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(Donation donation, int? selectedProjectId)
        {
            // Map the selected project ID from the dropdown if supplied
            if (selectedProjectId.HasValue && selectedProjectId.Value > 0)
            {
                donation.ReliefProjectId = selectedProjectId.Value;
            }

            // 1. Auto-assign logged-in user details if missing
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    donation.UserID = user.Id;
                    if (string.IsNullOrWhiteSpace(donation.DonorName)) donation.DonorName = user.FullName;
                    if (string.IsNullOrWhiteSpace(donation.DonorEmail)) donation.DonorEmail = user.Email ?? string.Empty;

                    ModelState.Remove(nameof(Donation.DonorName));
                    ModelState.Remove(nameof(Donation.DonorEmail));
                }
            }

            // 2. Remove EF Core navigation properties from model validation state
            ModelState.Remove(nameof(Donation.ReliefProject));
            ModelState.Remove(nameof(Donation.User));
            ModelState.Remove(nameof(Donation.DonationSchedule));
            ModelState.Remove(nameof(Donation.TaxCertificate));

            if (ModelState.IsValid)
            {
                donation.DonationDate = DateTime.UtcNow;

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your donation was processed successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload project dropdown list if returning view due to validation errors
            ViewBag.Projects = await _context.ReliefProjects.ToListAsync();
            return View("Index", donation);
        }
    }
}