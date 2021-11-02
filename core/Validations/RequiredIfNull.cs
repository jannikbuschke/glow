using System;
using System.ComponentModel.DataAnnotations;

namespace Glow.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfNull : RequiredAttribute
    {
        private string[] PropertyName { get; set; }

        public RequiredIfNull(params string[] properties)
        {
            PropertyName = properties;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            Type type = instance.GetType();

            var valueString = value?.ToString();
            foreach (var v in PropertyName)
            {
                if (type.GetProperty(v).GetValue(instance) == null && value == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}