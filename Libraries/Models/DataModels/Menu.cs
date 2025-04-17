using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class Menu : BaseDataModel
    {
        [StringLength(50)]
        public string Name { get; set; }
    }
}
