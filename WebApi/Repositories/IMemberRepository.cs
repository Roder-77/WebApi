using WebApi.DataModel;
using WebApi.Models;

namespace WebApi.Repositories
{
    public interface IMemberRepository
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
        /// <param name="skip">略過筆數</param>
        /// <param name="take">取得筆數</param>
        /// <returns></returns>
        IEnumerable<Member> GetMembers(int skip, int take);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        Task UpdateMember(MemberModel model);
    }
}
