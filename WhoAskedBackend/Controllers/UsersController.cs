using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Api;
using WhoAskedBackend.Model;
using WhoAskedBackend.Services;
using WhoAskedBackend.Services.ContextServices;
using WhoAskedBackend.Services.Messaging;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly SecurityService _securityService;
        private readonly ActiveUsersService _activeUsersService;
        private readonly MessageProvider? _messageProvider;
        private readonly UserInQueueService _userInQueueService;

        public UsersController([FromServices] SecurityService securityService, [FromServices] UserService userService,
            ActiveUsersService activeUsersService, MessageProvider? messageProvider,
            UserInQueueService userInQueueService)
        {
            _securityService = securityService;
            _userService = userService;
            _activeUsersService = activeUsersService;
            _messageProvider = messageProvider;
            _userInQueueService = userInQueueService;
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpPost("UserStatus")]
        public IActionResult UserStatus(string username, bool active)
        {
            _activeUsersService.ToggleUserActive(username, active);
            return Ok();
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpGet("UsersOnline")]
        public IActionResult UsersOnline()
        {
            return Ok(_activeUsersService.GetActiveUsers());
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpPost("SetAvatar")]
        public async Task<IActionResult> SetAvatarByUsername(AvatarPostDto avatar)
        {
            await _userService.SetAvatarByUsername(avatar.UserName, avatar.AvatarName);
            return Ok();
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpPost("RemoveFromQueue")]
        public async Task<IActionResult> RemoveFromQueue(RemoveFromQueueDto removeFromQueue)
        {
            await _userInQueueService.RemoveFromQueue(removeFromQueue.QueueId, removeFromQueue.UserId);
            return Ok();
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpPost("AddToQueue")]
        public async Task<IActionResult> AddToQueue(AddToQueueDto addToQueue)
        {
            await _userInQueueService.AddToQueue(addToQueue.QueueId, addToQueue.UserName);
            return Ok();
        }

        [AllowAnonymous] //[Authorize(Roles = CustomRoles.User)]
        [HttpGet("ByUsername")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userService.GetByUsername(username);

            return Ok(UserToDto(user));
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
        public async Task<IActionResult> Login(UserLoginDto login)
        {
            User user;
            try
            {
                user = await _userService.GetUserByCredentials(login.UserName, login.Password);
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
            return Ok(_securityService.BuildJwtToken(await _userService.Create(user.UserName, user.Password)));
        }

        [NonAction]
        public UserDto UserToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Avatar = user.Avatar,
                Queues = user.Queues.Select(q => new QueueDto
                {
                    QueueId = q.QueueId,
                    QueueName = q.Queue?.QueueName,
                    LatestMessage = _messageProvider!.RetrieveLatestMessage(q.QueueId).Mess,
                    OwnerUsername = q.Queue?.Owner.UserName,
                    Seen = q.Seen,
                    Users =
                        q.Queue?.Users.Select(r => new UserSimpleDto
                            {UserId = r.User.UserId, UserName = r.User.UserName, Avatar = r.User.Avatar})
                })
            };
        }
    }
}