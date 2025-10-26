using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AppSettingsService(AppDbContext db) : IAppSettingsService
    {
        public async Task<AppSettings> GetSettingsAsync()
        {
            var settings = await db.AppSettings.FindAsync(1);

            if (settings == null)
            {
                settings = new AppSettings
                {
                    Id = 1,
                };
                db.AppSettings.Add(settings);
                await db.SaveChangesAsync();
            }

            return settings;
        }

        public async Task<AppSettings> UpdateSettingsAsync(AppSettingsDto dto)
        {
            var settings = await GetSettingsAsync();

            settings.SessionTimeoutMinutes = dto.SessionTimeoutMinutes;
            settings.MaxFailedLoginAttempts = dto.MaxFailedLoginAttempts;
            settings.LockoutDurationMinutes = dto.LockoutDurationMinutes;

            await db.SaveChangesAsync();
            return settings;
        }
    }
}
