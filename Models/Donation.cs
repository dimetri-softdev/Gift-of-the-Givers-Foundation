using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GiftOfTheGivers.Models;
namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        // Primary Key & Legacy Aliases
        public int DonationID { get; set; }

        [NotMapped]
        public int Id { get => DonationID; set => DonationID = value; }

        // User Link (Nullable to allow guest & anonymous donations)
        public int? UserID { get; set; }

        // Donor Details (Optional/Nullable to support Anonymous donations)
        [StringLength(100)]
        public string? DonorName { get; set; }

        [EmailAddress]
        public string? DonorEmail { get; set; }

        // Anonymous Flag
        public bool IsAnonymous { get; set; } = false;

        // Financial & Frequency Properties
        [Column(TypeName = "decimal(18,2)")]
        [Range(1.00, 1000000.00, ErrorMessage = "Please enter a valid donation amount.")]
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

        // Date Properties
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public DateTime Date
        {
            get => DonationDate;
            set => DonationDate = value;
        }

        // Tax Certificate & Relief Project Foreign Keys
        public string? TaxCertificateCode { get; set; }

        public int? ReliefProjectId { get; set; }

        [ForeignKey("ReliefProjectId")]
        public ReliefProject? ReliefProject { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public DonationSchedule? DonationSchedule { get; set; }
        public TaxCertificate? TaxCertificate { get; set; }
    }
}