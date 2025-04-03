using System;
using System.Threading.Tasks;
using BlindBoxShop.Shared.DataTransferObject.Account;
using BlindBoxShop.Shared.ResultModel;
namespace BlindBoxShop.Service.Contract
{
    public interface IUserService : IDisposable
    {
        Task<object> GetUserByIdAsync(Guid userId, bool trackChanges);
        Task<bool> UpdateUserAsync(Guid userId, object user);

        //list account for admin
        Task<Result<IEnumerable<AccountDto>>> GetAllAccountsAsync(AccountParameter accountParameter, bool trackChanges);

        //create account for admin
        Task<Result<AccountDto>> CreateAccountAsync(AccountForCreateDto account);

        //update account for admin
        Task<Result<AccountDto>> UpdateAccountAsync(Guid accountId, AccountForUpdateDto account, bool trackChanges);

        //delete account for admin
        Task<Result<AccountDto>> DeleteAccountAsync(Guid accountId, bool trackChanges);

        //get total count of accounts
        Task<int> GetTotalCountAsync(AccountParameter accountParameter, bool trackChanges);
    }
}
