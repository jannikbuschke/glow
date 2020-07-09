using System;

namespace EfConfigurationProvider.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GeneratedControllerAttribute : Attribute
    {
        public GeneratedControllerAttribute(string path)
        {
            Route = $"api/configurations/{path}";
            Path = path;
        }

        public string Path { get; set; }

        public string Route { get; set; }

        public string Title { get; set; }
    }
}
