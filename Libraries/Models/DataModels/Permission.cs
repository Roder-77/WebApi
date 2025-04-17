using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class Permission : BaseDataModel
    {
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<PermissionMenuRelation> MenuRelations { get; set; } = new HashSet<PermissionMenuRelation>();
    }
}
