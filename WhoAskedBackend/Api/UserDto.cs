namespace WhoAskedBackend.Api;

public class UserDto
{
    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
    public IEnumerable<QueueDto>? Queues { get; set; }
}