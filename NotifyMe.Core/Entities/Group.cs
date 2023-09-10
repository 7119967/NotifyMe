using System.Text.Json.Serialization;

namespace NotifyMe.Core.Entities
{
    public class Group : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ConfigurationId { get; set; }
        [JsonIgnore]
        public virtual Configuration? Configuration { get; set; }
        public virtual List<User> Users { get; set; } = new();
    }
}