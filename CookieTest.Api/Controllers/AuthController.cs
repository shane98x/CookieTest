using CookieTest.Api.Db;
using CookieTest.Api.Db.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CookieTest.Api.Request;

namespace CookieTest.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly CookieDbContext _context;
        public AuthController(CookieDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            // Just for testing
            if (request.Username == "user" && request.Password == "pw") 
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Username)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [Authorize]
        [HttpGet("Info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}
