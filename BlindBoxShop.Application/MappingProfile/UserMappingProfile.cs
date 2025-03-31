using AutoMapper;

using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.Users;

namespace BlindBoxShop.Application.MappingProfile
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserDtoWithRelation>()
                .IncludeBase<User, UserDto>();
        }
    }
}
