using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
