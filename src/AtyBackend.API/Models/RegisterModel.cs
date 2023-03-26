using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max " +
        "{1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    //[DataType(DataType.Password)]
    //[Display(Name = "Confirm password")]
    //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string? Role { get; set; }
}
