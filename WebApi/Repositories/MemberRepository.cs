using Microsoft.EntityFrameworkCore;
using WebApi.DataModel;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly MemoryContext _context;

        public MemberRepository(MemoryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 查詢會員
        /// </summary>
        /// <param name="hasTracking">是否追蹤資料</param>
        /// <returns></returns>
        public IQueryable<Member> QueryMember(bool hasTracking = false) => hasTracking ? _context.Member : _context.Member.AsNoTracking();

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        public async Task InsertMember(MemberModel model)
        {
            _context.Member.Add(new Member
            {
                Name = model.Name,
                Mobile = model.Mobile,
                Email = model.Email,
                IsVerifyByMobile = true
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        public void UpdateMember(MemberModel model)
        {
            var member = _context.Member
                .FirstOrDefault(x => x.IsVerifyByMobile && x.Id == model.Id);

            if (member == null)
                throw new NullReferenceException();

            member.Name = model.Name;
            member.Mobile = model.Mobile;
            member.Email = model.Email;
        }
    }
}
