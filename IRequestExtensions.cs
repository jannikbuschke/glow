using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace JannikB.AspNetCore.Utils.Module
{
    public static class IRequestExtensions
    {
        public static List<ValidationResult> Validate(this IRequest obj)
        {
            ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(obj, serviceProvider: null, items: null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, context, validationResults, true);
            return validationResults;
        }

        public static List<ValidationResult> Validate<T>(this IRequest<T> obj)
        {
            ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(obj, serviceProvider: null, items: null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, context, validationResults, true);
            return validationResults;
        }
    }
}
