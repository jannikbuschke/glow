using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Glow.TypeScript;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Sample.Users
{
    [GenerateTsInterface]
    public class User
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }

    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static readonly IEnumerable<User> users = new Faker<User>()
            .RuleFor(v => v.Id, f => Guid.NewGuid().ToString())
            .RuleFor(v => v.DisplayName, f => f.Person.FullName)
            .RuleFor(v => v.Email, f => f.Person.Email)
            .Generate(20);

        public UserController()
        {

        }

        [HttpGet]
        public Task<IEnumerable<User>> Index(string search)
        {
            return Task.FromResult(users.Where(v => string.IsNullOrEmpty(search) || v.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
