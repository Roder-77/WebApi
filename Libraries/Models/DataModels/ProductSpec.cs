using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace Models.DataModels
{
    public class ProductSpec : BaseDataModel
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
