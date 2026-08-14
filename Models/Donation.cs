using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int DonationID { get; set; } // Primary Key
        public int? UserID { get; set; } // Nullable for anonymous guest donations
        [ForeignKey("UserID")] 
        public User? User { get; set; } // Navigation property to the User model
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Amount donated
        [Required, StringLength(3)]
        public string Currency { get; set; } = "ZAR"; // Currency code (e.g., ZAR, USD, EUR)
        [Required]
        public string Frequency { get; set; } = "One-Time"; // One-Time, Monthly Recurring
        public DateTime DonationDate { get; set; } = DateTime.Now; // Date of the donation
        public Guid TaxCertificateCode { get; set; } = Guid.NewGuid(); // Unique code for tax certificate generation
    }
}