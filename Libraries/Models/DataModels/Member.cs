using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class Member : BaseDataModel, ICreateEntity, IUpdateEntity
    {
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public long CreateTime { get; set; }

        public int Creator { get; set; }

        public long UpdateTime { get; set; }

        public int Updater { get; set; }

        public ICollection<MemberPermission> Permissions { get; set; } = new HashSet<MemberPermission>();
    }
}
