using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; }
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; }
        [Required, StringLength(250)]
        public string Skills { get; set; } = string.Empty; // Medical, Logistics, Driving, etc.
        [Required, StringLength(100)]
        public string Availability { get; set; } = string.Empty; // Weekdays, Weekends, Emergency Callout
        public int? AssignedProjectID { get; set; }
        [ForeignKey("AssignedProjectID")]
        public ReliefProject? AssignedProject { get; set; }
    }
}