using Models.DataModels;

#nullable disable

namespace Models.ViewModel
{
    public class MemberVM
    {
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
