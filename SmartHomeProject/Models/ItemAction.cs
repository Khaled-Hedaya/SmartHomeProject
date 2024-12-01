using System;

namespace SmartHomeProject.Models
{
    public class ItemAction : BaseEntity
    {
        public Guid ItemId { get; set; }
        public string State { get; set; }
        public string Value { get; set; }

        public Item Item { get; set; }
    }
}