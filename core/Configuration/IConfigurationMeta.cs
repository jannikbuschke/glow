namespace Glow.Configurations
{
    public interface IConfigurationMeta
    {
        string Route { get; }
        string Title { get; set; }
        string Id { get; }
        public string SectionId { get; set; }
    }
}
