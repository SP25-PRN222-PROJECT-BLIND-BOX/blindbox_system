using AutoMapper;

using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;

namespace BlindBoxShop.Application.MappingProfile
{
    public class BlindBoxMappingProfile : Profile
    {
        public BlindBoxMappingProfile()
        {
            // Map từ Entity sang DTO
            CreateMap<BlindBox, BlindBoxDto>()
                .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src =>
                src.BlindBoxPriceHistories != null && src.BlindBoxPriceHistories.Any()
                        ? src.BlindBoxPriceHistories
                            .OrderByDescending(i => i.CreatedAt)
                            .First()
                            .Price
                        : 0))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src =>
                    src.BlindBoxImages != null && src.BlindBoxImages.Any()
                    ? src.BlindBoxImages.OrderBy(i => i.CreatedAt).First().ImageUrl
                    : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.BlindBoxCategory != null
                    ? src.BlindBoxCategory.Name
                    : string.Empty))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src =>
                    src.Package != null
                    ? src.Package.Name
                    : string.Empty));

            // Map từ DTO sang Entity
            CreateMap<BlindBoxDto, BlindBox>();

            // Map cho Create
            CreateMap<BlindBoxForCreate, BlindBox>();

            // Map cho Update
            CreateMap<BlindBoxForUpdate, BlindBox>();
            CreateMap<BlindBoxDto, BlindBoxForUpdate>();

            // Map from Entity to DTO
            CreateMap<BlindBoxImage, BlindBoxImageDto>();
            CreateMap<BlindBoxPriceHistory, BlindBoxPriceHistoryDto>();

        }
    }
}