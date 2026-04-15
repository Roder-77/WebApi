using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Models.Requests
{
    public class InsertMemberRequest
    {
        /// <summary>
        /// 會員名稱
        /// </summary>
        [Required]
        [StringLength(20)]
        [Description("會員名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 會員手機
        /// </summary>
        [Required]
        [StringLength(10)]
        [Description("會員手機")]
        public string Mobile { get; set; }

        /// <summary>
        /// 會員信箱
        /// </summary>
        [StringLength(50)]
        [Description("會員信箱")]
        public string Email { get; set; }
    }
}
