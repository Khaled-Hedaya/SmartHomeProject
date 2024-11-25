using System.ComponentModel.DataAnnotations;

namespace SmartHomeProject.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Image { get; set; }
    }
}
