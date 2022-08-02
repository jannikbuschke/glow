namespace Glow;

using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

public class JsonSerializationSettings
{
    public const JsonUnionEncoding JsonDefaultUnionEncoding = JsonUnionEncoding.AdjacentTag
                                                              | JsonUnionEncoding.UnwrapRecordCases
                                                              | JsonUnionEncoding.UnwrapOption
                                                              | JsonUnionEncoding.UnwrapSingleCaseUnions
                                                              | JsonUnionEncoding.AllowUnorderedTag;

    public static void ConfigureStjSerializerDefaults(System.Text.Json.JsonSerializerOptions options)
    {
        options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new JsonFSharpConverter(
            unionEncoding: JsonDefaultUnionEncoding));
    }
}