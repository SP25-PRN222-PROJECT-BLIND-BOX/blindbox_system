using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class ReviewErrors
    {
        #region Errors Message
        public const string ReviewNotFound = "Review with id {0} not found.";
        public const string ReviewsNotFound = "Reviews not found.";
        public const string ReviewExist = "Review with name {0} already exist.";
        public const string ReviewUpdateNotAllowedAfter30Days = "Cannot update a review after 30 days from its creation date.";
        public const string ReviewUpdateNotAllowedAfterUpdated = "Cannot update a review that has already been updated.";
        public const string NoReviewsFoundForUser = "No reviews found for user with id {0}.";
        public const string NoReviewsFoundForBlindBox = "No reviews found for blind box with id {0}.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetReviewNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(ReviewNotFound),
            Description = string.Format(ReviewNotFound, id)
        };

        public static ErrorResult GetReviewsNotFoundError()
        => new ErrorResult()
        {
            Code = nameof(ReviewNotFound),
            Description = string.Format(ReviewNotFound)
        };

        public static ErrorResult GetReviewExistError(string name)
        => new ErrorResult()
        {
            Code = nameof(ReviewExist),
            Description = string.Format(ReviewExist, name)
        };

        public static ErrorResult GetReviewUpdateNotAllowedAfter30DaysError()
        => new ErrorResult()
        {
            Code = nameof(ReviewUpdateNotAllowedAfter30Days),
            Description = ReviewUpdateNotAllowedAfter30Days
        };

        public static ErrorResult GetReviewUpdateNotAllowedAfterUpdatedError()
        => new ErrorResult()
        {
            Code = nameof(ReviewUpdateNotAllowedAfterUpdated),
            Description = ReviewUpdateNotAllowedAfterUpdated
        };

        public static ErrorResult GetNoReviewsFoundForUserError(Guid userId)
        => new ErrorResult()
        {
            Code = nameof(NoReviewsFoundForUser),
            Description = string.Format(NoReviewsFoundForUser, userId)
        };

        public static ErrorResult GetNoReviewsFoundForBlindBoxError(Guid blindBoxId)
        => new ErrorResult()
        {
            Code = nameof(NoReviewsFoundForBlindBox),
            Description = string.Format(NoReviewsFoundForBlindBox, blindBoxId)
        };
        #endregion
    }
}
