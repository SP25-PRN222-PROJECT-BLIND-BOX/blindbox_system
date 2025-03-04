using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class ReplyErrors
    {
        #region Errors Message
        public const string ReplyNotFound = "Reply with id {0} not found.";
        public const string ReplyExist = "Reply with content '{0}' already exists.";
        public const string ReplyUpdateNotAllowed = "Cannot update a reply that has already been updated.";
        public const string NoRepliesFoundForReview = "No replies found for review with id {0}.";
        public const string NoRepliesFoundForUser = "No replies found for user with id {0}.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetReplyNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(ReplyNotFound),
            Description = string.Format(ReplyNotFound, id)
        };

        public static ErrorResult GetReplyExistError(string content)
        => new ErrorResult()
        {
            Code = nameof(ReplyExist),
            Description = string.Format(ReplyExist, content)
        };
        public static ErrorResult GetNoRepliesFoundForReviewError(Guid reviewId)
        => new ErrorResult()
        {
            Code = nameof(NoRepliesFoundForReview),
            Description = string.Format(NoRepliesFoundForReview, reviewId)
        };
        #endregion
    }
}
