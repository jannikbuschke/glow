using System.ComponentModel.DataAnnotations;

namespace JannikB.Glue.AspNetCore.Tests
{
    public class UserDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
