using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController(IAppSettingsService settingsService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await settingsService.GetSettingsAsync();
            var dto = new AppSettingsDto
            {
                SessionTimeoutMinutes = settings.SessionTimeoutMinutes,
                MaxFailedLoginAttempts = settings.MaxFailedLoginAttempts,
                LockoutDurationMinutes = settings.LockoutDurationMinutes
            };
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] AppSettingsDto dto)
        {
            var settings = await settingsService.UpdateSettingsAsync(dto);
            var resultDto = new AppSettingsDto
            {
                SessionTimeoutMinutes = settings.SessionTimeoutMinutes,
                MaxFailedLoginAttempts = settings.MaxFailedLoginAttempts,
                LockoutDurationMinutes = settings.LockoutDurationMinutes
            };
            return Ok(resultDto);
        }
    }
}
