using WhoAskedBackend.Model;

namespace WhoAskedBackend.Api;

public class UserInQueueDto
{
    public long UserId { get; set; }
    public long QueueId { get; set; }
}