using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.ContextServices;

public class UserService : ModelServiceBase
{
    private readonly WhoAskedContext _context;
    private readonly SecurityService _securityService;
    private readonly QueueService _queueService;
    private readonly UserInQueueService _userInQueueService;

    public UserService([FromServices] WhoAskedContext context, [FromServices] SecurityService securityService,
        UserInQueueService userInQueueService, QueueService queueService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _securityService = securityService;
        _userInQueueService = userInQueueService;
        _queueService = queueService;
    }

    public async Task<User> GetUserByCredentials(string? username, string? password)
    {
        var user = await _context.Users!.FirstOrDefaultAsync(q => q.UserName == username.ToLower())
                   ?? throw CreateException($"User {username} does not exist.");
        if (!_securityService.VerifyPassword(password, user.PasswordHash!))
            throw CreateException($"Credentials are not valid.");

        return user;
    }

    public async Task<User> Create(string? username, string? password)
    {
        EnsureNotNull(username, nameof(username));
        EnsureNotNull(password, nameof(password));

        username = username.ToLower();

        if (_context.Users!.Any(q => q.UserName == username))
            throw CreateException($"User {username} already exists.", null);
        var hash = _securityService.HashPassword(password);
        var ret = new User {UserName = username, PasswordHash = hash, Avatar = "user"};

        _context.Users!.Add(ret);
        await _context.SaveChangesAsync();

        await CreateAll(ret);

        return ret;
    }

    public async Task CreateAll(User user)
    {
        var queue1 = await _queueService.Create(user.UserName + "'s chat 1", user.UserId);
        var queue2 = await _queueService.Create(user.UserName + "'s chat 2", user.UserId);

        Queue global;
        if (user.UserName == "admin")
        {
            global = await _queueService.Create("Global chat", user.UserId);
        }
        else
        {
            global = await _context.Queue!.FirstAsync(q => q.QueueName == "Global chat");
        }

        await _userInQueueService.Create(user.UserId, global.QueueId);
        await _userInQueueService.Create(user.UserId, queue1.QueueId);
        await _userInQueueService.Create(user.UserId, queue2.QueueId);
    }

    public async Task<User> GetByUsername(string username)
    {
        return (await GetIncluded()).FirstOrDefault(q => q.UserName == username) ??
               throw CreateException($"User with id {username} does not exist", null);
    }

    public async Task<List<User>> GetAll()
    {
        return await GetIncluded();
    }

    private async Task<List<User>> GetIncluded()
    {
        if (this._context.Users == null)
        {
            return new List<User>();
        }

        return await this._context.Users.Include(q => q.Queues).ThenInclude(q => q.Queue).ToListAsync();
    }

    public async Task SetAvatarByUsername(string? avatarUserName, string? avatarAvatarName)
    {
        (await (this._context.Users ?? throw new InvalidOperationException()).FirstAsync(q =>
            q.UserName == avatarUserName)).Avatar = avatarAvatarName;
        await _context.SaveChangesAsync();
    }
}