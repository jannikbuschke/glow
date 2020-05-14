using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace Glow
{
    public static class InterfaceExportBuilderExtensions
    {
        public static InterfaceExportBuilder<T> WithDefaults<T>(this InterfaceExportBuilder<T> builder)
        {
            return builder.WithPublicProperties()
                .Substitute(typeof(string), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(Guid), new RtSimpleTypeName("string"))
                .Substitute(typeof(Guid?), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(DateTime?), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(DateTime), new RtSimpleTypeName("string"))
                .Substitute(typeof(IEnumerable<string>), new RtSimpleTypeName("string[]"))
                .Substitute(typeof(string[]), new RtSimpleTypeName("string[]"))
                .AutoI(false);
        }
    }

    public class TypescriptGenerationConfiguration
    {
        public static void Configure(ConfigurationBuilder builder)
        {
            builder.Global(options =>
            {
                options.CamelCaseForProperties(true);
                options.UseModules(true);
            });

            //builder.ExportAsInterface<Contacts.Contact>()
            //    .WithDefaults();
        }
    }
}
