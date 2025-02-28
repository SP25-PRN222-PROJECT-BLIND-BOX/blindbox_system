using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IReplyService : IDisposable
    {
        Task<Result<IEnumerable<ReplyDto>>> GetRepliesAsync(ReplyParameter replyParameter, bool trackChanges);
        Task<Result<IEnumerable<ReplyDto>>> GetRepliesByReviewIdAsync(Guid reviewId, ReplyParameter replyReviewParameter, bool trackChanges);
        Task<Result<ReplyDto>> CreateReplyAsync(ReplyForCreationDto replyForCreationDto);
        Task<Result> UpdateReplyAsync(Guid id, ReplyForUpdateDto replyForUpdateDto);
        Task<Result> DeleteReplyAsync(Guid id);
    }
}
