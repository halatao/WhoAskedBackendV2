using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend;
using WhoAskedBackend.Api;
using WhoAskedBackend.Services;
using WhoAskedBackend.Services.ContextServices;
using WorkIT_Backend.Api;
using WorkIT_Backend.Model;

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

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpPost("UserStatus")]
        public IActionResult UserStatus(string username, bool active)
        {
            _userService.ToggleUserActive(username, active);
            return Ok();
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpGet("UsersOnline")]
        public IActionResult UsersOnline()
        {
            return Ok(_userService.GetActiveUsers());
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
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
            var users = await _userService.GetAll();

            return Ok(users);
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

        //[NonAction]
        //public List<UserDto> UserToDto(List<User> users)
        //{
        //    return users.Select(user => new UserDto
        //    {
        //        UserId = user.UserId,
        //        UserName = user.UserName,
        //        Avatar = user.Avatar,
        //        QueuesOwner = user.OwnedQueues.Select(q => new QueueDto
        //        {
        //            QueueId = q.QueueId,
        //            QueueName = q.QueueName,
        //            LatestMessage = q.LatestMessage,
        //            Users = q.Users.Select(r => new UserSimpleDto
        //                {UserId = r.User.UserId, UserName = r.User.UserName, Avatar = r.User.Avatar})
        //        }),
        //        Queues = user.OwnedQueues.Select(q => new QueueDto
        //        {
        //            QueueId = q.QueueId,
        //            QueueName = q.QueueName,
        //            LatestMessage = q.LatestMessage,
        //            Users =
        //                q.Users.Select(r => new UserSimpleDto
        //                    {UserId = r.User.UserId, UserName = r.User.UserName, Avatar = r.User.Avatar})
        //        })
        //    }).ToList();
        //}
    }
}