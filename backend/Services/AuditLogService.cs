using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AuditLogService(AppDbContext db) : IAuditLogService
    {
        public async Task LogAsync(Guid userId, string actionType, bool successState)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = DateTime.UtcNow,
                ActionType = actionType,
                SuccessState = successState
            };

            db.AuditLogs.Add(auditLog);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllLogsAsync()
        {
            return await db.AuditLogs
                .OrderByDescending(log => log.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(Guid userId)
        {
            return await db.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Date)
                .ToListAsync();
        }
    }
}
