using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlindBoxShop.Service
{
    public class ReplyService : BaseService, IReplyService
    {
        private readonly IReplyRepository _replyReviewsRepository;

        public ReplyService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _replyReviewsRepository = repositoryManager.ReplyReviews;
        }

        private async Task<Result<ReplyReviews>> GetAndCheckIfReplyExistsByIdAsync(Guid id, bool trackChanges)
        {
            var reply = await _replyReviewsRepository.FindById(id, trackChanges);
            if (reply is null)
                return Result<ReplyReviews>.Failure(ReplyErrors.GetReplyNotFoundError(id));

            return Result<ReplyReviews>.Success(reply);
        }

        public async Task<Result<IEnumerable<ReplyDto>>> GetRepliesAsync(ReplyParameter replyParameter, bool trackChanges)
        {
            var replies = await _replyReviewsRepository.GetRepliesAsync(replyParameter, trackChanges);

            var repliesDto = _mapper.Map<IEnumerable<ReplyDto>>(replies);

            return (repliesDto, replies.MetaData);
        }

        public async Task<Result<ReplyDto>> CreateReplyAsync(ReplyForCreationDto replyForCreateDto)
        {
            var validationResult = await ValidateReplyCreationAsync(replyForCreateDto);
            if (!validationResult.IsSuccess)
                return Result<ReplyDto>.Failure(validationResult.Errors);

            var replyEntity = _mapper.Map<ReplyReviews>(replyForCreateDto);
            await _replyReviewsRepository.CreateAsync(replyEntity);
            await _replyReviewsRepository.SaveAsync();

            var replyDto = _mapper.Map<ReplyDto>(replyEntity);
            return Result<ReplyDto>.Success(replyDto);
        }

        private async Task<Result> ValidateReplyCreationAsync(ReplyForCreationDto replyForCreateDto)
        {
            var existingReply = await _replyReviewsRepository.FindAsync(r =>
                r.CustomerReviewsId == replyForCreateDto.CustomerReviewsId &&
                r.UserId == replyForCreateDto.UserId);

            if (existingReply != null)
            {
                return Result.Failure(new ErrorResult
                {
                    Code = "DuplicateReply",
                    Description = "You have already replied to this review."
                });
            }
            return Result.Success();
        }

        public async Task<Result> DeleteReplyAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfReplyExistsByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var replyEntity = checkIfExistResult.GetValue<ReplyReviews>();

            _replyReviewsRepository.Delete(replyEntity);
            await _replyReviewsRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ReplyDto>>> GetRepliesByReviewIdAsync(Guid reviewId, ReplyParameter replyReviewParameter, bool trackChanges)
        {
            var replies = await _replyReviewsRepository.GetRepliesByReviewIdAsync(reviewId, replyReviewParameter, trackChanges);

            if (!replies.Any())
                return Result<IEnumerable<ReplyDto>>.Failure(ReplyErrors.GetNoRepliesFoundForReviewError(reviewId));

            var repliesDto = _mapper.Map<IEnumerable<ReplyDto>>(replies);

            return (repliesDto, replies.MetaData);
        }

        public async Task<Result> UpdateReplyAsync(Guid id, ReplyForUpdateDto replyForUpdateDto)
        {
            var checkIfExistResult = await GetAndCheckIfReplyExistsByIdAsync(id, true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var replyEntity = checkIfExistResult.GetValue<ReplyReviews>();

            _mapper.Map(replyForUpdateDto, replyEntity);

            await _replyReviewsRepository.SaveAsync();

            return Result.Success();
        }

        public void Dispose()
        {
            _replyReviewsRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
