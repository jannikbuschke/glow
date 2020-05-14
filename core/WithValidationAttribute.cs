using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;


namespace JannikB.Glue
{
    public class WithValidationAttribute : ActionFilterAttribute
    {
        private const string ParameterName = "x-action-intent";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues header = context.HttpContext.Request.Headers[ParameterName];
            var intent = header.FirstOrDefault();
            if (intent == null)
            {
                context.Result = new BadRequestObjectResult($"Missing header '{ParameterName}'");
                return;
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            switch (intent)
            {
                case "validate": context.Result = new OkResult(); break;
                case "execute": break;
                default: context.Result = new BadRequestObjectResult($"Unknown _action parameter value: '{ParameterName}'"); break;
            }

        }
    }
}
