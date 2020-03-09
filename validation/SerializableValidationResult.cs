using Microsoft.AspNetCore.Mvc;

namespace JannikB.Glue
{
    public class SerializableValidationResult
    {
        public bool IsValid { get; set; }
        public SerializableError Errors { get; set; }
    }
}
