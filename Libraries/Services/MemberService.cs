using AutoMapper;
using Microsoft.AspNetCore.Http;
using Models.DataModels;
using Models.Request;
using Models.ViewModels;
using static Services.Extensions.PaginationExtension;

namespace Services
{
    public class MemberService : BaseService<Member>
    {
        private readonly IMapper _mapper;

        public MemberService(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public async Task<MemberVM?> GetMember(int id)
        {
            var member = await _repository.GetById(id);

            return _mapper.Map<Member, MemberVM>(member);
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task InsertMember(InsertMemberRequest request)
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
        public async Task<PaginationList<MemberVM>> GetMembers(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var members = await _repository.TableWithoutTracking
                .Where(x => x.IsVerifyByMobile)
                .ToPaginationList(page, pageSize);

            return _mapper.Map<PaginationList<Member>, PaginationList<MemberVM>>(members);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task UpdateMember(UpdateMemberRequest request)
        {
            var member = await _repository.GetById(request.Id);

            if (member == null)
                throw new NullReferenceException();

            member.Name = request.Name;
            member.Mobile = request.Mobile;
            member.Email = request.Email;

            await _repository.Update(member);
        }
    }
}
