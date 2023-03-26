using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models;

public class TokenRefreshModel
{
    [Required(ErrorMessage = "Access Token is requiered")]
    public string? AccessToken { get; set; }

    [Required(ErrorMessage = "Refresh Token is required")]
    public string? RefreshToken { get; set; }
}
