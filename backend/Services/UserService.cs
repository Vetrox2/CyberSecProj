using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class UserService(AppDbContext db, IOneTimePasswordService otpService, IConfiguration settings) : IUserService
    {
        private readonly PasswordHasher<User> passwordHasher = new();
        private readonly string recaptchaSecret = settings.GetValue<string>("RecaptchaSecret") ?? "";

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

        public async Task<bool> VerifyRecaptchaAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var http = new HttpClient();
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("secret", recaptchaSecret),
                new KeyValuePair<string, string>("response", token)
            ]);

            HttpResponseMessage resp;
            try
            {
                resp = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            }
            catch (Exception)
            {
                return false;
            }

            if (!resp.IsSuccessStatusCode)
                return false;

            var json = await resp.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var parsed = JsonSerializer.Deserialize<RecaptchaResponse>(json, options);

            return parsed is not null && parsed.Success;
        }

        public async Task<ImageCaptchaDto> GenerateImageCaptchaAsync()
        {
            List<string> categories = ["car", "bike", "building"];
            var random = new Random();
            var selectedCategory = categories[random.Next(categories.Count)];

            var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "captcha");
            var allFiles = Directory.GetFiles(assetsPath, "*.png");

            var correctImages = allFiles
                .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(selectedCategory, StringComparison.OrdinalIgnoreCase))
                .OrderBy(_ => random.Next())
                .Take(3)
                .ToList();

            var incorrectImages = allFiles
                .Where(f => !Path.GetFileNameWithoutExtension(f).StartsWith(selectedCategory, StringComparison.OrdinalIgnoreCase))
                .OrderBy(_ => random.Next())
                .Take(6)
                .ToList();

            var allImages = correctImages.Concat(incorrectImages).OrderBy(_ => random.Next()).ToList();

            var imageDtos = new List<ImageDto>();
            for (int i = 0; i < allImages.Count; i++)
            {
                var imageBytes = await File.ReadAllBytesAsync(allImages[i]);
                var base64 = Convert.ToBase64String(imageBytes);
                imageDtos.Add(new ImageDto
                {
                    Index = i,
                    Base64Data = $"data:image/png;base64,{base64}"
                });
            }

            var correctIndices = new List<int>();
            for (int i = 0; i < allImages.Count; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(allImages[i]);
                if (fileName.StartsWith(selectedCategory, StringComparison.OrdinalIgnoreCase))
                {
                    correctIndices.Add(i);
                }
            }

            var encryptedKey = GenerateCaptchaHash(correctIndices);

            var challengeMessage = selectedCategory switch
            {
                "car" => "Select all cars",
                "bike" => "Select all bikes",
                "tree" => "Select all trees",
                "building" => "Select all buildings",
                _ => "Select matching images"
            };

            return new ImageCaptchaDto
            {
                Images = imageDtos,
                Challenge = challengeMessage,
                EncryptedKey = encryptedKey
            };
        }

        public bool VerifyImageCaptcha(VerifyCaptchaDto dto)
        {
            var userHash = GenerateCaptchaHash(dto.SelectedIndices);
            return userHash == dto.EncryptedKey;
        }

        private static string GenerateCaptchaHash(List<int> indices)
        {
            var sortedIndices = string.Join(",", indices.OrderBy(x => x));
            var bytes = Encoding.UTF8.GetBytes(sortedIndices);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
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
