using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInQueueController : ControllerBase
    {
        private readonly UserInQueueService _userInQueueService;

        public UserInQueueController(UserInQueueService _, UserInQueueService userInQueueService)
        {
            _userInQueueService = userInQueueService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(long userId, long queueId)
        {
            return Ok(await _userInQueueService.Create(userId, queueId));
        }
    }
}