using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository.Contract
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<PagedList<User>> GetUsersAsync(UserParameters userParameters, bool trackChanges);

        Task<User?> GetUserAsync(Guid userId, bool trackChanges);
    }
}
