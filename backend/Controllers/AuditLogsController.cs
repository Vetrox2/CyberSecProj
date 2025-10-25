using backend.Mappers;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logDtos = await auditLogService.GetAllLogsAsync();
            return Ok(logDtos);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var logDtos = await auditLogService.GetLogsByUserIdAsync(userId);
            return Ok(logDtos);
        }
    }
}
