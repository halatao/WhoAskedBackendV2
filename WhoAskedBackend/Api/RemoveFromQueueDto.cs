using WhoAskedBackend.Model;

namespace WhoAskedBackend.Api;

public class RemoveFromQueueDto
{
    public long UserId { get; set; }
    public long QueueId { get; set; }
}