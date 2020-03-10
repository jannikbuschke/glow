using System;

namespace JannikB.Glue.AspNetCore
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string msg) : base(msg) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string msg) : base(msg) { }
    }
}
