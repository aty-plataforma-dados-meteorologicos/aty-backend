﻿using AtyBackend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtyBackend.API.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max " +
        "{1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Type ir required")]
    public UserTypeEnum Type { get; set; }

    //[DataType(DataType.Password)]
    //[Display(Name = "Confirm password")]
    //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string? Role { get; set; }
}
