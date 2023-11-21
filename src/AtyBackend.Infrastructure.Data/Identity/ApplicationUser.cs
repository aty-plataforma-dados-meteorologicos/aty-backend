using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Enums;
using AtyBackend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AtyBackend.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public string? Name { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    public int? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordCodeExpiration { get; set; }
    public UserTypeEnum Type { get; set; }

    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }

    public List<WeatherStationUser>? WeatherStationUsers { get; set; }
}
