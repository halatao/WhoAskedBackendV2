using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Services.Messaging;

public class QueueStorage
{
    private readonly ActiveUsersService _activeUsersService;
    public List<MessageStorage> Queues { get; set; }

    public QueueStorage(ActiveUsersService activeUsersService)
    {
        _activeUsersService = activeUsersService;
        this.Queues = new List<MessageStorage>();
    }
}