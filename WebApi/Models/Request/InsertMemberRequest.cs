using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Request
{
    public class InsertMemberRequest
    {
        /// <summary>
        /// 會員名稱
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 會員手機
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Mobile { get; set; }

        /// <summary>
        /// 會員信箱
        /// </summary>
        [StringLength(50)]
        public string Email { get; set; }
    }
}
