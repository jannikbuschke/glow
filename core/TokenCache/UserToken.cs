using System;

namespace Glow.TokenCache
{
    public class UserToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}