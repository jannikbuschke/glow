using System.ComponentModel.DataAnnotations;
using Glow.Configurations;

namespace Glow.Sample.Configurations
{
    [Configuration(
        Title = "Sample Configuration",
        Id = "sample-configuration",
        SectionId = "sample-configuration",
        Policy = "test-policy"
    )]
    public class SampleConfiguration
    {
        [MinLength(2)]
        public string Prop1 { get; set; }
        public int Prop2 { get; set; }
    }
}
