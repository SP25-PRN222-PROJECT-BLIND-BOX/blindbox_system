using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges)
        {
            return await FindByCondition(od => od.OrderId == orderId, trackChanges)
                .Include(od => od.Order)
                .Include(od => od.BlindBoxPriceHistory!.BlindBox)
                .ToListAsync();
        }
    }
}
