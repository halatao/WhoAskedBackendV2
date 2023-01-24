using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WorkIT_Backend.Model;

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
}