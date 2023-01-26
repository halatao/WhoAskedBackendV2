using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Services.ContextServices;

public class UserService : ModelServiceBase
{
    private readonly WhoAskedContext _context;
    private readonly SecurityService _securityService;

    public UserService([FromServices] WhoAskedContext context, [FromServices] SecurityService securityService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _securityService = securityService;
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
        var ret = new User {UserName = username, PasswordHash = hash, Avatar = ""};

        _context.Users!.Add(ret);
        await _context.SaveChangesAsync();

        return ret;
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