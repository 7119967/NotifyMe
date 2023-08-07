using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Не верно указан логин")]
    [Remote("CheckUserName", "Validation", ErrorMessage = "Такой логин уже использован")]
    [Display(Name = "Логин")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }
        
    [Required(ErrorMessage = "Не верно указан email")]
    [Remote("CheckEmailAddress", "Validation", ErrorMessage = "Такой email уже использован")]
    [Display(Name = "Электронная почта")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }    
    
    [Required(ErrorMessage = "Не верно указан PhoneNumber")]
    [Remote("CheckPhoneNumber", "Validation", ErrorMessage = "Такой PhoneNumber уже использован")]
    [Display(Name = "PhoneNumber")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
        
    [Required(ErrorMessage = "Поле не заполнено")]
    [Display(Name = "Загрузите изображение для аватара")]
    [DataType(DataType.Text)]
    public IFormFile? File { get; set; }
        
    [Required(ErrorMessage = "Не указан пароль")]
    [StringLength(16, MinimumLength=5, ErrorMessage = "Длина пароля должна быть минимум 5 символов")]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
        
    [Required(ErrorMessage = "Не указан пароль")]
    [StringLength(16, MinimumLength=5, ErrorMessage = "Длина пароля должна быть минимум 5 символов")]
    [Compare("Password", ErrorMessage = "Пароль не совпадает")]
    [Display(Name = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}