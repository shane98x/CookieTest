using CookieTest.Api.Db;
using CookieTest.Api.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieTest.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]/")]
    public class UserController : ControllerBase
    {
        private readonly CookieDbContext _context;
        public UserController(CookieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
    }
}
