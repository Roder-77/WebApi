using System.ComponentModel.DataAnnotations;
using WebApi.DataModel;

namespace WebApi.Models
{
    public class MemberModel
    {
        public MemberModel()
        { }

        public MemberModel(Member member)
        {
            Id = member.Id;
            Name = member.Name;
            Mobile = member.Mobile;
            Email = member.Email;
        }

        /// <summary>
        /// 會員代碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 會員名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 會員手機
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 會員信箱
        /// </summary>
        public string Email { get; set; }
    }
}
