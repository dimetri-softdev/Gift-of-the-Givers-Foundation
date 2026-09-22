using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class ProjectVolunteer
    {
        public int ProjectVolunteerID { get; set; }

        public int ProjectID { get; set; }

        public int VolunteerID { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(30)]
        public string AssignmentStatus { get; set; } = string.Empty;

        [ForeignKey("ProjectID")]
        public ReliefProject? Project { get; set; }

        [ForeignKey("VolunteerID")]
        public Volunteer? Volunteer { get; set; }
    }
}