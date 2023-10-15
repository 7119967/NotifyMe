using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace NotifyMe.Core.Entities
{
    public class User : IdentityUser
    {
        public override string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public override string? Email { get; set; }
        public override string? PhoneNumber { get; set; }
        public string? Info { get; set; }
        public string? Avatar { get; set; }
        public string? GroupId { get; set; }

        [NotMapped] 
        public IFormFile? File { get; set; }
        [JsonIgnore]
        public virtual Group? Group { get; set; }
            
        public virtual ICollection<NotificationUser>? NotificationUsers { get; set; }
    }
}