using BlindBoxShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IReviewServiceManager
    {
        Task<List<CustomerReviews>> GetAllCustomerReviewsAsync();
        Task AddReplyReviewAsync(Guid customerReviewId, Guid userId, string reply);
        Task DeleteReplyReviewAsync(Guid customerReviewId);
    }
}
