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

        public async Task<IEnumerable<LogDto>> GetAllLogsAsync()
        {
            return await db.AuditLogs
                .Join(db.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new LogDto
                    {
                        UserLogin = user.Login,
                        Timestamp = log.Date.ToString(),
                        Action = log.ActionType,
                        Success = log.SuccessState
                    })
                .OrderByDescending(dto => dto.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogDto>> GetLogsByUserIdAsync(Guid userId)
        {
            return await db.AuditLogs
                .Where(log => log.UserId == userId)
                .Join(db.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new LogDto
                    {
                        UserLogin = user.Login,
                        Timestamp = log.Date.ToString(),
                        Action = log.ActionType,
                        Success = log.SuccessState
                    })
                .OrderByDescending(dto => dto.Timestamp)
                .ToListAsync();
        }
    }
}
