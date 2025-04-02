using AutoMapper;

using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;

using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<bool>> DeleteBlindBoxAsync(Guid blindBoxId, bool trackChanges)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, trackChanges);
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

                // If this is an online BlindBox (Probability > 0), get its items
                if (blindBox.Probability > 0)
                {
                    // Get BlindBoxItems related to this BlindBox
                    var items = await _repositoryManager.BlindBoxItem
                        .FindByCondition(item => item.BlindBoxId == blindBoxId, trackChanges)
                        .ToListAsync();

                    if (items.Any())
                    {
                        // Map items to DTOs
                        var itemDtos = new List<BlindBoxItemDto>();
                        foreach (var item in items)
                        {
                            var itemDto = new BlindBoxItemDto
                            {
                                Id = item.Id,
                                BlindBoxId = item.BlindBoxId,
                                Name = item.Name,
                                Description = item.Description,
                                Rarity = (int)item.Rarity,
                                ImageUrl = item.ImageUrl,
                                IsSecret = item.IsSecret,
                                CreatedAt = item.CreatedAt
                            };
                            itemDtos.Add(itemDto);
                        }
                        blindBoxDto.Items = itemDtos;
                    }
                }

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
                var blindBoxes = await _blindBoxRepository.GetBlindBoxesAsync(blindBoxParameter, trackChanges);

                var blindBoxDtos = _mapper.Map<IEnumerable<BlindBoxDto>>(blindBoxes);

                return (blindBoxDtos, blindBoxes.MetaData);
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

        public async Task<Result<BlindBoxDto>> UpdateBlindBoxAsync(Guid blindBoxId, BlindBoxForUpdate blindBoxForUpdate, bool trackChanges)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, trackChanges);
                if (blindBox == null)
                    return Result<BlindBoxDto>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                _mapper.Map(blindBoxForUpdate, blindBox);
                blindBox.UpdatedAt = DateTime.UtcNow;

                _blindBoxRepository.Update(blindBox);
                await _blindBoxRepository.SaveAsync();

                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);

                // Create a new price history entry with the updated price
                var blindBoxPriceHistory = new BlindBoxPriceHistory
                {
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = blindBoxForUpdate.Price,
                    Price = blindBoxForUpdate.Price,
                    CreatedAt = DateTime.UtcNow
                };

                _repositoryManager.BlindBoxPriceHistory.Create(blindBoxPriceHistory);
                await _repositoryManager.BlindBoxPriceHistory.SaveAsync();

                // Set the current price in the DTO
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

        public async Task<Result<bool>> ResetBlindBoxProbabilityAsync(Guid blindBoxId)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, true);
                if (blindBox == null)
                    return Result<bool>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                // Reset the probability to a default value (e.g., 0.0)
                blindBox.Probability = 0.0f;
                blindBox.UpdatedAt = DateTime.UtcNow;

                _blindBoxRepository.Update(blindBox);
                await _blindBoxRepository.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "ResetBlindBoxProbabilityError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> IncrementBlindBoxProbabilityAsync(Guid blindBoxId)
        {
            try
            {
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, true);
                if (blindBox == null)
                    return Result<bool>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                // Increment the probability by a fixed amount (e.g., 0.1)
                blindBox.Probability += 0.1f;
                blindBox.UpdatedAt = DateTime.UtcNow;

                _blindBoxRepository.Update(blindBox);
                await _blindBoxRepository.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "IncrementBlindBoxProbabilityError",
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
