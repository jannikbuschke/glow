using Microsoft.AspNetCore.Mvc;

namespace JannikB.Glue
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public SerializableError Errors { get; set; }
    }
}
