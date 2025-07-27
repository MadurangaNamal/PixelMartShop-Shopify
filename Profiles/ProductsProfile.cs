using AutoMapper;
using PixelMartShop.Entities;
using PixelMartShop.Models;

namespace PixelMartShop.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
            .ReverseMap();

        CreateMap<ShopifySharp.Product, Product>()
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
            .ReverseMap();

        CreateMap<ShopifySharp.ProductVariant, ProductVariant>().ReverseMap();

        //CreateMap<ProductDto, Product>()
        //    .ForMember(dest => dest.Id, opt => opt.Ignore());

    }
}
