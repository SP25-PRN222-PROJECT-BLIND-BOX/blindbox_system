using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<User?> GetUserAsync(Guid userId, bool trackChanges)
        {
            return await FindById(userId, trackChanges);
        }

        public async Task<PagedList<User>> GetUsersAsync(UserParameters userParameters, bool trackChanges)
        {
            var users = await FindAll(trackChanges)
                .FilterByRole(userParameters.Role)
                .Sort(userParameters.OrderBy)
                .ToPagedListAsync(userParameters);

            return users;
        }
    }
}
