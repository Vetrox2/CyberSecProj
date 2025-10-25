using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class OneTimePasswordService(AppDbContext db) : IOneTimePasswordService
    {
        public async Task<double> GenerateOneTimePasswordAsync(string userLogin)
        {
            await DeactiveAllOTPs(userLogin);

            var (otpToDb, otpForUser) = GenerateOtp(userLogin);
            var oneTimePassword = new OneTimePassword
            {
                Id = Guid.NewGuid(),
                UserLogin = userLogin,
                Active = true,
                Password = otpToDb
            };

            db.OneTimePasswords.Add(oneTimePassword);
            await db.SaveChangesAsync();

            return otpForUser;
        }

        public async Task<bool> VerifyOneTimePasswordAsync(User user, double password)
        {
            var otp = await GetActivePasswordAsync(user.Login);
            if (otp == null)
                return false;

            var a = user.Login.ToString().Length;
            var yPrime = a / password;

            if (Math.Abs(yPrime - otp.Password) < 0.0001)
            {
                otp.Active = false;
                await db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        private async Task<OneTimePassword?> GetActivePasswordAsync(string userLogin)
        {
            return await db.OneTimePasswords
                .Where(otp => otp.UserLogin == userLogin && otp.Active)
                .FirstOrDefaultAsync();
        }

        private async Task DeactiveAllOTPs(string userLogin)
        {
            var existingPasswords = await db.OneTimePasswords
                .Where(otp => otp.UserLogin == userLogin && otp.Active)
                .ToListAsync();

            foreach (var otp in existingPasswords)
            {
                otp.Active = false;
            }

            await db.SaveChangesAsync();
        }

        private (double otpToDb, double otpForUser) GenerateOtp(string userLogin)
        {
            var a = userLogin.ToString().Length;

            var random = new Random();
            double x;
            do
            {
                x = random.Next(1, 10000);
            } while (x == 0);

            var y = a / x;
            return (y, x);
        }
    }
}
