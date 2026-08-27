using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        // Primary Key & Legacy Aliases
        public int DonationID { get; set; }
        [NotMapped]
        public int Id { get => DonationID; set => DonationID = value; }

        // User Link required by ApplicationDbContext & LINQ queries
        public int? UserID { get; set; }

        // Donor Details
        [Required(ErrorMessage = "Donor name is required.")]
        [StringLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Donor email is required.")]
        [EmailAddress]
        public string DonorEmail { get; set; } = string.Empty;

        // Financial & Frequency Properties
        [Column(TypeName = "decimal(18,2)")]
        [Range(1.00, 1000000.00, ErrorMessage = "Please enter a valid amount.")]
        public decimal Amount { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "ZAR";

        [Required, StringLength(20)]
        public string Frequency { get; set; } = "One-Time";

        [NotMapped]
        public string DonationType
        {
            get => Frequency;
            set => Frequency = value;
        }

        public bool IsAnonymous { get; set; } = false;

        // Date Properties
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public DateTime Date
        {
            get => DonationDate;
            set => DonationDate = value;
        }

        // Tax Certificate & Foreign Keys
        public string? TaxCertificateCode { get; set; }

        public int? ReliefProjectId { get; set; }

        [ForeignKey("ReliefProjectId")]
        public ReliefProject? ReliefProject { get; set; }

        public User? User { get; set; }
        public DonationSchedule? DonationSchedule { get; set; }
        public TaxCertificate? TaxCertificate { get; set; }
    }
}