using WebApi.DataModel;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Repositories;

namespace WebApi.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repository;

        public MemberService(IMemberRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public Member? GetMember(int id) => _repository.QueryMember().FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        public async Task InsertMember(MemberModel member)
        {
            await _repository.InsertMember(member);
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="quantity">數量</param>
        /// <returns></returns>
        public IEnumerable<Member> GetMembers(int page, int quantity)
        {
            var skip = (page - 1) * quantity;

            return _repository.QueryMember()
                .Where(x => x.IsVerifyByMobile)
                .Skip(skip)
                .Take(quantity)
                .ToList();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        public void UpdateMember(MemberModel member)
        {
            _repository.UpdateMember(member);
        }
    }
}
