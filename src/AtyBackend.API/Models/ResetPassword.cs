using AtyBackend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models;

public class ResetPassword
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max " +
        "{1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Code is required")]
    public int Code { get; set; }
}
