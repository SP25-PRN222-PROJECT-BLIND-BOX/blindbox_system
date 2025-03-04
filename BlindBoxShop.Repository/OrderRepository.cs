using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        // Find an order by its ID
        public async Task<Order?> FindAsync(Expression<Func<Order, bool>> predicate)
        {
            return await FindByCondition(predicate, trackChanges: false)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();
        }

        // Get paged orders with filtering and sorting
        public async Task<PagedList<Order>> GetOrdersAsync(OrderParameter orderParameter, bool trackChanges)
        {
            var orders = await FindAll(trackChanges)
                .Include(o => o.User)
                .SearchById(orderParameter.SearchById)
                .SearchByDate(orderParameter.SearchByDate)
                .SearchByStatus(orderParameter.SearchByStatus)
                .Sort(orderParameter.OrderBy)
                .Skip((orderParameter.PageNumber - 1) * orderParameter.PageSize)
                .Take(orderParameter.PageSize)
                .ToListAsync();

            var count = await FindAll(trackChanges)
                .SearchById(orderParameter.SearchById)
                .SearchByDate(orderParameter.SearchByDate)
                .CountAsync();

            return new PagedList<Order>(
                orders,
                count,
                orderParameter.PageNumber,
                orderParameter.PageSize);
        }

        // Get orders for a specific user with pagination and sorting
        public async Task<PagedList<Order>> GetOrdersByUserIdAsync(Guid userId, OrderParameter orderParameter, bool trackChanges)
        {
            var query = FindByCondition(o => o.UserId == userId, trackChanges)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .SearchById(orderParameter.SearchById)
                .SearchByDate(orderParameter.SearchByDate)
                .SearchByStatus(orderParameter.SearchByStatus)
                .Sort(orderParameter.OrderBy);

            var count = await query.CountAsync();

            var orders = await query
                .Skip((orderParameter.PageNumber - 1) * orderParameter.PageSize)
                .Take(orderParameter.PageSize)
                .ToListAsync();

            return new PagedList<Order>(
                orders,
                count,
                orderParameter.PageNumber,
                orderParameter.PageSize);
        }

        // Get orders for a specific date range with pagination and sorting
        public async Task<PagedList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, OrderParameter orderParameter, bool trackChanges)
        {
            var query = FindByCondition(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate, trackChanges)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .SearchById(orderParameter.SearchById)
                .SearchByDate(orderParameter.SearchByDate)
                .SearchByStatus(orderParameter.SearchByStatus)
                .Sort(orderParameter.OrderBy);

            var count = await query.CountAsync();

            var orders = await query
                .Skip((orderParameter.PageNumber - 1) * orderParameter.PageSize)
                .Take(orderParameter.PageSize)
                .ToListAsync();

            return new PagedList<Order>(
                orders,
                count,
                orderParameter.PageNumber,
                orderParameter.PageSize);
        }
    }
}
