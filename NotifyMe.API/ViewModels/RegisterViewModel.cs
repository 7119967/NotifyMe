using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "The username is specified incorrectly")]
    [Remote("CheckUserName", "Validation", ErrorMessage = "This login has already been used")]
    [Display(Name = "Login")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "The email address is specified incorrectly")]
    [Remote("CheckEmailAddress", "Validation", ErrorMessage = "This email has already been used")]
    [Display(Name = "Email address")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "The phone number is specified incorrectly")]
    [Remote("CheckPhoneNumber", "Validation", ErrorMessage = "This phone number has already been used")]
    [Display(Name = "Phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "The field is not filled in")]
    [Display(Name = "Upload an avatar image")]
    [DataType(DataType.Text)]
    public IFormFile? File { get; set; }

    [Required(ErrorMessage = "No password specified")]
    [StringLength(16, MinimumLength = 4, ErrorMessage = "The password must be at least 4 characters long")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "No password specified")]
    [StringLength(16, MinimumLength = 4, ErrorMessage = "The password must be at least 4 characters long")]
    [Compare("Password", ErrorMessage = "The password doesn't match")]
    [Display(Name = "Confirm your password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}