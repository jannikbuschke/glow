using Microsoft.AspNetCore.Mvc;

namespace JannikB.Glue
{
    internal class SerializableValidationResult
    {
        public bool IsValid { get; set; }
        public SerializableError Errors { get; set; }
    }
}
