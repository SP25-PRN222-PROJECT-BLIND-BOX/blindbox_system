using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class PackageService : BaseService, IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _packageRepository = repositoryManager.Package;
        }

        public async Task<Result<IEnumerable<PackageDto>>> GetAllPackagesAsync(bool trackChanges)
        {
            try
            {
                var packages = await _packageRepository.FindAll(trackChanges).ToListAsync();
                var packageDtos = _mapper.Map<IEnumerable<PackageDto>>(packages);
                
                return Result<IEnumerable<PackageDto>>.Success(packageDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<PackageDto>>.Failure(new ErrorResult
                {
                    Code = "GetPackagesError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<PackageDto>> GetPackageByIdAsync(Guid packageId, bool trackChanges)
        {
            try
            {
                var package = await _packageRepository.FindById(packageId, trackChanges);
                if (package == null)
                {
                    return Result<PackageDto>.Failure(new ErrorResult
                    {
                        Code = "PackageNotFound",
                        Description = $"Package with id: {packageId} doesn't exist."
                    });
                }

                var packageDto = _mapper.Map<PackageDto>(package);
                return Result<PackageDto>.Success(packageDto);
            }
            catch (Exception ex)
            {
                return Result<PackageDto>.Failure(new ErrorResult
                {
                    Code = "GetPackageError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<PackageDto>> UpdatePackageAsync(Guid id, PackageDto packageDto)
        {
            var package = await _packageRepository.FindById(id, true);
            if (package == null)
            {
                return Result<PackageDto>.Failure(new ErrorResult()
                {
                    Code = "PackageNotFound", 
                    Description = "Package not found"
                });
            }

            package.Name = packageDto.Name;
            package.TotalBlindBox = packageDto.TotalBlindBox;
            return null; 
        }
        

        public void Dispose()
        {
            _packageRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
