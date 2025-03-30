using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IAuthService : IDisposable
    {
        Task<Result<UserLoginResult>> LoginAsync(string email, string password);
    }

    public class UserLoginResult
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
} 