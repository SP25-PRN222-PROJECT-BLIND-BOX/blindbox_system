using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.CustomerReview;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Service
{
    public class CustomerReviewService : BaseService, ICustomerReviewsService
    {
        private readonly ICustomerReviewRepository _customerReviewsRepository;
        private readonly IBlindBoxService _blindBoxService;
        public CustomerReviewService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _customerReviewsRepository = repositoryManager.CustomerReviews;
        }

        private async Task<Result<CustomerReviews>> GetAndCheckIfReviewExistByIdAsync(Guid id, bool trackChanges)
        {
            var review = await _customerReviewsRepository.FindById(id, trackChanges);
            if (review is null)
                return Result<CustomerReviews>.Failure(ReviewErrors.GetReviewNotFoundError(id));

            return Result<CustomerReviews>.Success(review);
        }

        public async Task<Result<ReviewDto>> CreateReviewAsync(ReviewForCreationDto reviewForCreateDto)
        {
            try
            {
                var validationResult = await ValidateReviewCreationAsync(reviewForCreateDto);
                if (!validationResult.IsSuccess)
                    return Result<ReviewDto>.Failure(validationResult.Errors);
                var reviewEntity = _mapper.Map<CustomerReviews>(reviewForCreateDto);
                await _customerReviewsRepository.CreateAsync(reviewEntity);
                await _customerReviewsRepository.SaveAsync();

                var reviewDto = _mapper.Map<ReviewDto>(reviewEntity);
                return Result<ReviewDto>.Success(reviewDto);
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                Console.WriteLine($"Error in CreateReviewAsync: {ex.Message}");
                return Result<ReviewDto>.Failure(new ErrorResult
                {
                    Code = "InternalError",
                    Description = "An unexpected error occurred."
                });
            }
        }

        private async Task<Result> ValidateReviewCreationAsync(ReviewForCreationDto reviewForCreateDto)
        {
            /*var blindBox = await _blindBoxService.getBlindBoxAsynce(reviewForCreateDto.BlindBoxId);
            if (blindBox == null)
            {
                return Result.Failure(new ErrorResult
                {
                    Code = "BlindBoxNotFound",
                    Description = "The specified BlindBox does not exist."
                });
            }*/

            var existingReview = await _customerReviewsRepository.FindAsync(r =>
                r.BlindBoxId == reviewForCreateDto.BlindBoxId && r.UserId == reviewForCreateDto.UserId);

            if (existingReview != null)
            {
                return Result.Failure(new ErrorResult
                {
                    Code = "DuplicateReview",
                    Description = "You have already reviewed this BlindBox."
                });
            }

            return Result.Success();
        }

        public async Task<Result> DeleteReviewAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfReviewExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var reviewEntity = checkIfExistResult.GetValue<CustomerReviews>();

            _customerReviewsRepository.Delete(reviewEntity);
            await _customerReviewsRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(Guid userId, CustomerReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _customerReviewsRepository.GetReviewsByUserIdAsync(userId, customerReviewParameter, trackChanges);

            if (!reviews.Any())
                return Result<IEnumerable<ReviewDto>>.Failure(ReviewErrors.GetNoReviewsFoundForUserError(userId));

            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return (reviewsDto, reviews.MetaData);
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, CustomerReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _customerReviewsRepository.GetReviewsByBlindBoxIdAsync(blindBoxId, customerReviewParameter, trackChanges);

            if (!reviews.Any())
                return Result<IEnumerable<ReviewDto>>.Failure(ReviewErrors.GetNoReviewsFoundForBlindBoxError(blindBoxId));

            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return (reviewsDto, reviews.MetaData);
        }


        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsAsync(CustomerReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _customerReviewsRepository.GetReviewsAsync(customerReviewParameter, trackChanges);

            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return (reviewsDto, reviews.MetaData);
        }

        public async Task<Result> UpdateReviewAsync(Guid id, ReviewForUpdateDto reviewForUpdateDto)
        {
            var checkIfExistResult = await GetAndCheckIfReviewExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var reviewEntity = checkIfExistResult.GetValue<CustomerReviews>();

            if (reviewEntity.CreatedAt.AddDays(30) < DateTime.UtcNow)
            {
                return Result.Failure(ReviewErrors.GetReviewUpdateNotAllowedAfter30DaysError());
            }

            if (reviewEntity.UpdatedAt != null)
            {
                return Result.Failure(ReviewErrors.GetReviewUpdateNotAllowedAfterUpdatedError());
            }

            _mapper.Map(reviewForUpdateDto, reviewEntity);

            await _customerReviewsRepository.SaveAsync();

            return Result.Success();
        }

        public void Dispose()
        {
            _customerReviewsRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}