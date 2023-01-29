using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.ContextServices;

public class UserInQueueService : ModelServiceBase
{
    private readonly WhoAskedContext _context;

    public UserInQueueService(WhoAskedContext context)
    {
        _context = context;
    }

    public async Task<UserInQueue> Create(long userId, long queueId)
    {
        var ret = new UserInQueue {QueueId = queueId, UserId = userId, Seen = false};
        _context.Add(ret);
        await _context.SaveChangesAsync();
        return ret;
    }

    public async Task RemoveFromQueue(long queueId, long userId)
    {
        var ret = await (_context.UserInQueue ?? throw new InvalidOperationException()).FirstAsync(q =>
            q.QueueId == queueId && q.UserId == userId);

        _context.Remove(ret);
        await _context.SaveChangesAsync();
    }

    public async Task AddToQueue(long queueId, string username)
    {
        var user = await (_context.Users ?? throw new InvalidOperationException()).FirstAsync(q =>
            q.UserName == username);
        var userInQueue = new UserInQueue {QueueId = queueId, UserId = user.UserId};
        _context.Add(userInQueue);
        await _context.SaveChangesAsync();
    }

    public async Task SetSeen(long queueId, long userId)
    {
        var ret = await (_context.UserInQueue ?? throw new InvalidOperationException("User is not in this queue"))
            .FirstAsync(q =>
                q.QueueId == queueId && q.UserId == userId);
        ret.Seen = true;

        await _context.SaveChangesAsync();
    }
}