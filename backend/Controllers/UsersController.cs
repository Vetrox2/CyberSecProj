using backend.Mappers;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAllAsync();
            var dtos = users.Select(u => u.ToDto());
            return Ok(dtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var dto = user.ToDto();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await userService.CreateAsync(dto);
                var resultDto = user.ToDto();
                return CreatedAtAction(nameof(Get), new { id = user.Id }, resultDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await userService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            var ok = await userService.ChangePasswordAsync(id, dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpPost("{id:guid}/set-password-admin")]
        public async Task<IActionResult> SetPasswordByAdmin(Guid id, [FromBody] SetNewPasswordDto dto)
        {
            var ok = await userService.SetNewPasswordAsync(id, dto.NewPassword, false);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpPost("{id:guid}/set-password-user")]
        public async Task<IActionResult> SetPasswordByUser(Guid id, [FromBody] SetNewPasswordDto dto)
        {
            var ok = await userService.SetNewPasswordAsync(id, dto.NewPassword, true);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}
