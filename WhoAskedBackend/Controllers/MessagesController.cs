using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Api;
using WhoAskedBackend.Services.Messaging;
using WhoAskedBackend.Services.ContextServices;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net.WebSockets;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageProvider? _messageProvider;
        private readonly UserInQueueService _userInQueueService;

        public MessagesController(MessageProvider messageProvider, UserInQueueService userInQueueService)
        {
            _messageProvider = messageProvider;
            _userInQueueService = userInQueueService;
        }

        [Authorize(Roles = CustomRoles.User)]
        [HttpGet("{queueId}/{amount}")]
        public async Task<IActionResult> GetMessages(long queueId, long userId, int amount)
        {
            await _userInQueueService.SetSeen(queueId, userId);
            return Ok((_messageProvider?.RetrieveLatestMessages(queueId, amount)!).Select(mess => new MessageDto
            {
                QueueId = mess.QueueId,
                Sender = mess.Sender,
                Sent = mess.Sent,
                Mess = mess.Mess,
            }).ToList());
        }

        [Authorize(Roles = CustomRoles.User)]
        [HttpPost]
        public IActionResult PostMessage(MessageDto message)
        {
            _messageProvider?.QueueMessage(new Message
                {Mess = message.Mess!, QueueId = message.QueueId, Sender = message.Sender, Sent = message.Sent});
            if (_messageProvider?.Delivered == false)
            {
                Thread.Sleep(100);
            }

            return Ok();
        }

        [Route("/ws")]
        [HttpGet]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(HttpContext, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }




    }
}