using Models.DataModels;
using Models.Request;
using Models.ViewModels;
using static Models.Extensions.PaginationExtension;

namespace Services
{
    public interface IMemberService
    {
        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        MemberVM? GetMember(int id);

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        Task InsertMember(InsertMemberRequest request);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        Task UpdateMember(MemberVM member);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="quantity">數量</param>
        /// <returns></returns>
        PaginationList<MemberVM> GetMembers(int page, int quantity);
    }
}
