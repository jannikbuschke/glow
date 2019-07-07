using System;
using System.Collections.Generic;
using System.Text;

namespace JannikB.Glue.AspNetCore
{
    public class BadRequestException:Exception
    {
        public BadRequestException(string msg) : base(msg) { }
    }
}
