using backend.Models;

namespace backend.Services
{
    public interface IAppSettingsService
    {
        Task<AppSettings> GetSettingsAsync();
        Task<AppSettings> UpdateSettingsAsync(AppSettingsDto dto);
    }
}
