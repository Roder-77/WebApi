using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace Models.DataModels
{
    public class ProductImage : BaseDataModel
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [StringLength(300)]
        public string Url { get; set; }
    }
}
