﻿using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Services.Messaging
{
    public class QueueProvider
    {
        private readonly List<QueueConnection>? _queues;
        private readonly BrokerConnection _brokerConnection;
        private readonly QueueService _queueService;
        private readonly QueueStorage _queueStorage;

        public QueueProvider(BrokerConnection brokerConnection, QueueService queueService, QueueStorage queueStorage)
        {
            this._brokerConnection = brokerConnection;
            this._queueService = queueService;
            _queueStorage = queueStorage;
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
                _queueStorage.Queues.Add(new MessageStorage(queue.QueueId));
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