using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "The username is specified incorrectly")]
    [Remote("IsExist", "Validation", ErrorMessage = "This login is not registered")]
    [Display(Name = "Login")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }
                        
    [Required(ErrorMessage = "No password specified")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
        
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
        
    public string? ReturnUrl { get; set; }
}