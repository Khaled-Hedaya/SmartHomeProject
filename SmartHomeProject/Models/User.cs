using System;
using System.Collections.Generic;

namespace SmartHomeProject.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string? Image { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}