﻿using AutoMapper;
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

        public async Task<Result<IEnumerable<BlindBoxImageDto>>> CreateBlindBoxImagesAsync(IEnumerable<BlindBoxImageDto> blindBoxImageDtos)
        {
            try
            {
                if (blindBoxImageDtos == null || !blindBoxImageDtos.Any())
                {
                    return Result<IEnumerable<BlindBoxImageDto>>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxImage.Create.InvalidData",
                        Description = "Danh sách ảnh không hợp lệ hoặc rỗng."
                    });
                }

               
                var blindBoxImages = blindBoxImageDtos.Select(dto => new BlindBoxImage
                {
                    Id = Guid.NewGuid(),
                    BlindBoxId = dto.BlindBoxId,
                    ImageUrl = dto.ImageUrl,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                
                await _blindBoxImageRepository.CreateRangeAsync(blindBoxImages);
                await _blindBoxImageRepository.SaveAsync();

                
                var resultDtos = _mapper.Map<IEnumerable<BlindBoxImageDto>>(blindBoxImages);
                return Result<IEnumerable<BlindBoxImageDto>>.Success(resultDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BlindBoxImageDto>>.Failure(new ErrorResult
                {
                    Code = "BlindBoxImage.Create.Failed",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> DeleteBlindBoxImageByIdAsync(Guid blindBoxImageId)
        {
            try
            {
                var blindBoxImage = await _blindBoxImageRepository.FindById(blindBoxImageId, true);
                if (blindBoxImage == null)
                {
                    return Result<bool>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxImage.Delete.NotFound",
                        Description = "Không tìm thấy hình ảnh trong blind box."
                    });
                }

                _blindBoxImageRepository.Delete(blindBoxImage);
                await _blindBoxImageRepository.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "BlindBoxImage.Delete.Failed",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> DeleteBlindBoxImagesByBlindBoxIdAsync(Guid blindBoxId)
        {
            try
            {
                var blindBoxImages = await _blindBoxImageRepository
                    .FindByCondition(bi => bi.BlindBoxId.Equals(blindBoxId), true)
                    .ToListAsync();

                if (!blindBoxImages.Any())
                {
                    return Result<bool>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxImage.Delete.NotFound",
                        Description = "Không có hình ảnh nào trong blind box để xóa."
                    });
                }

                await _blindBoxImageRepository.DeleteRangeAsync(blindBoxImages);
                await _blindBoxImageRepository.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "BlindBoxImage.Delete.Failed",
                    Description = ex.Message
                });
            }
        }
        public async Task<Result<BlindBoxImageDto>> CreateBlindBoxImageAsync(BlindBoxImageDto blindBoxImageDto)
        {
            try
            {
                if (blindBoxImageDto == null)
                {
                    return Result<BlindBoxImageDto>.Failure(new ErrorResult
                    {
                        Code = "BlindBoxImage.Create.InvalidData",
                        Description = "Dữ liệu ảnh không hợp lệ."
                    });
                }


                var blindBoxImage = new BlindBoxImage
                {
                    Id = Guid.NewGuid(),
                    BlindBoxId = blindBoxImageDto.BlindBoxId,
                    ImageUrl = blindBoxImageDto.ImageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                
                await _blindBoxImageRepository.CreateAsync(blindBoxImage);
                await _blindBoxImageRepository.SaveAsync();

                
                var resultDto = _mapper.Map<BlindBoxImageDto>(blindBoxImage);
                return Result<BlindBoxImageDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<BlindBoxImageDto>.Failure(new ErrorResult
                {
                    Code = "BlindBoxImage.Create.Failed",
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
