using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService(AppDbContext db, IOneTimePasswordService otpService) : IUserService
    {
        private readonly PasswordHasher<User> passwordHasher = new();

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await db.Users.AsNoTracking().Where(u => u.Login != "ADMIN").ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await db.Users.FindAsync(id);
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> CreateAsync(CreateUserDto dto)
        {
            if (await db.Users.AnyAsync(u => u.Login == dto.Login))
                throw new InvalidOperationException("Login już istnieje.");

            var user = new User
            {
                Login = dto.Login,
                Name = dto.Name,
                RoleId = dto.RoleId ?? 1,
                RequirePasswordRules = dto.RequirePasswordRules,
                PasswordValidTo = dto.PasswordValidTo
            };

            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return null;

            if (dto.Name != null) user.Name = dto.Name;
            if (dto.IsBlocked.HasValue) user.IsBlocked = dto.IsBlocked.Value;
            if (dto.RoleId.HasValue) user.RoleId = dto.RoleId.Value;
            if (dto.RequirePasswordRules.HasValue) user.RequirePasswordRules = dto.RequirePasswordRules.Value;
            if (dto.PasswordValidTo.HasValue) user.PasswordValidTo = dto.PasswordValidTo;

            await db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null) return false;

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto changePasswordDto)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null) return false;
            if (!IsNewPasswordValid(user, changePasswordDto.NewPassword))
                return false;
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                return false;

            user.PasswordHash = passwordHasher.HashPassword(user, changePasswordDto.NewPassword);

            await db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Set requiresExpiredPassword = true for user's update, requiresExpiredPassword = false for admin's update
        /// </summary>
        public async Task<bool> SetNewPasswordAsync(Guid id, string newPassword, bool requiresExpiredPassword = true)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null) return false;
            if (requiresExpiredPassword && user.PasswordValidTo.HasValue && user.PasswordValidTo > DateTime.UtcNow && !user.MustChangePassword)
                return false;
            if (!IsNewPasswordValid(user, newPassword))
                return false;

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            if (requiresExpiredPassword)
                user.MustChangePassword = false;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<User?> Login(LoginDto dto)
        {
            var user = await GetByLoginAsync(dto.Login);
            if (user == null) return null;
            if (user.IsBlocked) return null;

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Failed)
            {
                return user;
            }

            if (double.TryParse(dto.Password, out var otpValue) && await otpService.VerifyOneTimePasswordAsync(user, otpValue))
            {
                return user;
            }

            return null;
        }

        private static bool IsNewPasswordValid(User user, string password)
            => !user.RequirePasswordRules || VerifyPasswordRules(password);

        private static bool VerifyPasswordRules(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            bool hasLowercase = password.Any(char.IsLower);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasLowercase && hasSpecial;
        }
    }
}
