using System;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IUserService : IDisposable
    {
        Task<object> GetUserByIdAsync(Guid userId, bool trackChanges);
        Task<bool> UpdateUserAsync(Guid userId, object user);
    }
}
