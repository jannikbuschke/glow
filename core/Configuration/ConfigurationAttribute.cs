using System;

namespace Glow.Configurations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute, IConfigurationMeta
    {
        public string Id { get; set; }
        public string Policy { get; set; }
        public string ReadPolicy { get; set; }
        public string SectionId { get; set; }
        private string path;
        public string Path
        {
            get { return path ?? Id; }
            set { path = value; }
        }
        public string Route
        {
            get
            {
                return $"api/configurations/{Path}";
            }
        }

        public string Title { get; set; }

        public Meta ToPartialConfiguration()
        {
            return new Meta { Title = Title, Route = Route, Id = Id, SectionId = SectionId };
        }
    }
}
