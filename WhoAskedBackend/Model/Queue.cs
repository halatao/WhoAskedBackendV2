namespace WhoAskedBackend.Model;

public class Queue
{
    public Queue()
    {
    }

    public long QueueId { get; set; }
    public string QueueName { get; set; }

    public User Owner { get; set; }
    public long OwnerId { get; set; }

    public ICollection<UserInQueue> Users { get; set; }
}