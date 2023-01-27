using WhoAskedBackend.Model.Messaging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WhoAskedBackend.Services.Messaging
{
    public class MessageProvider
    {
        private readonly QueueProvider _queueProvider;
        private readonly QueueStorage _queueStorage;


        public bool Delivered { get; private set; }

        public MessageProvider(QueueProvider queueProvider, QueueStorage queueStorage)
        {
            this._queueProvider = queueProvider;
            _queueStorage = queueStorage;
            MessageReceivedHandler.SetProvider(this);
        }


        public void QueueMessage(Message message)
        {
            Delivered = false;
            _queueProvider.BasicPublic(message);
        }

        public void ReceivedMessage(Message message)
        {
            _queueStorage.Queues.Find(q => q.QueueId == message.QueueId)?.ReceivedMessage(message);
            Delivered = true;
        }

        public List<Message>? RetrieveLatestMessages(long queueId, int amount)
        {
            return _queueStorage.Queues.Find(q => q.QueueId == queueId)?.RetrieveLatestMessages(amount);
        }
    }
}