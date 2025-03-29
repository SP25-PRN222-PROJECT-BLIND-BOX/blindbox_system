using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class BlindBoxService : BaseService, IBlindBoxService
    {
        private readonly IBlindBoxRepository _blindBoxRepository;
        public BlindBoxService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxRepository = repositoryManager.BlindBox;
        }

        public async Task<Result<BlindBoxDto>> CreateBlindBoxAsync(BlindBoxForCreate blindBoxForCreate)
        {
            try
            {
                var blindBox = _mapper.Map<BlindBox>(blindBoxForCreate);
                _blindBoxRepository.Create(blindBox);
                await _blindBoxRepository.SaveAsync();

                // Create initial price history
                var blindBoxPriceHistory = new BlindBoxPriceHistory
                {
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = blindBoxForCreate.Price,
                    Price = blindBoxForCreate.Price,
                    CreatedAt = DateTime.UtcNow
                };

                _repositoryManager.BlindBoxPriceHistory.Create(blindBoxPriceHistory);
                await _repositoryManager.BlindBoxPriceHistory.SaveAsync();

                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);
                blindBoxDto.CurrentPrice = blindBoxForCreate.Price;

                return Result<BlindBoxDto>.Success(blindBoxDto);
            }
            catch (Exception ex)
            {
                return Result<BlindBoxDto>.Failure(new ErrorResult
                {
                    Code = "CreateBlindBoxError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> DeleteBlindBoxAsync(Guid blindBoxId)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, true);
                if (blindBox == null)
                    return Result<bool>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                _blindBoxRepository.Delete(blindBox);
                await _blindBoxRepository.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "DeleteBlindBoxError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<BlindBoxDto>> GetBlindBoxByIdAsync(Guid blindBoxId, bool trackChanges)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, trackChanges);
                if (blindBox == null)
                    return Result<BlindBoxDto>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);

                // Get current price from the latest price history
                var priceHistories = _repositoryManager.BlindBoxPriceHistory
                    .FindByCondition(ph => ph.BlindBoxId == blindBoxId, trackChanges)
                    .ToList();

                var latestPrice = priceHistories.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                if (latestPrice != null)
                    blindBoxDto.CurrentPrice = latestPrice.Price;

                return Result<BlindBoxDto>.Success(blindBoxDto);
            }
            catch (Exception ex)
            {
                return Result<BlindBoxDto>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<IEnumerable<BlindBoxDto>>> GetBlindBoxesAsync(BlindBoxParameter blindBoxParameter, bool trackChanges)
        {
            try
            {
                var blindBoxes = _blindBoxRepository.FindAll(trackChanges);
                
                // Apply filtering by category if specified
                if (blindBoxParameter.CategoryId.HasValue)
                {
                    blindBoxes = blindBoxes.Where(bb => bb.BlindBoxCategoryId == blindBoxParameter.CategoryId.Value);
                }

                // Apply search by name if specified
                if (!string.IsNullOrWhiteSpace(blindBoxParameter.SearchByName))
                {
                    blindBoxes = blindBoxes.Where(bb => 
                        bb.Name.Contains(blindBoxParameter.SearchByName, StringComparison.OrdinalIgnoreCase) || 
                        bb.Description.Contains(blindBoxParameter.SearchByName, StringComparison.OrdinalIgnoreCase));
                }

                // Apply package filter if specified
                if (blindBoxParameter.PackageId.HasValue)
                {
                    blindBoxes = blindBoxes.Where(bb => bb.PackageId == blindBoxParameter.PackageId.Value);
                }

                // Apply status filter if specified
                if (blindBoxParameter.Status.HasValue)
                {
                    blindBoxes = blindBoxes.Where(bb => (int)bb.Status == blindBoxParameter.Status.Value);
                }

                // Apply rarity filter if specified
                if (blindBoxParameter.Rarity.HasValue)
                {
                    blindBoxes = blindBoxes.Where(bb => (int)bb.Rarity == blindBoxParameter.Rarity.Value);
                }

                // Apply sorting
                if (string.IsNullOrWhiteSpace(blindBoxParameter.OrderBy))
                {
                    blindBoxes = blindBoxes.OrderByDescending(e => e.CreatedAt);
                }
                else
                {
                    // Default to ordering by creation date if order param is invalid
                    blindBoxes = blindBoxes.OrderByDescending(e => e.CreatedAt);
                }

                // Apply pagination
                blindBoxes = blindBoxes
                    .Skip((blindBoxParameter.PageNumber - 1) * blindBoxParameter.PageSize)
                    .Take(blindBoxParameter.PageSize);

                var blindBoxList = await blindBoxes.ToListAsync();
                var blindBoxDtos = _mapper.Map<IEnumerable<BlindBoxDto>>(blindBoxList);

                // Get current prices for each blindbox
                foreach (var dto in blindBoxDtos)
                {
                    var priceHistories = _repositoryManager.BlindBoxPriceHistory
                        .FindByCondition(ph => ph.BlindBoxId == dto.Id, trackChanges)
                        .ToList();

                    var latestPrice = priceHistories.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                    if (latestPrice != null)
                        dto.CurrentPrice = latestPrice.Price;
                }

                return Result<IEnumerable<BlindBoxDto>>.Success(blindBoxDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxDto>>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxesError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<BlindBoxDto>> UpdateBlindBoxAsync(Guid blindBoxId, BlindBoxForUpdate blindBoxForUpdate)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, true);
                if (blindBox == null)
                    return Result<BlindBoxDto>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                _mapper.Map(blindBoxForUpdate, blindBox);
                
                // Check if price changed, if so create a new price history
                var currentPriceHistories = _repositoryManager.BlindBoxPriceHistory
                    .FindByCondition(ph => ph.BlindBoxId == blindBoxId, true)
                    .ToList();

                var latestPrice = currentPriceHistories.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                if (latestPrice != null && latestPrice.Price != blindBoxForUpdate.Price)
                {
                    var blindBoxPriceHistory = new BlindBoxPriceHistory
                    {
                        BlindBoxId = blindBox.Id,
                        DefaultPrice = latestPrice.DefaultPrice,
                        Price = blindBoxForUpdate.Price,
                        CreatedAt = DateTime.UtcNow
                    };

                    _repositoryManager.BlindBoxPriceHistory.Create(blindBoxPriceHistory);
                    await _repositoryManager.BlindBoxPriceHistory.SaveAsync();
                }

                blindBox.UpdatedAt = DateTime.UtcNow;
                await _blindBoxRepository.SaveAsync();

                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);
                blindBoxDto.CurrentPrice = blindBoxForUpdate.Price;

                return Result<BlindBoxDto>.Success(blindBoxDto);
            }
            catch (Exception ex)
            {
                return Result<BlindBoxDto>.Failure(new ErrorResult
                {
                    Code = "UpdateBlindBoxError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<IEnumerable<BlindBoxDto>>> GetBlindBoxesByPackageIdAsync(Guid packageId, bool trackChanges)
        {
            try
            {
                // Create a query for blindboxes that belong to the specified package
                var blindBoxes = _blindBoxRepository
                    .FindByCondition(bb => bb.PackageId == packageId, trackChanges);
                
                var blindBoxList = await blindBoxes.ToListAsync();
                var blindBoxDtos = _mapper.Map<IEnumerable<BlindBoxDto>>(blindBoxList);
                
                // Get current prices for each blindbox
                foreach (var dto in blindBoxDtos)
                {
                    var priceHistories = _repositoryManager.BlindBoxPriceHistory
                        .FindByCondition(ph => ph.BlindBoxId == dto.Id, trackChanges)
                        .ToList();

                    var latestPrice = priceHistories.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                    if (latestPrice != null)
                        dto.CurrentPrice = latestPrice.Price;
                }
                
                return Result<IEnumerable<BlindBoxDto>>.Success(blindBoxDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxDto>>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxesByPackageIdError",
                    Description = ex.Message
                });
            }
        }

        public void Dispose()
        {
            _blindBoxRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
