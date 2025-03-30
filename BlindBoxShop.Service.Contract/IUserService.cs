using BlindBoxShop.Shared.DataTransferObject.Users;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IUserService : IDisposable
    {
        Task<object> GetUserByIdAsync(Guid userId, bool trackChanges);
        Task<bool> UpdateUserAsync(Guid userId, object user);
        Task<Result<IEnumerable<UserDtoWithRelation>>> GetStaffListAsync(UserParameters userParameters, bool trackChanges);
        Task<Result<IEnumerable<UserDtoWithRelation>>> GetUserListAsync(UserParameters userParameters, bool trackChanges);
    }
}
