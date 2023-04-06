using AtyBackend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models
{
    public class ApplicationUserDTO
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid format email.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Role is required.")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public UserTypeEnum Type { get; set; }

        [Required(ErrorMessage = "UserStatus is required.")]
        public bool IsEnabled { get; set; }
    }
}
