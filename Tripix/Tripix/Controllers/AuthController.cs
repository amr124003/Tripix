#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tripix.View_Models;

namespace Tripix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> signinmanger;
        private readonly RoleManager<IdentityRole> rolemanger;

        public AuthController ( UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signinmanger, RoleManager<IdentityRole> rolemanger )
        {
            this.userManager = userManager;
            _configuration = configuration;
            this.signinmanger = signinmanger;
            this.rolemanger = rolemanger;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register ( [FromBody] RegisterModel model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            await userManager.AddToRoleAsync(user, "User");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login ( [FromBody] LoginModel model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid Credentials" });
            }

            var result = await signinmanger.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid Credentials" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AsignRole ( [FromBody] AssignRoleModel model )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User Not Found");
            }

            if (!await rolemanger.RoleExistsAsync(model.Role))
            {
                return BadRequest("Invalid Role");
            }

            await userManager.AddToRoleAsync(user, model.Role);
            return Ok(new { message = $"Role {model.Role} Assigned To {user.UserName}" });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("AddAdmin")]
        public async Task<IActionResult> AddAdmin ( [FromBody] AddAdminModel model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var admin = new IdentityUser { Email = model.Email, UserName = model.Username };
            var result = await userManager.CreateAsync(admin, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            await userManager.AddToRoleAsync(admin, "Admin");
            return Ok(new { message = "Admin Is Created" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAdmins ()
        {
            var result = userManager.GetUsersInRoleAsync("Admin");
            return Ok(result);
        }

        private string GenerateJwtToken ( IdentityUser user )
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = userManager.GetRolesAsync(user).Result;

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
            }.ToList();

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
