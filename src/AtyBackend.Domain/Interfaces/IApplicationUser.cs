using AtyBackend.Domain.Enums;

namespace AtyBackend.Domain.Interfaces
{
    public interface IApplicationUser
    {
        string Name { get; set; }
        string? RefreshToken { get; set; }
        DateTime? RefreshTokenExpiration { get; set; }
        UserTypeEnum Type { get; set; }

        bool IsEnabled { get; set; }
        bool IsDeleted { get; set; }
    }
}
