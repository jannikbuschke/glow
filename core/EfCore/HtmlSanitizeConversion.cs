using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ganss.XSS;

namespace Glow.Core.EfCore
{
    /// <summary>
    /// Sanitizes html input (removes script tags and other problematic html code) on database updates
    /// This is usefull if users are able to insert html code into the database (i.e. by using a html based rich text editor)
    /// Does not do anything on reads
    /// </summary>
    public class HtmlSanitizeValueConverter : ValueConverter<string, string>
    {
        private static HtmlSanitizer sanitizer = new ();

        private static Expression<Func<string, string>> convertToProviderExpression =
            x => sanitizer.Sanitize(x, null, null);

        private static Expression<Func<string, string>> convertFromProviderExpression = x => x;

        public HtmlSanitizeValueConverter() : base(convertToProviderExpression, convertFromProviderExpression, null)
        {
        }
    }

    /// <summary>
    /// Sanitizes html input (removes script tags and other problematic html code) on database updates
    /// This is usefull if users are able to insert html code into the database (i.e. by using a html based rich text editor)
    /// Does not do anything on reads
    /// </summary>
    public static class HtmlSanitizeValueConverterExtension
    {
        public static PropertyBuilder<string> HasHtmlSanitizeConversion(this PropertyBuilder<string> self)
        {
            return self.HasConversion(new HtmlSanitizeValueConverter());
        }
    }
}