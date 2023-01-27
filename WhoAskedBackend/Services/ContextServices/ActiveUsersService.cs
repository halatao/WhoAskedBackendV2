using WhoAskedBackend.Data;

namespace WhoAskedBackend.Services.ContextServices;

public class ActiveUsersService
{
    private readonly List<string> _activeUsers;
    private readonly WhoAskedContext _context;

    public ActiveUsersService(WhoAskedContext context)
    {
        _context = context;
        _activeUsers = new List<string>();
    }

    public void ToggleUserActive(string username, bool active)
    {
        if (active)
        {
            try
            {
                if (_activeUsers.Contains(username) || !CheckExistence(username))
                {
                    return;
                }

                _activeUsers.Add(username);
            }
            catch (Exception e)
            {
                return;
            }
        }
        else if (!active)
        {
            try
            {
                if (!_activeUsers.Contains(username) || !CheckExistence(username))
                {
                    return;
                }

                _activeUsers.Remove(_activeUsers.First(q => q == username));
            }
            catch (Exception e)
            {
                return;
            }
        }

        bool CheckExistence(string username)
        {
            var list = _context.Users!.Where(q => q.UserName == username).ToList();
            return list.Count > 0;
        }
    }

    public List<string> GetActiveUsers()
    {
        return _activeUsers;
    }
}