using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models {
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [StringLength(20)]
        public string? PhoneNumber { get; set; } = "Donor"; // Admin, Employee, Donor, Volunteer
    }
}