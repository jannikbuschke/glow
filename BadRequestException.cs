using System;

namespace JannikB.Glue.AspNetCore
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string msg) : base(msg) { }
    }
}
