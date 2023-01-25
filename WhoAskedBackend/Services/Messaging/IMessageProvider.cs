using WhoAskedBackend.Model.Messaging;

namespace WhoAskedBackend.Services.Messaging
{
    public interface IMessageProvider
    {
        void ImportQueue(int queueId);
        void ExportQueue(int queueId);
        List<Message>? RetrieveLatestMessages(int queueId, int amount);
        void QueueMessage(Message message);
    }
}