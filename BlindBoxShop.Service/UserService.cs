using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using System;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _userRepository = repositoryManager.User;
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
    }
}
