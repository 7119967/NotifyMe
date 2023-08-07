using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Не верно указан логин")]
    [Remote("IsExist", "Validation", ErrorMessage = "Такой логин не зарегистрирован")]
    [Display(Name = "Логин")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }
                        
    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }
        
    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; }
        
    public string? ReturnUrl { get; set; }
}