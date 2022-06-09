using Models;
using Models.DataModels;
using Services.Repositories;
using static Models.Extensions.PaginationExtension;

namespace Services
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _repository;

        public MemberService(IGenericRepository<Member> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public Member? GetMember(int id) => _repository.GetById(id);

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="member">會員資料</param>
        /// <returns></returns>
        public async Task InsertMember(MemberModel member)
        {
            await _repository.Insert(new Member
            {
                Name = member.Name,
                Mobile = member.Mobile,
                Email = member.Email,
                IsVerifyByMobile = true
            });
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="pageSize">數量</param>
        /// <returns></returns>
        public PaginationList<Member> GetMembers(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            return _repository.TableWithoutTracking
                .Where(x => x.IsVerifyByMobile)
                .ToPaginationList(page, pageSize);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        public async Task UpdateMember(MemberModel model)
        {
            var member = _repository.GetById(model.Id, x => x.IsVerifyByMobile);

            if (member == null)
                throw new NullReferenceException();

            member.Name = model.Name;
            member.Mobile = model.Mobile;
            member.Email = model.Email;

            await _repository.Update(member);
        }
    }
}
