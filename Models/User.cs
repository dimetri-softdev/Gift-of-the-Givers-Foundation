using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Models
{
    public class User : IdentityUser<int>
    {
        // Primary Key Alias for backward compatibility with your EF models & queries
        public int UserID
        {
            get => Id;
            set => Id = value;
        }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Role { get; set; } = "Donor"; // Admin, Employee, Donor, Volunteer

        // Navigation Properties
        // public Employee? Employee { get; set; }
        public Volunteer? Volunteer { get; set; }
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<ReliefProject> ReliefProjects { get; set; } = new List<ReliefProject>();
        // public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}