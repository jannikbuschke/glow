using System.Threading.Tasks;
using Glow.Core.Actions;
using Glow.Users;
using Microsoft.EntityFrameworkCore;

namespace Gertrud.Users
{
    public class User : IUser
    {
        public const string SystemUserId = "___system___";
        public const string NoTenantId = "___no-tenant___";

        public string Id { get; set; }
        public string TenantId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserDto ToDto()
        {
            return new UserDto
            {
                Id = Id,
                DisplayName = DisplayName,
                Email = Email

            };
        }
    }

    public static class GraphUserExtension
    {
        public static User ToUser(this Microsoft.Graph.User user)
        {
            return new User
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Mail
            };
        }
    }
}