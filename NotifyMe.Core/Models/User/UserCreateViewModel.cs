using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NotifyMe.Core.Models.User;

public class UserCreateViewModel
{
    public string? Id { get; set; }

    [Display(Name = "Login")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    [Display(Name = "FirstName")]
    [DataType(DataType.Text)]
    public string? FirstName { get; set; }

    [Display(Name = "LastName")]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }

    [Display(Name = "Email address")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Display(Name = "Phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Upload an avatar image")]
    [DataType(DataType.Text)]
    public IFormFile? File { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; } 
    
    [Display(Name = "PasswordConfirm")]
    [DataType(DataType.Password)]
    public string? PasswordConfirm { get; set; }

    [Display(Name = "Avatar")]
    [DataType(DataType.Text)]
    public string? Avatar { get; set; }

    [Display(Name = "Information about yourself")]
    [DataType(DataType.Text)]
    public string? Info { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? PasswordHash { get; set; }
    public string? SecurityStamp { get; set; }
    public bool? LockoutEnabled { get; set; }
}