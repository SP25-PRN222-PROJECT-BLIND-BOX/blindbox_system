using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
