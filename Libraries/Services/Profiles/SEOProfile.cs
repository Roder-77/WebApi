using AutoMapper;
using Models.DataModels;
using Models.ViewModels;

namespace Services.Profiles
{
    public class SEOProfile : Profile
    {
        public SEOProfile()
        {
            CreateMap<SEO, SEOVM>();
        }
    }
}
