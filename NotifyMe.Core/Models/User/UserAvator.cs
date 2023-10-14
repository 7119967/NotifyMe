using Microsoft.AspNetCore.Http;

namespace NotifyMe.Core.Models.User;

public class UserAvator
{
    public string? Email { get; set; }
    public IFormFile? File { get; set; }
}