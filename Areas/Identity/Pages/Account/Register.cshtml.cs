using System.ComponentModel.DataAnnotations;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GiftOfTheGivers.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new User
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = $"{Input.FirstName.Trim()} {Input.LastName.Trim()}",
            Role = "Donor"
        };

        var createResult = await _userManager.CreateAsync(user, Input.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Donor");
        if (!roleResult.Succeeded)
        {
            AddIdentityErrors(roleResult);
            return Page();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        var safeReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)
            ? ReturnUrl
            : Url.Content("~/");

        return LocalRedirect(safeReturnUrl);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    public sealed class InputModel
    {
        [Required, StringLength(50), Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50), Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
