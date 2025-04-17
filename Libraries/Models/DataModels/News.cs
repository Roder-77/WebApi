using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class News : BaseDataModel
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(int.MaxValue)]
        public string Summary { get; set; }
    }
}
