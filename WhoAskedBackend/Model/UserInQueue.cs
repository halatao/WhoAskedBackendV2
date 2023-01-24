namespace WorkIT_Backend.Model;

public class UserInQueue
{
    public long UserId { get; set; }
    public User? User { get; set; }
    public long QueueId { get; set; }
    public Queue? Queue { get; set; }
}