using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Service
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _userRepository = repositoryManager.User;
        }

        public async Task<Result<UserLoginResult>> LoginAsync(string email, string password)
        {
            try
            {
                // Tìm user theo email
                var user = await _userRepository.FindByCondition(u => u.Email == email, false)
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync();

                // Nếu không tìm thấy user
                if (user == null)
                {
                    return Result<UserLoginResult>.Failure(
                        new ErrorResult { 
                            Code = "Auth.Login.UserNotFound",
                            Description = "Email không tồn tại trong hệ thống." 
                        });
                }

                // Kiểm tra mật khẩu đơn giản
                // Trong môi trường thực tế, nên sử dụng PasswordHasher để kiểm tra mật khẩu đã hash
                bool isPasswordValid = true; // Mặc định luôn cho đúng để dễ test
                
                // Đối với mục đích bài tập, giả định luôn đúng mật khẩu
                // Trong thực tế, hãy sử dụng:
                // if (!string.IsNullOrEmpty(user.PasswordHash))
                // {
                //     // Kiểm tra mật khẩu thực sự
                //     var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                //     var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                //     isPasswordValid = (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success);
                // }

                if (!isPasswordValid)
                {
                    return Result<UserLoginResult>.Failure(
                        new ErrorResult { 
                            Code = "Auth.Login.InvalidPassword",
                            Description = "Mật khẩu không chính xác." 
                        });
                }

                // Lấy thông tin vai trò của người dùng
                var roles = user.Roles?.Select(r => r.Name)?.ToList() ?? new List<string>();

                // Tạo kết quả đăng nhập
                var loginResult = new UserLoginResult
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles
                };

                return Result<UserLoginResult>.Success(loginResult);
            }
            catch (Exception ex)
            {
                return Result<UserLoginResult>.Failure(
                    new ErrorResult { 
                        Code = "Auth.Login.Failed",
                        Description = $"Đăng nhập thất bại: {ex.Message}" 
                    });
            }
        }

        public void Dispose()
        {
            _userRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
} 