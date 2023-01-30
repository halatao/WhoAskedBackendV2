using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserInQueueController : ControllerBase
    {
        private readonly UserInQueueService _userInQueueService;

        public UserInQueueController(UserInQueueService userInQueueService)
        {
            _userInQueueService = userInQueueService;
        }

        [Authorize(Roles = CustomRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(long userId, long queueId)
        {
            return Ok(await _userInQueueService.Create(userId, queueId));
        }
    }
}