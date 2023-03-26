using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Enums;
using AtyBackend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AtyBackend.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public string Nome { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    public UserTypeEnum UserType { get; set; }

    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }

    public List<WeatherStationUser>? WeatherStationUser { get; set; }
}
