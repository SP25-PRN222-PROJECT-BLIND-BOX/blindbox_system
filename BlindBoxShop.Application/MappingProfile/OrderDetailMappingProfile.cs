using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;

namespace BlindBoxShop.Application.MappingProfile
{
    public class OrderDetailMappingProfile : Profile
    {
        public OrderDetailMappingProfile() 
        {
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.BlindBoxName, opt => opt.MapFrom(src => src.BlindBoxPriceHistory!.BlindBox!.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BlindBoxPriceHistory!.Price))
                .ForMember(dest => dest.BlindBoxItemId, opt => opt.MapFrom(src => src.BlindBoxItemId))
                .ReverseMap();

            CreateMap<Order, OrderWithDetailsDto>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
        }
    }
}
