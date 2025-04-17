using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Models.Requests;
using Models.ViewModels;
using Services.Repositories;
using static Services.Extensions.PaginationExtension;

namespace Services
{
    public class MemberService : BaseService<Member>
    {
        private readonly IGenericRepository<MemberPermission> _memberPermissionRepository;

        public MemberService(
            IGenericRepository<MemberPermission> memberPermissionRepository,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _memberPermissionRepository = memberPermissionRepository;
        }

        /// <summary>
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public async Task<MemberVM?> Get(int id)
        {
            var member = await _repository.GetById(id);

            return _mapper.Map<MemberVM>(member);
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task Create(InsertMemberRequest request)
        {
            var member = _mapper.Map<Member>(request);
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
            var skip = (page - 1) * pageSize;

            var members = await _repository.Table
                .ToPaginationList(page, pageSize);

            return _mapper.Map<PaginationList<MemberVM>>(members);
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="request">會員資料</param>
        /// <returns></returns>
        public async Task Update(UpdateMemberRequest request)
        {
            var member = await _repository.GetById(request.Id);

            if (member == null)
                throw new NullReferenceException();

            member.Name = request.Name;
            member.Mobile = request.Mobile;
            member.Email = request.Email;

            await _repository.Update(member);
        }

        public async Task<List<PermissionMenuRelationVM>> GetMenu(int id, int menuId)
        {
            var menuRelation = await _memberPermissionRepository.Table
                .Where(x => x.Member.Id == id)
                .SelectMany(x => x.Permission.MenuRelations, (x, y) => new PermissionMenuRelationVM
                {
                    MenuId = y.MenuId,
                    PermissionName = x.Permission.Name,
                    AllowCreate = y.AllowCreate,
                    AllowRead = y.AllowRead,
                    AllowUpdate = y.AllowUpdate,
                    AllowDelete = y.AllowDelete,
                })
                .Where(x => x.MenuId == menuId)
                .ToListAsync();

            return menuRelation;
        }
    }
}
