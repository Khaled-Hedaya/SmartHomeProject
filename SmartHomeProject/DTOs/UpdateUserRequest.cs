using System.ComponentModel.DataAnnotations;

namespace SmartHomeProject.DTOs
{
    public class UpdateUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? Image { get; set; }
    }
}
