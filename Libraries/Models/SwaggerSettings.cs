#nullable disable warnings

namespace Models
{
    public class SwaggerHeader
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }

        public bool Required { get; set; } = false;
    }
}
