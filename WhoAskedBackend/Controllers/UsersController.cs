using System.Diagnostics.Tracing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend;
using WhoAskedBackend.Services;
using WorkIT_Backend.Api;
using WorkIT_Backend.Model;
using WorkIT_Backend.Services;

namespace WorkIT_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly SecurityService _securityService;

        public UsersController([FromServices] SecurityService securityService, [FromServices] UserService userService)
        {
            _securityService = securityService;
            _userService = userService;
        }

        [HttpGet("ByUsername")]
        [Authorize(Roles = CustomRoles.User)]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userService.GetByUsername(username);

            return Ok(user);
        }

        [HttpGet("All")]
        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userService.GetAll();

            return Ok(user);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            User user;
            try
            {
                user = await _userService.GetUserByCredentials(username, password);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(_securityService.BuildJwtToken(user));
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserLoginDto user)
        {
            return Ok(_securityService.BuildJwtToken(await _userService.Create(user.UserName!, user.Password!)));
        }
    }
}