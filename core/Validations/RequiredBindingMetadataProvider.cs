using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Glow.Validation
{
    public class RequiredBindingMetadataProvider : IBindingMetadataProvider
    {
        // public void GetBindingMetadata(BindingMetadataProviderContext context)
        // {
        //     if (context.PropertyAttributes.OfType<RequiredAttribute>().Any())
        //     {
        //         context.BindingMetadata.IsBindingRequired = true;
        //     }
        // }

        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.PropertyAttributes.OfType<RequiredAttribute>().Any())
            {
                context.BindingMetadata.IsBindingRequired = true;
            }
        }
    }
}