using Microsoft.AspNetCore.Mvc;
using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Api;
using WhoAskedBackend.Services.Messaging;

namespace WhoAskedBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageProvider? _messageProvider;

        public MessagesController(MessageProvider messageProvider)
        {
            _messageProvider = messageProvider;
        }

        [HttpGet("{queueId}/{amount}")]
        public IActionResult GetMessages(long queueId, int amount)
        {
            return Ok(_messageProvider?.RetrieveLatestMessages(queueId, amount)!.Select(mess => new MessageDto
            {
                QueueId = mess.QueueId,
                Sender = mess.Sender,
                Sent = mess.Sent,
                Mess = mess.Mess,
            }).ToList());
        }

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
    }
}