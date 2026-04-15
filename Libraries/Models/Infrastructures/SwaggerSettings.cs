#nullable disable warnings

using Models;

namespace Models.Infrastructures
{
    public class OpenApiHeader
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }

        public bool Required { get; set; } = false;
    }
}
