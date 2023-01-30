using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Services.Messaging;

public class QueueStorage
{
    public List<MessageStorage> Queues { get; set; }

    public QueueStorage()
    {
        this.Queues = new List<MessageStorage>();
    }
}