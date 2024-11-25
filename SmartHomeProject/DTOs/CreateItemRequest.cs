using System.ComponentModel.DataAnnotations;

namespace SmartHomeProject.DTOs
{
    public class CreateItemRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string MacIp { get; set; }

        public int LastVersionNumber { get; set; } = 1;
    }
}
