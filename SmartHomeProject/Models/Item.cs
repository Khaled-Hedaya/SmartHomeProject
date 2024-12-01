using System;
using System.Collections.Generic;

namespace SmartHomeProject.Models
{
    public class Item : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string MacIp { get; set; }
        public string LastVersionNumber { get; set; }
        public bool IsAssigned { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }
        public Guid? RoomId { get; set; }
        public Room? Room { get; set; }
        public Product Product { get; set; } 
        public ICollection<ItemAction> Actions { get; set; } = new List<ItemAction>();
    }
}