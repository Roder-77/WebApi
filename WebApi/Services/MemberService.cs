using WebApi.DataModel;
using WebApi.Models;
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
        public Member? GetMember(int id) => _repository.GetMember(id);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="quantity">數量</param>
        /// <returns></returns>
        public IEnumerable<Member> GetMembers(int page, int quantity)
        {
            var skip = (page - 1) * quantity;

            return _repository.GetMembers(skip, quantity);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        public async Task UpdateMember(MemberModel model)
        {
            await _repository.UpdateMember(model);
        }
    }
}
