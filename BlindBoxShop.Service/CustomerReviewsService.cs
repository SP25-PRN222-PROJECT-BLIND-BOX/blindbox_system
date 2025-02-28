using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.CustomerReview;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service
{
    public class CustomerReviewsService : BaseService, ICustomerReviewsService
    {
        private readonly ICustomerReviewsRepository _customerReviewsRepository;
        public CustomerReviewsService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
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
            var reviewEntity = _mapper.Map<CustomerReviews>(reviewForCreateDto);
            await _customerReviewsRepository.CreateAsync(reviewEntity);
            await _customerReviewsRepository.SaveAsync();

            var reviewDto = _mapper.Map<ReviewDto>(reviewEntity);

            return Result<ReviewDto>.Success(reviewDto);
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
            // Kiểm tra xem review có tồn tại không
            var checkIfExistResult = await GetAndCheckIfReviewExistByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var reviewEntity = checkIfExistResult.GetValue<CustomerReviews>();

            // Kiểm tra điều kiện không cho phép cập nhật
            if (reviewEntity.CreatedAt.AddDays(30) < DateTime.UtcNow)
            {
                return Result.Failure(ReviewErrors.GetReviewUpdateNotAllowedAfter30DaysError());
            }

            if (reviewEntity.UpdatedAt != null)
            {
                return Result.Failure(ReviewErrors.GetReviewUpdateNotAllowedAfterUpdatedError());
            }

            // Ánh xạ dữ liệu từ DTO sang entity
            _mapper.Map(reviewForUpdateDto, reviewEntity);

            // Lưu thay đổi
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