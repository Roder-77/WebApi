using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace Models.DataModels
{
    public class PermissionMenuRelation : BaseDataModel
    {
        [ForeignKey(nameof(Menu))]
        public int MenuId { get; set; }

        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; set; }

        public bool AllowCreate { get; set; }

        public bool AllowRead { get; set; }

        public bool AllowUpdate { get; set; }

        public bool AllowDelete { get; set; }

        public Menu Menu { get; set; }

        public Permission Permission { get; set; }
    }
}
