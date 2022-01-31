using WebApi.DataModel;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IMemberService
    {
        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        Member? GetMember(int id);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="quantity">數量</param>
        /// <returns></returns>
        IEnumerable<Member> GetMembers(int page, int quantity);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        Task UpdateMember(MemberModel model);
    }
}
