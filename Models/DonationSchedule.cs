using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class DonationSchedule
    {
        // Primary key for schedule lookup
        public int DonationScheduleID { get; set; }

        public int DonationId { get; set; }

        [Required]
        // Short friendly name, e.g. "One-Time", "Monthly"
        [StringLength(50)]
        public string Frequency { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime NextPaymentDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("DonationId")]
        public Donation Donation { get; set; } = null!;
    }
}