using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GiftOfTheGivers.Models;

public class CreateReliefProjectViewModel
{
    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "1", "1000000000", ErrorMessage = "Enter a target amount greater than zero.")]
    public decimal TargetAmount { get; set; }

    [Required]
    public string Status { get; set; } = "Active";

    [Url]
    [StringLength(500)]
    public string? VideoUrl { get; set; }

    public IFormFile? ImageFile { get; set; }
}
