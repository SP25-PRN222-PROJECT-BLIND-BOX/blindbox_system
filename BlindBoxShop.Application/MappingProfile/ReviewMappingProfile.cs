using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.CustomerReview;

namespace BlindBoxShop.Application.MappingProfile
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile() 
        {
            CreateMap<ReviewForUpdateDto, CustomerReviews>();
            CreateMap<ReviewForCreationDto, CustomerReviews>();
            CreateMap<ReviewDto, ReviewForUpdateDto>();
            CreateMap<CustomerReviews, ReviewDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.BlindBoxName, opt => opt.MapFrom(src => src.BlindBox != null ? src.BlindBox.Name : string.Empty))
                .ForMember(dest => dest.FeedBack, opt => opt.MapFrom(src => src.FeedBack))
                .ForMember(dest => dest.RatingStar, opt => opt.MapFrom(src => src.RatingStar))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
