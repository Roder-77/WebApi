#nullable disable warnings

namespace Models.ViewModels
{
    public class PermissionVM
    {
        public string Name { get; set; }
    }

    public class PermissionMenuRelationVM
    {
        public int MenuId { get; set; }

        public string PermissionName { get; set; }

        public bool AllowCreate { get; set; }

        public bool AllowRead { get; set; }

        public bool AllowUpdate { get; set; }

        public bool AllowDelete { get; set; }
    }
}
