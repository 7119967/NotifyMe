using System.ComponentModel.DataAnnotations;

namespace NotifyMe.API.ViewModels;

public class PostViewModel
{
    [Required(ErrorMessage = "The field must be filled in")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "The length of the text should be from 2 to 1000 characters")]
    public string? Description { get; set; }
    public string? UserId { get; set; }
    public IFormFile? File { get; set; }
    public DateTime? TimeStamp { get; set; }
    public int LikesCounter { get; set; }
    public int CommentsCounter { get; set; }
}