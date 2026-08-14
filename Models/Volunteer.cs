using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SkillCategory { get; set; } = string.Empty; // e.g. Medical, Driving, Logistics

        [Required]
        [StringLength(50)]
        public string Availability { get; set; } = string.Empty; // e.g. Weekdays, Weekends

        public int? AssignedProjectId { get; set; }

        [ForeignKey("AssignedProjectId")]
        public ReliefProject? AssignedProject { get; set; }
    }
}