﻿using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.DataTransferObject.Review;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service
{
    public class ReviewService : BaseService, IReviewsService
    {
        private readonly IReviewRepository _reviewsRepository;
        private readonly IReplyRepository _replyRepository;
        public ReviewService(IRepositoryManager repositoryManager, IMapper mapper, IReplyRepository replyRepository) : base(repositoryManager, mapper)
        {
            _reviewsRepository = repositoryManager.Review;
            _replyRepository = replyRepository;
        }

        public async Task<Result<IEnumerable<ReviewWithReplyDto>>> GetReviewsWithReplyAsync(ReviewParameter customerReviewParameter, bool trackChanges)
        {
            try
            {
                // Lấy danh sách các review từ repository
                var reviews = await _reviewsRepository.GetReviewsAsync(customerReviewParameter, trackChanges);

                if (!reviews.Any())
                    return Result<IEnumerable<ReviewWithReplyDto>>.Failure(ReviewErrors.GetReviewsNotFoundError());

                // Duyệt qua từng review để lấy danh sách các reply tương ứng
                var reviewWithReplies = new List<ReviewWithReplyDto>();

                foreach (var review in reviews)
                {
                    var replies = await _replyRepository.GetReplyByReviewIdAsync(review.Id, trackChanges);

                    var reviewWithReplyDto = new ReviewWithReplyDto
                    {
                        Review = _mapper.Map<ReviewDto>(review),
                        Reply = _mapper.Map<ReplyDto>(replies)
                    };

                    reviewWithReplies.Add(reviewWithReplyDto);
                }

                return (reviewWithReplies, reviews.MetaData);
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                Console.WriteLine($"Error in GetReviewsWithReplyAsync: {ex.Message}");
                return Result<IEnumerable<ReviewWithReplyDto>>.Failure(new ErrorResult
                {
                    Code = "InternalError",
                    Description = "An unexpected error occurred while fetching reviews with replies."
                });
            }
        }

        private async Task<Result<CustomerReviews>> GetAndCheckIfReviewExistByIdAsync(Guid id, bool trackChanges)
        {
            var review = await _reviewsRepository.FindById(id, trackChanges);
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
                await _reviewsRepository.CreateAsync(reviewEntity);
                await _reviewsRepository.SaveAsync();

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

            var existingReview = await _reviewsRepository.FindAsync(r =>
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

            _reviewsRepository.Delete(reviewEntity);
            await _reviewsRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(Guid userId, ReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _reviewsRepository.GetReviewsByUserIdAsync(userId, customerReviewParameter, trackChanges);

            if (!reviews.Any())
                return Result<IEnumerable<ReviewDto>>.Failure(ReviewErrors.GetNoReviewsFoundForUserError(userId));

            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return (reviewsDto, reviews.MetaData);
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, ReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _reviewsRepository.GetReviewsByBlindBoxIdAsync(blindBoxId, customerReviewParameter, trackChanges);

            if (!reviews.Any())
                return Result<IEnumerable<ReviewDto>>.Failure(ReviewErrors.GetNoReviewsFoundForBlindBoxError(blindBoxId));

            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return (reviewsDto, reviews.MetaData);
        }


        public async Task<Result<IEnumerable<ReviewDto>>> GetReviewsAsync(ReviewParameter customerReviewParameter, bool trackChanges)
        {
            var reviews = await _reviewsRepository.GetReviewsAsync(customerReviewParameter, trackChanges);

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

            await _reviewsRepository.SaveAsync();

            return Result.Success();
        }

        public void Dispose()
        {
            _reviewsRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}