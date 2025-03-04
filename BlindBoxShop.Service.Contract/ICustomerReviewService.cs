using BlindBoxShop.Shared.DataTransferObject.CustomerReview;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface ICustomerReviewsService : IDisposable
    {
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsAsync(CustomerReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsByUserIdAsync(Guid userId, CustomerReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<IEnumerable<ReviewDto>>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, CustomerReviewParameter customerReviewParameter, bool trackChanges);
        Task<Result<ReviewDto>> CreateReviewAsync(ReviewForCreationDto reviewForCreateDto);
        Task<Result> UpdateReviewAsync(Guid id, ReviewForUpdateDto reviewForUpdateDto);
        Task<Result> DeleteReviewAsync(Guid id);
    }
}
