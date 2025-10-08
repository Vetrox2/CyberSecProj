using backend.Models;

namespace backend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByLoginAsync(string login);
        Task<User> CreateAsync(CreateUserDto dto);
        Task<User?> UpdateAsync(Guid id, UpdateUserDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto changePasswordDto);
        /// <summary>
        /// Set requiresExpiredPassword = true for user's update, requiresExpiredPassword = false for admin's update
        /// </summary>
        Task<bool> SetNewPasswordAsync(Guid id, string newPassword, bool requiresExpiredPassword = true);
    }
}
