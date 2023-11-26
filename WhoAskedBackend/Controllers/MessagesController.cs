using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Api;
using WhoAskedBackend.Services.Messaging;
using WhoAskedBackend.Services.ContextServices;
using Microsoft.AspNetCore.Authorization;
using System.Net.WebSockets;
using System.Text;
using WebSocketHandler = WhoAskedBackend.Services.WebSocket.WebSocketHandler;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageProvider? _messageProvider;
        private readonly UserInQueueService _userInQueueService;
        private readonly WebSocketHandler _webSocketManager;

        public MessagesController(MessageProvider messageProvider, UserInQueueService userInQueueService,
            WebSocketHandler webSocketManager)
        {
            _messageProvider = messageProvider;
            _userInQueueService = userInQueueService;
            _webSocketManager = webSocketManager;
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

        [Route("/QueueWS")]
        [HttpGet]
        public async Task Get(long queueId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketId = _webSocketManager.AddSocket(queueId, webSocket);
                await Receive(HttpContext, webSocket, socketId);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }


        private async Task Receive(HttpContext context, WebSocket webSocket, string socketId)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            var tcs = new TaskCompletionSource<bool>();

            if (result.CloseStatus.HasValue)
            {
                tcs.SetResult(true);
            }

            await tcs.Task;

            await webSocket.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription,
                CancellationToken.None);
            await _webSocketManager.RemoveSocket(socketId);
        }
    }
}