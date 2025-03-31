using AutoMapper;

using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxItems;

namespace BlindBoxShop.Application.MappingProfile
{
    public class BlindBoxItemMappingProfile : Profile
    {
        public BlindBoxItemMappingProfile()
        {
            CreateMap<BlindBoxItemDtoForCreation, BlindBoxItem>();
            CreateMap<BlindBoxItemDtoForUpdate, BlindBoxItem>();
            CreateMap<BlindBoxItem, BlindBoxItemDto>();
        }
    }
}
