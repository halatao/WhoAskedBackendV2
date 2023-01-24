namespace WorkIT_Backend.Model;

public class Queue
{
    public Queue()
    {
    }

    public long Id { get; set; }
    public string Name { get; set; }

    public User Owner { get; set; }
    public long OwnerId { get; set; }

    public ICollection<UserInQueue> Users { get; set; }
}