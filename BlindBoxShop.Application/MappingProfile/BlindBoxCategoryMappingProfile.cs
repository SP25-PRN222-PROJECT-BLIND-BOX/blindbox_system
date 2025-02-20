using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;

namespace BlindBoxShop.Application.MappingProfile
{
    public class BlindBoxCategoryMappingProfile : Profile
    {
        public BlindBoxCategoryMappingProfile()
        {
            CreateMap<BlindBoxCategory, BlindBoxCategoryDto>();
            CreateMap<BlindBoxCategoryForUpdate, BlindBoxCategory>();
            CreateMap<BlindBoxCategoryForCreate, BlindBoxCategory>();
        }
    }
}
