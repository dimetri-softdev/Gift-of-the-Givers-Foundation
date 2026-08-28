using System;
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

        // GET: /Donate or /Donate?projectId=1
        [HttpGet]
        public async Task<IActionResult> Index(int? projectId)
        {
            var donation = new Donation
            {
                ReliefProjectId = projectId,
                Currency = "ZAR",
                Frequency = "One-Time",
                IsAnonymous = false
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

            ViewBag.Projects = await _context.ReliefProjects.ToListAsync();
            return View(donation);
        }

        // POST: /Donate/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(
            [Bind("DonationID,DonorName,DonorEmail,IsAnonymous,Amount,Currency,Frequency,ReliefProjectId")] Donation donation,
            [FromForm(Name = "RequiresTaxCertificate")] bool requiresTaxCertificate,
            [FromForm(Name = "TaxNumber")] string? taxNumber)
        {
            // 1. Handle Logged-In User Association & Anonymity Logic
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    donation.UserID = user.Id;
                }
            }

            if (donation.IsAnonymous)
            {
                // Clear identifying donor information for anonymous contributions
                donation.DonorName = "Anonymous";
                donation.DonorEmail = null;

                ModelState.Remove(nameof(Donation.DonorName));
                ModelState.Remove(nameof(Donation.DonorEmail));
            }
            else if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (string.IsNullOrWhiteSpace(donation.DonorName)) donation.DonorName = user.FullName;
                    if (string.IsNullOrWhiteSpace(donation.DonorEmail)) donation.DonorEmail = user.Email ?? string.Empty;

                    ModelState.Remove(nameof(Donation.DonorName));
                    ModelState.Remove(nameof(Donation.DonorEmail));
                }
            }

            // 2. Custom validation for tax certificate field
            if (requiresTaxCertificate && string.IsNullOrWhiteSpace(taxNumber))
            {
                ModelState.AddModelError("TaxNumber", "Tax reference number or RSA ID is required for a Section 18A Tax Certificate.");
            }

            // 3. Remove EF Core navigation properties from model validation state
            ModelState.Remove(nameof(Donation.ReliefProject));
            ModelState.Remove(nameof(Donation.User));
            ModelState.Remove(nameof(Donation.DonationSchedule));
            ModelState.Remove(nameof(Donation.TaxCertificate));

            if (ModelState.IsValid)
            {
                donation.DonationDate = DateTime.UtcNow;

                // 🟢 FIX: Fetch and update the allocated ReliefProject's RaisedAmount
                if (donation.ReliefProjectId.HasValue)
                {
                    var project = await _context.ReliefProjects
                        .FirstOrDefaultAsync(p => p.ProjectID == donation.ReliefProjectId.Value);

                    if (project != null)
                    {
                        project.RaisedAmount += donation.Amount;
                        _context.ReliefProjects.Update(project);
                    }
                }

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                // 4. Generate & save Tax Certificate if opted in
                if (requiresTaxCertificate && !string.IsNullOrWhiteSpace(taxNumber))
                {
                    var certificateCode = $"18A-{DateTime.UtcNow:yyyyMMdd}-{donation.DonationID:D5}";

                    var certificate = new TaxCertificate
                    {
                        DonationID = donation.DonationID,
                        TaxNumber = taxNumber,
                        CertifiedAmount = donation.Amount,
                        TaxCertificateNumber = certificateCode,
                        IssuedDate = DateTime.UtcNow
                    };

                    _context.TaxCertificates.Add(certificate);
                    donation.TaxCertificateCode = certificateCode;
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Thank you for your generous contribution! Your donation has been recorded.";
                return RedirectToAction(nameof(Confirmation), new { id = donation.DonationID });
            }

            ViewBag.Projects = await _context.ReliefProjects.ToListAsync();
            return View("Index", donation);
        }

        // GET: /Donate/Confirmation/5
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.ReliefProject)
                .FirstOrDefaultAsync(d => d.DonationID == id);

            if (donation == null)
            {
                return NotFound();
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && donation.UserID != null && donation.UserID != user.Id)
                {
                    return Forbid();
                }
            }

            var taxCert = await _context.TaxCertificates
                .FirstOrDefaultAsync(tc => tc.DonationID == id);

            if (taxCert != null)
            {
                donation.TaxCertificateCode = taxCert.TaxCertificateNumber;
            }

            return View(donation);
        }
    }
}