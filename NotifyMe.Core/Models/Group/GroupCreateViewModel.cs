using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Models.Group
{
    public class GroupCreateViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public PriorityType PriorityType { get; set; }
    }
}
