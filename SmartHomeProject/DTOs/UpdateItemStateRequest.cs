using System.ComponentModel.DataAnnotations;

namespace SmartHomeProject.DTOs
{
    public class UpdateItemStateRequest
    {
        [Required]
        public string State { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
