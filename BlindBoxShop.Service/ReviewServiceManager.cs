using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository;
using BlindBoxShop.Service.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class ReviewServiceManager : IReviewServiceManager
    {
        private readonly RepositoryContext _dbContext;

        public ReviewServiceManager(RepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CustomerReviews>> GetAllCustomerReviewsAsync()
        {
            return await _dbContext.CustomerReviews
                .Include(cr => cr.User)
                .Include(cr => cr.BlindBox)
                .Include(cr => cr.ReplyReviews)
                .ToListAsync();
        }

        public async Task AddReplyReviewAsync(Guid customerReviewId, Guid userId, string reply)
        {
            var replyReview = new ReplyReviews
            {
                Id = Guid.NewGuid(),
                CustomerReviewsId = customerReviewId,
                UserId = userId,
                Reply = reply,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.ReplyReviews.Add(replyReview);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteReplyReviewAsync(Guid customerReviewId)
        {
            var replyReview = await _dbContext.ReplyReviews
                .FirstOrDefaultAsync(rr => rr.CustomerReviewsId == customerReviewId);

            if (replyReview != null)
            {
                _dbContext.ReplyReviews.Remove(replyReview);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
