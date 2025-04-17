#nullable disable warnings

namespace Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Intro { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }

        public IEnumerable<ProductSpecVM> Specs { get; set; }

        public IEnumerable<ProductNewsRelationVM> NewsRelations { get; set; }
    }

    public class ProductSpecVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductNewsRelationVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
    }
}