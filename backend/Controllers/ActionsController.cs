using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController(IActionsService actionsService) : ControllerBase
    {
        [HttpGet("perform-file-edit")]
        public async Task<IActionResult> PerformFileEdit()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var canEdit = await actionsService.CanEditFileAsync(userId);
            return Ok(canEdit);
        }

        [HttpPost("generate-unlock-key")]
        public IActionResult GenerateUnlockKey([FromBody] GenerateKeyDto dto)
        {
            var encryptedKey = actionsService.GenerateUnlockKey(dto.UserId);
            return Ok(new UnlockKeyDto { EncryptedKey = encryptedKey });
        }

        [HttpPost("validate-unlock-key")]
        public async Task<IActionResult> ValidateUnlockKey([FromBody] ValidateUnlockKeyDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var isValid = await actionsService.ValidateUnlockKeyAsync(userId, dto.EncryptedKey);

            return Ok(new UnlockKeyResponseDto
            {
                Success = isValid,
                Message = isValid ? "Klucz poprawny - dostęp aktywowany" : "Nieprawidłowy klucz"
            });
        }
    }
}
