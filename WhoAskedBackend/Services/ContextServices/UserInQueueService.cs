using WhoAskedBackend.Data;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.ContextServices;

public class UserInQueueService
{
    private readonly WhoAskedContext _context;

    public UserInQueueService(WhoAskedContext context)
    {
        _context = context;
    }

    public async Task<UserInQueue> Create(long userId, long queueId)
    {
        var ret = new UserInQueue {QueueId = queueId, UserId = userId};
        _context.Add(ret);
        await _context.SaveChangesAsync();
        return ret;
    }
}