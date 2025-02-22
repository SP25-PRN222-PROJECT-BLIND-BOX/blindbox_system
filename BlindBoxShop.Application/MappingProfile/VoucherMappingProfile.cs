using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.Voucher;

namespace BlindBoxShop.Application.MappingProfile
{
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            CreateMap<Voucher, VoucherDto>();
            CreateMap<VoucherDto, VoucherDto>();
            CreateMap<VoucherForUpdate, Voucher>();
            CreateMap<VoucherForCreate, Voucher>();
        }
    }
}
