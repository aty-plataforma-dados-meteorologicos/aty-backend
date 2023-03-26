using AtyBackend.Domain.Enums;

namespace AtyBackend.Domain.Interfaces
{
    public interface IApplicationUser
    {
        string Nome { get; set; }
        string? RefreshToken { get; set; }
        DateTime? RefreshTokenExpiration { get; set; }
        UserTypeEnum UserType { get; set; }

        bool IsEnabled { get; set; }
        bool IsDeleted { get; set; }
    }
}
