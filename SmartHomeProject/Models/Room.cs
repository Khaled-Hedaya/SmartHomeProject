using System;
using System.Collections.Generic;

namespace SmartHomeProject.Models
{
    public class Room : BaseEntity
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
}