using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace Glow.Core.TsGeneration
{

    //public class CreateTypescriptDefinitions : IStartupFilter
    //{
    //    private readonly IWebHostEnvironment environment;
    //    private readonly Action<ConfigurationBuilder> callback;

    //    public CreateTypescriptDefinitions(
    //        IWebHostEnvironment environment,
    //        Action<ConfigurationBuilder> callback
    //    )
    //    {
    //        this.environment = environment;
    //        this.callback = callback;
    //    }

    //    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    //    {
    //        if (!environment.IsDevelopment())
    //        {
    //            return next;
    //        }
    //        Reinforced.Typings.TsExporter rt = ReinforcedTypings.Initialize(config =>
    //        {
    //            callback.Invoke(config);
    //            //ReinforcedConfiguration.Configure(config);
    //        });
    //        rt.Export(); // <-- this will create the ts files

    //        return next;
    //    }
    //}

    public static class InterfaceExportBuilderExtensions
    {
        public static InterfaceExportBuilder<T> WithDefaults<T>(this InterfaceExportBuilder<T> builder)
        {
            return builder
                .WithPublicProperties()
                .Substitute(typeof(string), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(string[]), new RtSimpleTypeName("string[]"))
                .Substitute(typeof(int?), new RtSimpleTypeName("number|null"))
                .Substitute(typeof(IEnumerable<string>), new RtSimpleTypeName("string[]"))
                .Substitute(typeof(Guid), new RtSimpleTypeName("string"))
                .Substitute(typeof(Guid?), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(DateTime?), new RtSimpleTypeName("string|null"))
                .Substitute(typeof(DateTime), new RtSimpleTypeName("string"))
                .Substitute(typeof(IDictionary<string, object>), new RtSimpleTypeName("any"))
                .OverrideNamespace("models")
                .AutoI(false);
        }
    }
}
