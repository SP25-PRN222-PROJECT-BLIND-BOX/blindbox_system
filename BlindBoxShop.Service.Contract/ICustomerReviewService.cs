using BlindBoxShop.Shared.DataTransferObject.Review;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IReviewsService : IDisposable
    {
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsAsync(ReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<IEnumerable<ReviewWithReplyDto>>> GetReviewsWithReplyAsync(ReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(Guid userId, ReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, ReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<ReviewDto>> CreateReviewAsync(ReviewForCreationDto reviewForCreateDto);
        Task<Result> UpdateReviewAsync(Guid id, ReviewForUpdateDto reviewForUpdateDto);
        Task<Result> DeleteReviewAsync(Guid id);
    }
}
