using WhoAsked.Services.Models;
using WhoAskedBackend.Services.ContextServices;
using WhoAskedBackend.Services.Messaging;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.Messaging
{
    public class QueueProvider
    {
        private readonly List<QueueConnection>? _queues;
        private readonly BrokerConnection _brokerConnection;
        private readonly QueueService _queueService;
        private readonly MessageReceivedHandler _messageReceivedHandler;

        public QueueProvider(BrokerConnection brokerConnection, QueueService queueService)
        {
            this._brokerConnection = brokerConnection;
            this._queueService = queueService;
            _queues = new List<QueueConnection>();
            SetQueues().Wait();
        }

        public async Task SetQueues()
        {
            _queues?.Clear();
            var queues = await _queueService.GetAll();
            foreach (var queue in queues)
            {
                _queues?.Add(new QueueConnection(queue.QueueId, _brokerConnection));
            }
        }

        public void BasicPublic(Message message)
        {
            var queue = _queues?.Find(queue => queue.QueueId == message.QueueId);
            var messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(message.ToString());
            queue?.Channel?.BasicPublish(queue.Exchange, message.QueueId.ToString(), false, null, messageBodyBytes);
        }
    }
}