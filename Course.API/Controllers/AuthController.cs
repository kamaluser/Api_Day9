using Course.Core.Entities;
using Course.Service.Dtos.UserDtos;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("users")]
        public async Task<IActionResult> CreateUser()
        {
            /*await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));*/

            AppUser user1 = new AppUser
            {
                FullName = "Member",
                UserName = "member",
            };
            await _userManager.CreateAsync(user1, "Member123");


            AppUser user2 = new AppUser
            {
                FullName = "Admin",
                UserName = "admin",
            };
            await _userManager.CreateAsync(user2, "Admin123");

            await _userManager.AddToRoleAsync(user1, "Member");
            await _userManager.AddToRoleAsync(user2, "Admin");

            return Ok(user1.Id);
        }


        [HttpPost("login")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }


        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }
    }
}
