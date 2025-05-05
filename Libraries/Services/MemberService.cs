using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Models.Requests;
using Models.ViewModels;
using static Services.Extensions.PaginationExtension;

namespace Services
{
    public class MemberService : BaseService<Member>
    {
        public MemberService(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public async Task<MemberVM?> Get(int id)
        {
            var member = await _repository.Table.FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<Member, MemberVM>(member!);
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task Create(InsertMemberRequest request)
        {
            var member = _mapper.Map<InsertMemberRequest, Member>(request);
            await _repository.Insert(member);
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="pageSize">數量</param>
        /// <returns></returns>
        public async Task<PaginationList<MemberVM>> GetList(int page, int pageSize)
        {
            var members = await _repository.Table
                .Where(x => x.IsVerifyByMobile)
                .ToPaginationList(page, pageSize);

            return _mapper.Map<PaginationList<Member>, PaginationList<MemberVM>>(members);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task Update(UpdateMemberRequest request)
        {
            var member = await _repository.Table.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (member is null)
                throw new NullReferenceException();

            member.Name = request.Name;
            member.Mobile = request.Mobile;
            member.Email = request.Email;

            await _repository.Update(member);
        }
    }
}
