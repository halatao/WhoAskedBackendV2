using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.ContextServices;

public class QueueService
{
    private readonly WhoAskedContext _context;

    public QueueService(WhoAskedContext context)
    {
        this._context = context;
    }

    public async Task<List<Queue>> GetAll()
    {
        if (_context.Queue == null)
            return new List<Queue>();
        return await _context.Queue.ToListAsync();
    }

    public async Task<Queue> Create(string queueName, long ownerId)
    {
        var ret = new Queue
        {
            QueueName = queueName,
            OwnerId = ownerId
        };
        _context.Add(ret);
        await _context.SaveChangesAsync();

        return ret;
    }
}