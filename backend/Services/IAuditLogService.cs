using backend.Models;

namespace backend.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid userId, string actionType, bool successState);
        Task<IEnumerable<AuditLog>> GetAllLogsAsync();
        Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(Guid userId);
    }
}
