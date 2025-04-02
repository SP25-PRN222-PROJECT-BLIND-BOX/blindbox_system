using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;

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

        public void Dispose()
        {
            _packageRepository.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result<IEnumerable<PackageManageDto>>> GetAllPackagesAsync(PackageParameter parameter, bool trackChanges)
        {
            try
            {
                var packages = _repositoryManager.Package.FindAll(trackChanges);

                // Apply search by name if specified
                if (!string.IsNullOrWhiteSpace(parameter.SearchByName))
                {
                    packages = packages.Where(bb =>
                        EF.Functions.Like(bb.Name.ToLower(), $"%{parameter.SearchByName.ToLower()}%"));
                }

                // Apply type filter if specified
                if (parameter.Type.HasValue)
                {
                    packages = packages.Where(bb => (int)bb.Type == parameter.Type);
                }

                // Apply sorting by creation date
                packages = packages.OrderByDescending(e => e.CreatedAt);

                // Apply pagination
                packages = packages
                    .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                    .Take(parameter.PageSize);

                var packageList = await packages.ToListAsync();
                var packagesDtos = _mapper.Map<IEnumerable<PackageManageDto>>(packageList);

                return Result<IEnumerable<PackageManageDto>>.Success(packagesDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<PackageManageDto>>.Failure(new ErrorResult
                {
                    Code = "GetPackageError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<PackageManageDto>> CreatePackageAsync(PackageForCreate package)
        {
            try
            {
                var result = _mapper.Map<Package>(package);
                _packageRepository.Create(result);
                await _packageRepository.SaveAsync();

                var packageDto = _mapper.Map<PackageManageDto>(result);

                return Result<PackageManageDto>.Success(packageDto);
            }
            catch (Exception ex)
            {
                return Result<PackageManageDto>.Failure(new ErrorResult
                {
                    Code = "CreatePackageError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> DeletePackageAsync(Guid packageId, bool trackChanges)
        {
            try
            {
                var package = await _packageRepository.FindById(packageId, trackChanges);
                if (package == null)
                    return Result<bool>.Failure(PackageError.GetPackageNotFoundError(packageId));

                _packageRepository.Delete(package);
                await _packageRepository.SaveAsync();

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

        public async Task<Result<PackageManageDto>> UpdatePackageAsync(Guid packageId, PackageForUpdate packageForUpdate, bool trackChanges)
        {
            try
            {
                var package = await _packageRepository.FindById(packageId, trackChanges);
                if (package == null)
                    return Result<PackageManageDto>.Failure(PackageError.GetPackageNotFoundError(packageId));

                _mapper.Map(packageForUpdate, package);
                package.UpdatedAt = DateTime.UtcNow;

                _packageRepository.Update(package);
                await _packageRepository.SaveAsync();

                var packageDto = _mapper.Map<PackageManageDto>(package);

                // Create a new price history entry with the updated price

                return Result<PackageManageDto>.Success(packageDto);
            }
            catch (Exception ex)
            {
                return Result<PackageManageDto>.Failure(new ErrorResult
                {
                    Code = "UpdatepPackageError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<PackageManageDto>> GetPackageById(Guid packageId, bool trackChanges)
        {
            try
            {
                var package = await _packageRepository.FindById(packageId, trackChanges);
                if (package == null)
                {
                    return Result<PackageManageDto>.Failure(new ErrorResult
                    {
                        Code = "PackageNotFound",
                        Description = $"Package with id: {packageId} doesn't exist."
                    });
                }

                var packageDto = _mapper.Map<PackageManageDto>(package);
                return Result<PackageManageDto>.Success(packageDto);
            }
            catch (Exception ex)
            {
                return Result<PackageManageDto>.Failure(new ErrorResult
                {
                    Code = "GetPackageError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<bool>> IsPackageOpenAsync(Guid packageId, bool trackChanges)
        {
            try
            {
                var package = await _packageRepository.FindById(packageId, trackChanges);
                if (package == null)
                    return Result<bool>.Failure(PackageError.GetPackageNotFoundError(packageId));

                // Check if the package is open
                var isOpen = package.CurrentTotalBlindBox < package.TotalBlindBox;
                return Result<bool>.Success(isOpen);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "CheckPackageOpenError",
                    Description = ex.Message
                });
            }
        }

        public async Task<int> GetTotalCountAsync(PackageParameter parameter, bool trackChanges)
        {
            var query = _repositoryManager.Package.FindAll(trackChanges);

            // Apply search by name if specified
            if (!string.IsNullOrWhiteSpace(parameter.SearchByName))
            {
                query = query.Where(bb =>
                    EF.Functions.Like(bb.Name.ToLower(), $"%{parameter.SearchByName.ToLower()}%"));
            }

            // Apply type filter if specified
            if (parameter.Type.HasValue)
            {
                query = query.Where(bb => (int)bb.Type == parameter.Type);
            }

            return await query.CountAsync();
        }
    }
}
