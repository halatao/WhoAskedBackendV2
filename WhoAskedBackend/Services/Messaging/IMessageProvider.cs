using WhoAskedBackend.Model.Messaging;

namespace WhoAskedBackend.Services.Messaging
{
    public interface IMessageProvider
    {
        void ImportQueue(long queueId);
        void ExportQueue(long queueId);
        List<Message>? RetrieveLatestMessages(long queueId, int amount);
        void QueueMessage(Message message);
    }
}