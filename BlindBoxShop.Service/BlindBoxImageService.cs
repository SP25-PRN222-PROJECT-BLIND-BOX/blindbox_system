using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Service
{
    public class BlindBoxImageService : BaseService, IBlindBoxImageService
    {
        private readonly IBlindBoxImageRepository _blindBoxImageRepository;

        public BlindBoxImageService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxImageRepository = repositoryManager.BlindBoxImage;
        }

        public async Task<Result<IEnumerable<BlindBoxImageDto>>> GetBlindBoxImagesByBlindBoxIdAsync(Guid blindBoxId)
        {
            try
            {
                var blindBoxImages = await _blindBoxImageRepository
                    .FindByCondition(bi => bi.BlindBoxId.Equals(blindBoxId), false)
                    .OrderBy(bi => bi.CreatedAt)
                    .ToListAsync();
                
                var blindBoxImagesDto = _mapper.Map<IEnumerable<BlindBoxImageDto>>(blindBoxImages);
                
                return Result<IEnumerable<BlindBoxImageDto>>.Success(blindBoxImagesDto);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxImageDto>>.Failure(
                    new ErrorResult { 
                        Code = "BlindBoxImage.GetByBlindBoxId.Failed", 
                        Description = ex.Message 
                    });
            }
        }

        public void Dispose()
        {
            _blindBoxImageRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
