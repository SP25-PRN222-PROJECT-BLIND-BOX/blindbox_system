using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service
{
    public class VoucherService : BaseService, IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        public VoucherService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _voucherRepository = repositoryManager.Voucher;
        }

        private async Task<Result<Voucher>> GetAndCheckIfVoucherExistByIdAsync(Guid id, bool trackChanges)
        {
            var voucher = await _voucherRepository.FindById(id, trackChanges);
            if (voucher is null)
                return Result<Voucher>.Failure(VoucherErrors.GetVoucherNotFoundError(id));

            return Result<Voucher>.Success(voucher);
        }

        public async Task<Result<VoucherDto>> CreateVoucherAsync(VoucherForCreate voucherForCreateDto)
        {
            var voucherEntity = _mapper.Map<Voucher>(voucherForCreateDto);
            voucherEntity.Status = VoucherStatus.Active;
            await _voucherRepository.CreateAsync(voucherEntity);
            await _voucherRepository.SaveAsync();

            var voucherDto = _mapper.Map<VoucherDto>(voucherEntity);

            return Result<VoucherDto>.Success(voucherDto);
        }

        public async Task<Result> DeleteVoucherAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfVoucherExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var voucherEntity = checkIfExistResult.GetValue<Voucher>();

            _voucherRepository.Delete(voucherEntity);
            await _voucherRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<VoucherDto>>> GetVouchersAsync(VoucherParameter voucherParameter, bool trackChanges)
        {
            var vouchers = await _voucherRepository.GetVouchersAsync(voucherParameter, trackChanges);

            var vouchersDto = _mapper.Map<IEnumerable<VoucherDto>>(vouchers);

            return (vouchersDto, vouchers.MetaData);
        }

        public async Task<Result<VoucherDto>> GetVoucherAsync(Guid id, bool trackChanges)
        {
            var checkIfExistResult = await GetAndCheckIfVoucherExistByIdAsync(id, trackChanges);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var voucherEntity = checkIfExistResult.GetValue<Voucher>();

            var voucherDto = _mapper.Map<VoucherDto>(voucherEntity);

            return Result<VoucherDto>.Success(voucherDto);
        }

        public async Task<Result> UpdateVoucherAsync(Guid id, VoucherForUpdate voucherForUpdateDto)
        {
            var checkIfExistResult = await GetAndCheckIfVoucherExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var voucherEntity = checkIfExistResult.GetValue<Voucher>();
            _mapper.Map(voucherForUpdateDto, voucherEntity);
            await _voucherRepository.SaveAsync();

            return Result.Success();
        }

        public void Dispose()
        {
            _voucherRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
