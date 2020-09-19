using JannikB.Glue.AspNetCore.Tests;

namespace Glow.Sample.Users
{
    public class TestUsers
    {
        public static UserDto TestUser()
        {
            return new UserDto
            {
                Id = "1",
                DisplayName = "Stanley Kubrick",
                Email = "sku@q-and-a.com"
            };
        }
    }
}
