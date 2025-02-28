using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.Reply;

namespace BlindBoxShop.Application.MappingProfile
{
    public class ReplyMappingProfile : Profile
    {
        public ReplyMappingProfile() 
        {
            CreateMap<ReplyForUpdateDto, ReplyReviews>();
            CreateMap<ReplyForCreationDto, ReplyReviews>();
            CreateMap<ReplyDto, ReplyForUpdateDto>();
            CreateMap<ReplyReviews, ReplyDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.Reply, opt => opt.MapFrom(src => src.Reply))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
