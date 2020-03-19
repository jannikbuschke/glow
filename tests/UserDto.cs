using System.ComponentModel.DataAnnotations;

namespace JannikB.Glue.AspNetCore.Tests
{
    public class UserDto
    {
        // TODO, implement "Dual Mode" for register / upser meeting item
        public string Id { get; set; }
        // TODO properly implement Validation
        //[Required]
        public string DisplayName { get; set; }
        //[Required, EmailAddress]
        public string Email { get; set; }
    }
}
