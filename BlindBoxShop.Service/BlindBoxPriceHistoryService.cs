﻿using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Service
{
    public class BlindBoxPriceHistoryService : BaseService, IBlindBoxPriceHistoryService
    {
        private readonly IBlindBoxPriceHistoryRepository _blindBoxPriceHistoryRepository;

        public BlindBoxPriceHistoryService(IRepositoryManager repositoryManager, IMapper mapper)
            : base(repositoryManager, mapper)
        {
            _blindBoxPriceHistoryRepository = repositoryManager.BlindBoxPriceHistory;
        }

        // Get by ID
        public async Task<Result<BlindBoxPriceHistoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var blindBoxPriceHistory = await _blindBoxPriceHistoryRepository.FindByIdAsync(id, false);
                if (blindBoxPriceHistory == null)
                {
                    return Result<BlindBoxPriceHistoryDto>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxPriceHistory.GetById.NotFound",
                        Description = "Không tìm thấy lịch sử giá."
                    });
                }

                var blindBoxPriceHistoryDto = _mapper.Map<BlindBoxPriceHistoryDto>(blindBoxPriceHistory);
                return Result<BlindBoxPriceHistoryDto>.Success(blindBoxPriceHistoryDto);
            }
            catch (Exception ex)
            {
                return Result<BlindBoxPriceHistoryDto>.Failure(new ErrorResult
                {
                    Code = "BlindBoxPriceHistory.GetById.Failed",
                    Description = ex.Message
                });
            }
        }

        // Get by BlindBoxId
        public async Task<Result<IEnumerable<BlindBoxPriceHistoryDto>>> GetByBlindBoxIdAsync(Guid blindBoxId)
        {

            try
            {
                // Await để lấy IQueryable<BlindBoxPriceHistory>
                var query = await _blindBoxPriceHistoryRepository
                    .FindByConditionAsync(bph => bph.BlindBoxId.Equals(blindBoxId), false);

                // Bây giờ có thể gọi OrderBy và ToListAsync trên IQueryable
                var blindBoxPriceHistories = await query
                    .OrderBy(bph => bph.CreatedAt)
                    .ToListAsync();

                var blindBoxPriceHistoriesDto = _mapper.Map<IEnumerable<BlindBoxPriceHistoryDto>>(blindBoxPriceHistories);
                return Result<IEnumerable<BlindBoxPriceHistoryDto>>.Success(blindBoxPriceHistoriesDto);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxPriceHistoryDto>>.Failure(new ErrorResult
                {
                    Code = "BlindBoxPriceHistory.GetByBlindBoxId.Failed",
                    Description = ex.Message
                });
            }
        }


        public void Dispose()
        {
            _blindBoxPriceHistoryRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
