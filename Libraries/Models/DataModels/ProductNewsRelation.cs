using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    public class ProductNewsRelation : BaseDataModel
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(nameof(News))]
        public int NewsId { get; set; }

        public News News { get; set; }
    }
}
