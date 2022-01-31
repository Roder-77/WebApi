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
        /// 取得會員
        /// </summary>
        /// <param name="id">會員代碼</param>
        /// <returns></returns>
        public Member? GetMember(int id) => _context.Member.FirstOrDefault(x => x.IsVerifyByMobile && x.Id == id);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="skip">略過筆數</param>
        /// <param name="take">取得筆數</param>
        /// <returns></returns>
        public IEnumerable<Member> GetMembers(int skip, int take)
        {
            return _context.Member
                .Where(x => x.IsVerifyByMobile)
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        public async Task UpdateMember(MemberModel model)
        {
            var member = _context.Member
                .FirstOrDefault(x => x.IsVerifyByMobile && x.Id == model.Id);

            if (member == null)
                throw new NullReferenceException();

            member.Name = model.Name;
            member.Mobile = model.Mobile;
            member.Email = model.Email;

            await _context.SaveChangesAsync();
        }
    }
}
