using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models;

public class RequestResetPassword
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid format email")]
    public string Email { get; set; }
}