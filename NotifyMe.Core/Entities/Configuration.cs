using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NotifyMe.Core.Enums;

namespace NotifyMe.Core.Entities
{
    public class Configuration : BaseEntity
    {
        public EventType EventType { get; set; }
        public double Threshold { get; set; }
        public List<User>? Recipients { get; set; }
    }
}
