using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Account;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        
        public UserService(
            IRepositoryManager repositoryManager, 
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Roles> roleManager) 
            : base(repositoryManager, mapper)
        {
            _userRepository = repositoryManager.User;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<object> GetUserByIdAsync(Guid userId, bool trackChanges)
        {
            try
            {
                var user = await _userRepository.FindById(userId, trackChanges);
                if (user == null)
                {
                    // Log the error for server-side debugging
                    Console.WriteLine($"User with ID {userId} was not found in the database");
                }
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error retrieving user with ID {userId}: {ex.Message}");
                throw; // Rethrow to let the caller handle it
            }
        }

        public async Task<bool> UpdateUserAsync(Guid userId, object userObj)
        {
            try
            {
                var user = userObj as User;
                if (user == null)
                    return false;
                    
                var existingUser = await _userRepository.FindById(userId, true);
                if (existingUser == null)
                    return false;

                // Update user properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Email = user.Email;
                existingUser.Address = user.Address;
                existingUser.Provinces = user.Provinces;
                existingUser.District = user.District;
                existingUser.Wards = user.Wards;
                existingUser.UpdatedAt = DateTime.Now;

                await _userRepository.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _userRepository.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result<IEnumerable<AccountDto>>> GetAllAccountsAsync(AccountParameter accountParameter, bool trackChanges)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Apply search term
                if (!string.IsNullOrWhiteSpace(accountParameter.SearchTerm))
                {
                    query = query.Where(u =>
                        u.UserName.Contains(accountParameter.SearchTerm) ||
                        u.Email.Contains(accountParameter.SearchTerm) ||
                        u.PhoneNumber.Contains(accountParameter.SearchTerm) ||
                        u.FirstName.Contains(accountParameter.SearchTerm) ||
                        u.LastName.Contains(accountParameter.SearchTerm));
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(accountParameter.Role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(accountParameter.Role);
                    var userIds = usersInRole.Select(u => u.Id);
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                // Apply active status filter
                if (accountParameter.IsActive.HasValue)
                {
                    query = query.Where(u => u.LockoutEnabled == !accountParameter.IsActive.Value);
                }

                // Apply sorting
                if (accountParameter.OrderBy == "name")
                {
                    query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
                }
                else if (accountParameter.OrderBy == "name_desc")
                {
                    query = query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName);
                }
                else if (accountParameter.OrderBy == "created")
                {
                    query = query.OrderBy(u => u.CreatedAt);
                }
                else if (accountParameter.OrderBy == "created_desc")
                {
                    query = query.OrderByDescending(u => u.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(u => u.CreatedAt);
                }

                // Apply pagination
                var totalCount = await query.CountAsync();
                var users = await query
                    .Skip((accountParameter.PageNumber - 1) * accountParameter.PageSize)
                    .Take(accountParameter.PageSize)
                    .ToListAsync();

                var accountDtos = new List<AccountDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var accountDto = _mapper.Map<AccountDto>(user);
                    accountDto.Roles = roles.ToList();
                    accountDto.IsActive = !user.LockoutEnabled;
                    accountDtos.Add(accountDto);
                }

                return Result<IEnumerable<AccountDto>>.Success(accountDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AccountDto>>.Failure(new ErrorResult
                {
                    Code = "GetAccountsError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<AccountDto>> CreateAccountAsync(AccountForCreateDto account)
        {
            try
            {
               

                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(account.Email);
                if (existingUser != null)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "EmailExists",
                        Description = "Email already exists."
                    });
                }

                // Map DTO to User entity
                var user = _mapper.Map<User>(account);
                user.CreatedAt = DateTime.UtcNow;
                user.EmailConfirmed = true; // Auto confirm email for staff/manager accounts
                user.PhoneNumberConfirmed = true; // Auto confirm phone for staff/manager accounts

                // Create user with password
                var result = await _userManager.CreateAsync(user, account.Password);
                if (!result.Succeeded)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "CreateUserError",
                        Description = string.Join(", ", result.Errors.Select(e => e.Description))
                    });
                }

                // Add user to role
                var roleResult = await _userManager.AddToRoleAsync(user, account.Role);
                if (!roleResult.Succeeded)
                {
                    // If adding role fails, delete the user
                    await _userManager.DeleteAsync(user);
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "AddRoleError",
                        Description = string.Join(", ", roleResult.Errors.Select(e => e.Description))
                    });
                }

                // Map user to AccountDto and return
                var accountDto = _mapper.Map<AccountDto>(user);
                accountDto.Roles = new List<string> { account.Role };
                accountDto.IsActive = true;

                return Result<AccountDto>.Success(accountDto);
            }
            catch (Exception ex)
            {
                return Result<AccountDto>.Failure(new ErrorResult
                {
                    Code = "CreateAccountError",
                    Description = ex.Message
                });
            }
        }

     
        public async Task<Result<AccountDto>> DeleteAccountAsync(Guid accountId)
        {
            try
            {
                // Find user by id
                var user = await _userManager.FindByIdAsync(accountId.ToString());
                if (user == null)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "UserNotFound",
                        Description = "User not found."
                    });
                }

                // Check if user is already deleted
                if (user.LockoutEnabled)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "UserAlreadyDeleted",
                        Description = "User is already deleted."
                    });
                }

                // Soft delete by enabling lockout
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue; // Lock forever
                user.UpdatedAt = DateTime.UtcNow;

                // Update user
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "DeleteUserError",
                        Description = string.Join(", ", result.Errors.Select(e => e.Description))
                    });
                }

                // Map user to AccountDto and return
                var accountDto = _mapper.Map<AccountDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                accountDto.Roles = roles.ToList();
                accountDto.IsActive = false;

                return Result<AccountDto>.Success(accountDto);
            }
            catch (Exception ex)
            {
                return Result<AccountDto>.Failure(new ErrorResult
                {
                    Code = "DeleteAccountError",
                    Description = ex.Message
                });
            }
        }

        public async Task<Result<AccountDto>> UpdateAccountAsync(Guid accountId, AccountForUpdateDto account, bool trackChanges)
        {
            try
            {
                // Find user by id
                var user = await _userManager.FindByIdAsync(accountId.ToString());
                if (user == null)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "UserNotFound",
                        Description = "User not found."
                    });
                }

                // Check if email is being changed and already exists
                if (user.Email != account.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(account.Email);
                    if (existingUser != null)
                    {
                        return Result<AccountDto>.Failure(new ErrorResult
                        {
                            Code = "EmailExists",
                            Description = "Email already exists."
                        });
                    }
                }

                // Update user properties
                user.FirstName = account.FirstName;
                user.LastName = account.LastName;
                user.Email = account.Email;
                user.PhoneNumber = account.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                // Update user
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result<AccountDto>.Failure(new ErrorResult
                    {
                        Code = "UpdateUserError",
                        Description = string.Join(", ", result.Errors.Select(e => e.Description))
                    });
                }

                // Update role if changed
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(account.Role))
                {
                    // Remove all current roles
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    // Add new role
                    var roleResult = await _userManager.AddToRoleAsync(user, account.Role);
                    if (!roleResult.Succeeded)
                    {
                        return Result<AccountDto>.Failure(new ErrorResult
                        {
                            Code = "UpdateRoleError",
                            Description = string.Join(", ", roleResult.Errors.Select(e => e.Description))
                        });
                    }
                }

                // Map user to AccountDto and return
                var accountDto = _mapper.Map<AccountDto>(user);
                accountDto.Roles = new List<string> { account.Role };
                accountDto.IsActive = !user.LockoutEnabled;

                return Result<AccountDto>.Success(accountDto);
            }
            catch (Exception ex)
            {
                return Result<AccountDto>.Failure(new ErrorResult
                {
                    Code = "UpdateAccountError",
                    Description = ex.Message
                });
            }
        }

        public Task<Result<AccountDto>> DeleteAccountAsync(Guid accountId, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalCountAsync(AccountParameter accountParameter, bool trackChanges)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Apply search term
                if (!string.IsNullOrWhiteSpace(accountParameter.SearchTerm))
                {
                    query = query.Where(u =>
                        u.UserName.Contains(accountParameter.SearchTerm) ||
                        u.Email.Contains(accountParameter.SearchTerm) ||
                        u.PhoneNumber.Contains(accountParameter.SearchTerm) ||
                        u.FirstName.Contains(accountParameter.SearchTerm) ||
                        u.LastName.Contains(accountParameter.SearchTerm));
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(accountParameter.Role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(accountParameter.Role);
                    var userIds = usersInRole.Select(u => u.Id);
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                // Apply active status filter
                if (accountParameter.IsActive.HasValue)
                {
                    query = query.Where(u => u.LockoutEnabled == !accountParameter.IsActive.Value);
                }

                return await query.CountAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
