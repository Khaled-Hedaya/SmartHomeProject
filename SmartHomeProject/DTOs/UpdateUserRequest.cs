using System.ComponentModel.DataAnnotations;

namespace SmartHomeProject.DTOs
{
    public class UpdateUserRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }


        public string Image { get; set; }
    }
}
