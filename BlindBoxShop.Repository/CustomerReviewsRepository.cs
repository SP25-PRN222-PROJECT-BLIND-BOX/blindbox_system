using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class CustomerReviewsRepository : RepositoryBase<CustomerReviews>, ICustomerReviewsRepository
    {
        public CustomerReviewsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
