namespace Glow;

using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

public class JsonSerializationSettings
{
    public const JsonUnionEncoding JsonDefaultWebUnionEncoding = JsonUnionEncoding.AdjacentTag
                                                              | JsonUnionEncoding.UnwrapSingleFieldCases
                                                              | JsonUnionEncoding.UnwrapRecordCases
                                                              | JsonUnionEncoding.UnwrapOption
                                                              | JsonUnionEncoding.UnwrapSingleCaseUnions
                                                              | JsonUnionEncoding.AllowUnorderedTag;

    public static void ConfigureStjSerializerDefaultsForWeb(System.Text.Json.JsonSerializerOptions options)
    {
        options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.Converters.Add(new JsonStringEnumConverter());

        options.Converters.Add(new JsonFSharpConverter(unionEncoding: JsonDefaultWebUnionEncoding));
    }
    public const JsonUnionEncoding JsonDefaultSerializationUnionEncoding = JsonUnionEncoding.AdjacentTag | JsonUnionEncoding.NamedFields
                                                                                                         | JsonUnionEncoding.UnwrapOption
                                                                                                         | JsonUnionEncoding.UnwrapSingleCaseUnions
                                                                                                         | JsonUnionEncoding.AllowUnorderedTag;
}
