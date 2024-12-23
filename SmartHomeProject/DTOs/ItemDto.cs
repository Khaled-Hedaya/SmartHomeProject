using System;
using System.Collections.Generic;

namespace SmartHomeProject.DTOs
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string MacIp { get; set; }
        public string LastVersionNumber { get; set; }
        public bool IsAssigned { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}