using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IModel = RabbitMQ.Client.IModel;

namespace WhoAskedBackend.Services.Messaging
{
    public class QueueConnection
    {
        public readonly IModel? Channel;
        public readonly string? Exchange;
        public long QueueId;

        public QueueConnection(long queueId, BrokerConnection brokerConnection)
        {
            QueueId = queueId;
            Channel = brokerConnection.Channel;
            Exchange = brokerConnection.Exchange;
            this.Exchange = brokerConnection.Exchange;
            CreateQueue(queueId);
            CreateConsumer(queueId);
        }

        private void CreateQueue(long queueId)
        {
            Channel?.QueueDeclare(queueId.ToString(), true, false, false, null);
            Channel?.QueueBind(queueId.ToString(), Exchange, queueId.ToString(), null);
        }

        private void CreateConsumer(long queueId)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += MessageReceivedHandler.OnMessageReceived;
            Channel.BasicConsume(queueId.ToString(), true, consumer);
        }
    }
}