using System.ComponentModel.DataAnnotations;
using Glow.Configurations;
using Glow.TypeScript;

namespace Glow.Sample.Configurations
{
    public class Ts : TypeScriptProfile
    {
        public Ts()
        {
            Add<SampleConfiguration>();
        }
    }

    [Configuration(
        Title = "Sample Configuration",
        Id = "sample-configuration",
        SectionId = "sample-configuration",
        Policy = Policies.Privileged
    )]
    public class SampleConfiguration
    {
        [MinLength(2)]
        public string Prop1 { get; set; }
        public int Prop2 { get; set; }
        public Nested Nested { get; set; }
        public Enum Enum { get; set; }
        public Enum? NullableEnum { get; set; }
    }

    public enum Enum
    {
        EnumVal1 = 1,
        EnumVal2 = 2
    }

    public class Nested
    {
        public string Value { get; set; }
    }
}
