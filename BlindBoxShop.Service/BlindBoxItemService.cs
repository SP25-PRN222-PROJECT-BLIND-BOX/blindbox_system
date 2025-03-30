using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class BlindBoxItemService : BaseService, IBlindBoxItemService
    {
        private readonly IBlindBoxItemRepository _blindBoxItemRepository;

        public BlindBoxItemService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxItemRepository = repositoryManager.BlindBoxItem;
        }

        /// <summary>
        /// Gets all items for a specific BlindBox by its ID
        /// </summary>
        /// <param name="blindBoxId">The ID of the BlindBox</param>
        /// <param name="trackChanges">Whether to track changes</param>
        /// <returns>A collection of BlindBoxItemDto objects</returns>
        public async Task<Result<IEnumerable<BlindBoxItemDto>>> GetItemsByBlindBoxIdAsync(Guid blindBoxId, bool trackChanges)
        {
            try
            {
                var items = await _blindBoxItemRepository
                    .FindByCondition(item => item.BlindBoxId == blindBoxId, trackChanges)
                    .ToListAsync();

                // Manual mapping instead of using AutoMapper
                var itemDtos = items.Select(item => new BlindBoxItemDto
                {
                    Id = item.Id,
                    BlindBoxId = item.BlindBoxId,
                    Name = item.Name,
                    Description = item.Description,
                    Rarity = (int)item.Rarity,
                    ImageUrl = item.ImageUrl,
                    IsSecret = item.IsSecret,
                    CreatedAt = item.CreatedAt
                }).ToList();

                return Result<IEnumerable<BlindBoxItemDto>>.Success(itemDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetItemsByBlindBoxIdAsync: {ex.Message}");
                return Result<IEnumerable<BlindBoxItemDto>>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxItemsError",
                    Description = ex.Message
                });
            }
        }
        
        /// <summary>
        /// Gets a specific BlindBoxItem by its ID
        /// </summary>
        /// <param name="blindBoxItemId">The ID of the BlindBoxItem</param>
        /// <param name="trackChanges">Whether to track changes</param>
        /// <returns>A BlindBoxItemDto object</returns>
        public async Task<Result<BlindBoxItemDto>> GetBlindBoxItemByIdAsync(Guid blindBoxItemId, bool trackChanges)
        {
            try
            {
                var item = await _blindBoxItemRepository
                    .FindByCondition(item => item.Id == blindBoxItemId, trackChanges)
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return Result<BlindBoxItemDto>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxItemNotFound",
                        Description = $"BlindBoxItem with ID {blindBoxItemId} not found"
                    });
                }

                // Manual mapping instead of using AutoMapper
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

                return Result<BlindBoxItemDto>.Success(itemDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBlindBoxItemByIdAsync: {ex.Message}");
                return Result<BlindBoxItemDto>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxItemError",
                    Description = ex.Message
                });
            }
        }

        public void Dispose()
        {
            _blindBoxItemRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
