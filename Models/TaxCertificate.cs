using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class TaxCertificate
    {
        public int CertificateID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CertifiedAmount { get; set; }

        public int DonationID { get; set; }

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(100)]
        public string TaxCertificateNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string TaxNumber { get; set; } = string.Empty;

        [ForeignKey("DonationID")]
        public Donation Donation { get; set; } = null!;
    }
}