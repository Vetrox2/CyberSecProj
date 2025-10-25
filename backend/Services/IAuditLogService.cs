using backend.Models;

namespace backend.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid userId, string actionType, bool successState);
        Task<IEnumerable<LogDto>> GetAllLogsAsync();
        Task<IEnumerable<LogDto>> GetLogsByUserIdAsync(Guid userId);
    }
}
