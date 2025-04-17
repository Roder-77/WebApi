using AutoMapper;
using Models.DataModels;
using Models.ViewModels;

namespace Services.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductVM>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(x => x.Url)));

            CreateMap<ProductSpec, ProductSpecVM>();
            CreateMap<ProductNewsRelation, ProductNewsRelationVM>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.News.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.News.Title))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.News.Summary));
        }
    }
}
