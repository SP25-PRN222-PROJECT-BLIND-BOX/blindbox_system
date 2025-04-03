using AutoMapper;
using BlindBoxShop.Entities.Models; 
using BlindBoxShop.Shared.DataTransferObject.Account;

namespace BlindBoxShop.Application.MappingProfile
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<User, AccountDto>();

            //map for create
            CreateMap<AccountForCreateDto, User>();
       
            //map for update
            CreateMap<AccountForUpdateDto, User>();

            //map for manage
            CreateMap<AccountDto, User>();
        }
    }
}
