using System.ComponentModel.DataAnnotations;

namespace NotifyMe.Core.Models;

public class ProfileViewModel
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    [Display(Name = "FirstName")]
    public string? FirstName { get; set; }
    [Display(Name = "LastName")]
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    [Display(Name = "Information about yourself")]
    public string? Info { get; set; }
}