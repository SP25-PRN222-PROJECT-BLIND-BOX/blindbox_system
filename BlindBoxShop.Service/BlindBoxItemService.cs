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

                var itemDtos = _mapper.Map<IEnumerable<BlindBoxItemDto>>(items);
                return Result<IEnumerable<BlindBoxItemDto>>.Success(itemDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxItemDto>>.Failure(new ErrorResult
                {
                    Code = "GetBlindBoxItemsError",
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
