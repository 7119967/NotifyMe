using System.ComponentModel.DataAnnotations;

namespace NotifyMe.API.ViewModels;

public class EditProfileViewModel
{
    [Display(Name = "Логин")]
    [DataType(DataType.Text)]
    public string? UserName { get; set; }
        
    [Display(Name = "FirstName")]
    [DataType(DataType.Text)]
    public string? FirstName { get; set; }   
    
    [Display(Name = "LastName")]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }
        
    [Display(Name = "Электронная почта")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }    
    
    [Display(Name = "PhoneNumber")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
        
    [Display(Name = "Загрузите изображение для аватара")]
    [DataType(DataType.Text)]
    public IFormFile? File { get; set; }
        
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
        
    [Display(Name = "Аватар")]
    [DataType(DataType.Text)]
    public string? Avatar { get; set; }
             
    [Display(Name = "Информация о себе")]
    [DataType(DataType.Text)]
    public string? Info { get; set; }
}