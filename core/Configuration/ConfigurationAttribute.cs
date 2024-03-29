using System;

namespace Glow.Configurations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute,
                                          IConfigurationMeta
    {
        /// <summary>
        /// Required. Unique identifier
        /// </summary>
        public string Id { get; set; }

        public string Policy { get; set; }
        public string ReadPolicy { get; set; }

        private string sectionId;
        /// <summary>
        /// (Optional) The base name for the ASP.NET Core configuration path. If null 'Id' will be used
        /// </summary>
        public string SectionId { get { return sectionId ?? Id; } set { sectionId = value; } }

        private string path;

        /// <summary>
        /// (Optional) Part of the path. The fully qualified path will be
        /// api/configurations/:path
        /// If Path is not set, Id will be used
        /// </summary>
        public string Path
        {
            get { return path ?? Id; }
            [Obsolete("Use Id instead")]
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