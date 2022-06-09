using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Models.DataModels
{
    public class Member : BaseDataModel, ICreateEntity, IUpdateEntity
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
        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// 是否通過手機驗證
        /// </summary>
        public bool IsVerifyByMobile { get; set; }

        public long CreateTime { get; set; }

        public string Creator { get; set; }

        public long UpdateTime { get; set; }

        public string Updater { get; set; }
    }
}
