using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInQueueController : ControllerBase
    {
        private readonly UserInQueueService _userInQueueService;

        public UserInQueueController(UserInQueueService userInQueueService)
        {
            _userInQueueService = userInQueueService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(long userId, long queueId)
        {
            return Ok(await _userInQueueService.Create(userId, queueId));
        }

        [AllowAnonymous]
        [HttpPost("SetSeen")]
        public async Task<IActionResult> SetSeen(long queueId, long userId)
        {
            await _userInQueueService.SetSeen(queueId, userId);
            return Ok();
        }
    }
}