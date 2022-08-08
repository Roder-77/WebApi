using AutoMapper;
using Models.DataModels;
using Models.Request;
using Models.ViewModels;
using static Services.Extensions.PaginationExtension;

namespace Services.Profiles
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<Member, MemberVM>();
            CreateMap<MemberVM, Member>();

            CreateMap<InsertMemberRequest, Member>();

            CreateMap<PaginationList<Member>, PaginationList<MemberVM>>();
        }
    }
}
