namespace WhoAskedBackend.Model;

public class UserInQueue
{
    public long UserInQueueId { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long QueueId { get; set; }
    public Queue? Queue { get; set; }
    public bool Seen { get; set; }
}