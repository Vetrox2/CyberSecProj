using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await db.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}
