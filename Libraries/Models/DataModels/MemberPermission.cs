using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace Models.DataModels
{
    public class MemberPermission : BaseDataModel
    {
        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }

        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; set; }

        public Member Member { get; set; }

        public Permission Permission { get; set; }
    }
}
