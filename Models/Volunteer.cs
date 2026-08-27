using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Volunteer
    {
        public int VolunteerID { get; set; }

        public int? UserID { get; set; } // Optional: link to User account if logged in

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required.")]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Skill category is required.")]
        [StringLength(100)]
        public string SkillCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Availability is required.")]
        [StringLength(100)]
        public string Availability { get; set; } = string.Empty;

        // Optional direct campaign assignment
        public int? AssignedProjectId { get; set; }

        [ForeignKey("AssignedProjectId")]
        public ReliefProject? AssignedProject { get; set; }

        // Navigation Properties for Many-to-Many junction table
        public User? User { get; set; }
        public ICollection<ProjectVolunteer> ProjectVolunteers { get; set; } = new List<ProjectVolunteer>();
    }
}