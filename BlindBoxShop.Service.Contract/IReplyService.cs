using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IReplyService : IDisposable
    {
        Task<Result<IEnumerable<ReplyDto>>> GetRepliesAsync(ReplyParameter replyParameter, bool trackChanges);
        Task<Result<ReplyDto>> GetReplyByReviewIdAsync(Guid reviewId, bool trackChanges);
        Task<Result<ReplyDto>> CreateReplyAsync(ReplyForCreationDto replyForCreationDto);
        Task<Result> UpdateReplyAsync(Guid id, ReplyForUpdateDto replyForUpdateDto);
        Task<Result> DeleteReplyAsync(Guid id);
    }
}
