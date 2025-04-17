using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class Product : BaseDataModel
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Intro { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();

        public ICollection<ProductSpec> Specs { get; set; } = new HashSet<ProductSpec>();

        public ICollection<ProductNewsRelation> NewsRelations { get; set; } = new HashSet<ProductNewsRelation>();
    }
}
