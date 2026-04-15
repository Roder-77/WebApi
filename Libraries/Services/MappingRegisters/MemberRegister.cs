using Mapster;
using Models.DataModels;
using Models.Requests;
using Models.ViewModels;
using static Services.Extensions.PaginationExtension;

namespace Services.MappingRegisters
{
    public class MemberRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Member, MemberVM>();
            config.NewConfig<MemberVM, Member>();
            config.NewConfig<InsertMemberRequest, Member>();
            config.NewConfig<PaginationList<Member>, PaginationList<MemberVM>>();
        }
    }
}
