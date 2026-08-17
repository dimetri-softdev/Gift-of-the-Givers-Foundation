using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        public string? DonorName { get; set; }

        public string? DonorEmail { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "ZAR";

        public string Frequency { get; set; } = "One-Time";

        public string? TaxCertificateCode { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.Now;

        // Foreign Key for ReliefProject
        public int? ReliefProjectId { get; set; }

        // Navigation Property
        [ForeignKey("ReliefProjectId")]
        public virtual ReliefProject? ReliefProject { get; set; }
    }
}