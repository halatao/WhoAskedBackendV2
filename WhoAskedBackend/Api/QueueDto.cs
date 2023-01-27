namespace WhoAskedBackend.Api;

public class QueueDto
{
    public long QueueId { get; set; }
    public string? QueueName { get; set; }
    public string? LatestMessage { get; set; }
    public string? OwnerUsername { get; set; }
    public IEnumerable<UserSimpleDto>? Users { get; set; }
}