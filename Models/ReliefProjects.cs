using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
	public class ReliefProject
	{
		[Key]
		public int ProjectID { get; set; } // Primary Key
		[Required, StringLength(150)]
		public string Title { get; set; } = string.Empty; // Project title
		[Required, StringLength(100)]
		public string Location { get; set; } = string.Empty; // Location of the relief project
		[Required]
		public string Status { get; set; } = "Active"; // Active, Completed, Pending
		public DateTime CreatedDate { get; set; } = DateTime.Now; // Date the project was created
		[Column(TypeName = "decimal(18,2)")]
		public decimal TargetAmount { get; set; } // Target amount to be raised for the project
		[Column(TypeName = "decimal(18,2)")]
		public decimal RaisedAmount { get; set; } // Amount raised so far for the project
		public int? CreatedByUserID { get; set; } // Nullable foreign key to the User who created the project
		[ForeignKey("CreatedByUserID")]
		public User? CreatedByUser { get; set; } // Navigation property to the User model
	}
}