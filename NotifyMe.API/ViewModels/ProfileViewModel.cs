using System.ComponentModel.DataAnnotations;

namespace NotifyMe.API.ViewModels;

public class ProfileViewModel
{
    public long Id { get; set; }
    public string UserName { get; set; }
    [Display(Name = "FirstName")]
    public string FirstName { get; set; }
    [Display(Name = "LastName")]
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    [Display(Name = "Информация о себе")]
    public string Info { get; set; }
}