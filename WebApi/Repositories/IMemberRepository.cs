using WebApi.DataModel;
using WebApi.Models;

namespace WebApi.Repositories
{
    public interface IMemberRepository
    {
        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="hasTracking">是否追蹤資料</param>
        /// <returns></returns>
        IQueryable<Member> QueryMember(bool hasTracking = false);

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        Task InsertMember(MemberModel model);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        void UpdateMember(MemberModel model);
    }
}
