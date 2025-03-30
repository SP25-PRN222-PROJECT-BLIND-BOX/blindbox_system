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
                    Id = Guid.NewGuid(),
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = blindBoxForCreate.Price,
                    Price = blindBoxForCreate.Price,
                    DefaultProbability = (decimal)blindBox.Probability,
                    Probability = (decimal)blindBox.Probability,
                    CreatedAt = DateTime.Now
                };

                var blindBoxPriceHistoryRepository = _repositoryManager.BlindBoxPriceHistory;
                blindBoxPriceHistoryRepository.Create(blindBoxPriceHistory);
                await blindBoxPriceHistoryRepository.SaveAsync();

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

                // Get current price AND probability from the latest price history
                var priceHistories = _repositoryManager.BlindBoxPriceHistory
                    .FindByCondition(ph => ph.BlindBoxId == blindBoxId, trackChanges)
                    .ToList();

                var latestPrice = priceHistories.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                if (latestPrice != null)
                {
                    blindBoxDto.CurrentPrice = latestPrice.Price;
                    // Use the probability from price history instead of from the BlindBox entity
                    blindBoxDto.Probability = (float)latestPrice.Probability;
                    blindBoxDto.ProbabilitySource = $"PriceHistory({latestPrice.Id}) - Probability:{latestPrice.Probability}, DefaultProbability:{latestPrice.DefaultProbability}";
                    Console.WriteLine($"GetBlindBoxByIdAsync: Setting probability to {blindBoxDto.Probability}% and price to {blindBoxDto.CurrentPrice} from price history");
                }
                else
                {
                    Console.WriteLine($"GetBlindBoxByIdAsync: No price history found for BlindBox {blindBoxId}, using entity values");
                    // If no price history exists, fall back to the entity values
                    blindBoxDto.CurrentPrice = 0;
                    blindBoxDto.Probability = blindBox.Probability;
                    blindBoxDto.ProbabilitySource = $"BlindBoxEntity - Probability:{blindBox.Probability}";
                }
                    
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
                    {
                        dto.CurrentPrice = latestPrice.Price;
                        // Also set probability from the price history
                        dto.Probability = (float)latestPrice.Probability;
                        dto.ProbabilitySource = $"PriceHistory({latestPrice.Id}) - Probability:{latestPrice.Probability}, DefaultProbability:{latestPrice.DefaultProbability}";
                    }
                    else
                    {
                        dto.ProbabilitySource = $"BlindBoxEntity - No PriceHistory found";
                    }
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
                    Id = Guid.NewGuid(),
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = blindBoxForUpdate.Price,
                    Price = blindBoxForUpdate.Price,
                    DefaultProbability = (decimal)blindBox.Probability,
                    Probability = (decimal)blindBox.Probability,
                    CreatedAt = DateTime.Now
                };

                var blindBoxPriceHistoryRepository = _repositoryManager.BlindBoxPriceHistory;
                blindBoxPriceHistoryRepository.Create(blindBoxPriceHistory);
                await blindBoxPriceHistoryRepository.SaveAsync();

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
                    {
                        dto.CurrentPrice = latestPrice.Price;
                        // Also set probability from the price history
                        dto.Probability = (float)latestPrice.Probability;
                        dto.ProbabilitySource = $"PriceHistory({latestPrice.Id}) - Probability:{latestPrice.Probability}, DefaultProbability:{latestPrice.DefaultProbability}";
                    }
                    else
                    {
                        dto.ProbabilitySource = $"BlindBoxEntity - No PriceHistory found";
                    }
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
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, false);
                if (blindBox == null)
                    return Result<bool>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));

                // Get the latest price history for this BlindBox
                var priceHistoryRepo = _repositoryManager.BlindBoxPriceHistory;
                var latestPriceHistory = await priceHistoryRepo.GetLatestPriceHistoryByBlindBoxIdAsync(blindBoxId, false);
                
                // Create new price history record with reset values based on defaults
                    var newPriceHistory = new BlindBoxPriceHistory
                    {
                        BlindBoxId = blindBoxId,
                        DefaultProbability = latestPriceHistory.DefaultProbability,
                        Probability = latestPriceHistory.DefaultProbability, // Reset to default
                        DefaultPrice = latestPriceHistory.DefaultPrice,
                        Price = latestPriceHistory.DefaultPrice,             // Reset to default
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    priceHistoryRepo.Create(newPriceHistory);
                    await priceHistoryRepo.SaveAsync();
                    
                    Console.WriteLine($"Created new reset price history record with ID: {newPriceHistory.Id} for BlindBox {blindBoxId}");
                    Console.WriteLine($"Reset probability to {newPriceHistory.Probability}%, price to {newPriceHistory.Price}₫");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in ResetBlindBoxProbabilityAsync: {ex.Message}");
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

        // New method to increment both probability and price when no secret item is found
        public async Task<Result<BlindBoxDto>> IncrementProbabilityAndPriceAsync(Guid blindBoxId)
        {
            try
            {
                // Get the BlindBox for returning later
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, false);
                if (blindBox == null)
                    return Result<BlindBoxDto>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));
                
                // Get the current price history for this blindbox
                var priceHistories = _repositoryManager.BlindBoxPriceHistory
                    .FindByCondition(ph => ph.BlindBoxId == blindBoxId, false)
                    .OrderByDescending(ph => ph.CreatedAt)
                    .ToList();
                
                Console.WriteLine($"Found {priceHistories.Count} price history entries for BlindBox {blindBoxId}");
                    
                var latestPrice = priceHistories.FirstOrDefault();
                if (latestPrice == null)
                {
                    Console.WriteLine("No existing price history found for this BlindBox");
                    return Result<BlindBoxDto>.Failure(new ErrorResult
                    {
                        Code = "IncrementProbabilityAndPriceError",
                        Description = "No price history found for this BlindBox"
                    });
                }
                
                Console.WriteLine($"Current price: {latestPrice.Price}, Default price: {latestPrice.DefaultPrice}");
                Console.WriteLine($"Current probability in history: {latestPrice.Probability}, Default probability: {latestPrice.DefaultProbability}");
                
                // Calculate new probability (current + 0.1%)
                decimal newProbability = latestPrice.Probability + 0.1m;
                // Calculate new price (current + 10,000)
                decimal newPrice = latestPrice.Price + 10000;
                
                Console.WriteLine($"Incrementing probability from {latestPrice.Probability}% to {newProbability}%");
                Console.WriteLine($"Incrementing price from {latestPrice.Price} to {newPrice}");
                
                // Create a new price history with increased probability and price
                var blindBoxPriceHistory = new BlindBoxPriceHistory
                {
                    Id = Guid.NewGuid(),
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = latestPrice.DefaultPrice,
                    Price = newPrice,
                    DefaultProbability = latestPrice.DefaultProbability,
                    Probability = newProbability,
                    CreatedAt = DateTime.Now
                };

                Console.WriteLine($"Creating new price history with price {newPrice} and probability {newProbability}");
                
                var blindBoxPriceHistoryRepository = _repositoryManager.BlindBoxPriceHistory;
                blindBoxPriceHistoryRepository.Create(blindBoxPriceHistory);
                
                // Make sure to save changes immediately after creating the price history
                await blindBoxPriceHistoryRepository.SaveAsync();
                Console.WriteLine($"New price history record saved successfully with ID: {blindBoxPriceHistory.Id}");
                
                // Double check the database to verify the record was saved
                var verifyRecord = await blindBoxPriceHistoryRepository
                    .FindByCondition(ph => ph.Id == blindBoxPriceHistory.Id, false)
                    .FirstOrDefaultAsync();
                    
                if (verifyRecord != null)
                {
                    Console.WriteLine($"Verified: New price history record found in database with probability {verifyRecord.Probability}");
                }
                else
                {
                    Console.WriteLine($"WARNING: Could not verify the new price history record in the database");
                }
                
                // Return a DTO with updated values
                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);
                blindBoxDto.CurrentPrice = newPrice;
                blindBoxDto.Probability = (float)newProbability;
                
                return Result<BlindBoxDto>.Success(blindBoxDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IncrementProbabilityAndPriceAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Result<BlindBoxDto>.Failure(new ErrorResult
                {
                    Code = "IncrementProbabilityAndPriceError",
                    Description = ex.Message
                });
            }
        }
        
        // New method to reset probability and price when a secret item is found
        public async Task<Result<BlindBoxDto>> ResetProbabilityAndPriceAsync(Guid blindBoxId)
        {
            try
            {
                // Get the BlindBox for returning later
                var blindBox = await _blindBoxRepository.FindById(blindBoxId, false);
                if (blindBox == null)
                    return Result<BlindBoxDto>.Failure(BlindBoxErrors.GetBlindBoxNotFoundError(blindBoxId));
                
                // Get the default price and probability from price history
                var priceHistories = _repositoryManager.BlindBoxPriceHistory
                    .FindByCondition(ph => ph.BlindBoxId == blindBoxId, false)
                    .OrderByDescending(ph => ph.CreatedAt)
                    .ToList();
                
                Console.WriteLine($"Found {priceHistories.Count} price history entries for BlindBox {blindBoxId}");
                    
                var latestPrice = priceHistories.FirstOrDefault();
                
                if (latestPrice == null)
                {
                    Console.WriteLine("No price history found for this BlindBox, cannot reset");
                    return Result<BlindBoxDto>.Failure(new ErrorResult
                    {
                        Code = "ResetBlindBoxProbabilityAndPriceError",
                        Description = "No price history found for this BlindBox"
                    });
                }
                
                Console.WriteLine($"Current price: {latestPrice.Price}, Default price: {latestPrice.DefaultPrice}");
                Console.WriteLine($"Current probability in history: {latestPrice.Probability}, Default probability: {latestPrice.DefaultProbability}");
                
                Console.WriteLine($"Resetting probability from {latestPrice.Probability}% to default {latestPrice.DefaultProbability}%");
                Console.WriteLine($"Resetting price from {latestPrice.Price} to default {latestPrice.DefaultPrice}");
                
                // Create a new price history with reset price and probability
                var blindBoxPriceHistory = new BlindBoxPriceHistory
                {
                    Id = Guid.NewGuid(),
                    BlindBoxId = blindBox.Id,
                    DefaultPrice = latestPrice.DefaultPrice,
                    Price = latestPrice.DefaultPrice,
                    DefaultProbability = latestPrice.DefaultProbability,
                    Probability = latestPrice.DefaultProbability,
                    CreatedAt = DateTime.Now
                };

                Console.WriteLine($"Creating new price history with reset price {latestPrice.DefaultPrice} and probability {latestPrice.DefaultProbability}");
                
                var blindBoxPriceHistoryRepository = _repositoryManager.BlindBoxPriceHistory;
                blindBoxPriceHistoryRepository.Create(blindBoxPriceHistory);
                await blindBoxPriceHistoryRepository.SaveAsync();
                Console.WriteLine($"New reset price history record saved successfully with ID: {blindBoxPriceHistory.Id}");
                
                // Double check the database to verify the record was saved
                var verifyRecord = await blindBoxPriceHistoryRepository
                    .FindByCondition(ph => ph.Id == blindBoxPriceHistory.Id, false)
                    .FirstOrDefaultAsync();
                    
                if (verifyRecord != null)
                {
                    Console.WriteLine($"Verified: New reset price history record found in database with probability {verifyRecord.Probability}");
                }
                else
                {
                    Console.WriteLine($"WARNING: Could not verify the new reset price history record in the database");
                }
                
                // Return a DTO with updated values
                var blindBoxDto = _mapper.Map<BlindBoxDto>(blindBox);
                blindBoxDto.CurrentPrice = latestPrice.DefaultPrice;
                blindBoxDto.Probability = (float)latestPrice.DefaultProbability;
                
                return Result<BlindBoxDto>.Success(blindBoxDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ResetProbabilityAndPriceAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Result<BlindBoxDto>.Failure(new ErrorResult
                {
                    Code = "ResetProbabilityAndPriceError",
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
