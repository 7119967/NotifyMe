using System.ComponentModel.DataAnnotations;

namespace NotifyMe.API.ViewModels;

public class PostViewModel
{
    [Required(ErrorMessage = "Поле должно быть заполнено")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Длина текста должна быть от 2 до 1000 символов")]
    public string? Description { get; set; }
    public string? UserId { get; set; }
    public IFormFile? File { get; set; }
    public DateTime? TimeStamp { get; set; }
    public int LikesCounter { get; set; }
    public int CommentsCounter { get; set; }
}