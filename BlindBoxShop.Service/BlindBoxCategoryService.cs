using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Service
{
    public class BlindBoxCategoryService : BaseService, IBlindBoxCategoryService
    {
        private readonly IBlindBoxCategoryRepository _blindboxCategoryRepository;
        public BlindBoxCategoryService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindboxCategoryRepository = repositoryManager.BlindBoxCategory;
        }

        private async Task<Result<BlindBoxCategory>> GetAndCheckIfBlindBoxCategoryExistByIdAsync(Guid id, bool trackChanges)
        {
            var blindBoxCategory = await _blindboxCategoryRepository.FindById(id, trackChanges);
            if (blindBoxCategory is null)
                return Result<BlindBoxCategory>.Failure(BlindBoxCategoryErrors.GetBlindBoxCategoryNotFoundError(id));

            return Result<BlindBoxCategory>.Success(blindBoxCategory);
        }

        private async Task<Result> BlindBoxCategoryExistByNameAsync(string name)
        {

            var checkExist = await _blindboxCategoryRepository.FindByCondition(e => e.Name.ToLower().Equals(name.ToLower()), false).AnyAsync();
            if (checkExist)
                return Result<BlindBoxCategory>.Failure(BlindBoxCategoryErrors.GetBlindBoxCategoryExistError(name));

            return Result.Success();
        }

        public async Task<Result<BlindBoxCategoryDto>> CreateBlindBoxCategoryAsync(BlindBoxCategoryForCreate blindBoxCategoryForCreate)
        {
            var checkIfExistResult = await BlindBoxCategoryExistByNameAsync(blindBoxCategoryForCreate.Name);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var blindBoxCategoryEntity = _mapper.Map<BlindBoxCategory>(blindBoxCategoryForCreate);


            await _blindboxCategoryRepository.CreateAsync(blindBoxCategoryEntity);
            await _blindboxCategoryRepository.SaveAsync();

            var blindBoxCategoryDto = _mapper.Map<BlindBoxCategoryDto>(blindBoxCategoryEntity);

            return blindBoxCategoryDto;
        }

        public async Task<Result> DeleteBlindBoxCategoryAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfBlindBoxCategoryExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult;

            var blindBoxCategoryEntity = checkIfExistResult.GetValue<BlindBoxCategory>();


            _blindboxCategoryRepository.Delete(blindBoxCategoryEntity);
            await _blindboxCategoryRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<BlindBoxCategoryDto>>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges)
        {
            var blindBoxCategoriesWithMetaData = await _repositoryManager.BlindBoxCategory.GetBlindBoxCategoriesAsync(blindBoxCategoryParameter, trackChanges);

            var blindBoxCategoriesDto = _mapper.Map<IEnumerable<BlindBoxCategoryDto>>(blindBoxCategoriesWithMetaData);

            return (blindBoxCategoriesDto, blindBoxCategoriesWithMetaData.MetaData);

        }

        public async Task<Result<BlindBoxCategoryDto>> GetBlindBoxCategoryAsync(Guid id, bool trackChanges)
        {
            var checkIfExistResult = await GetAndCheckIfBlindBoxCategoryExistByIdAsync(id, trackChanges);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var blindBoxCategoryEntity = checkIfExistResult.GetValue<BlindBoxCategory>();

            var blindBoxCategoryDto = _mapper.Map<BlindBoxCategoryDto>(blindBoxCategoryEntity);

            return blindBoxCategoryDto;
        }

        public async Task<Result> UpdateBlindBoxCategoryAsync(Guid id, BlindBoxCategoryForUpdate blindBoxCategoryForUpdate)
        {
            var checkIfExistResult = await GetAndCheckIfBlindBoxCategoryExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var blindBoxCategoryEntity = checkIfExistResult.GetValue<BlindBoxCategory>();
            _mapper.Map(blindBoxCategoryForUpdate, blindBoxCategoryEntity);
            await _blindboxCategoryRepository.SaveAsync();

            return Result.Success();
        }
    }
}
