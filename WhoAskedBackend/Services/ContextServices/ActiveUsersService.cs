namespace WhoAskedBackend.Services.ContextServices;

public class ActiveUsersService
{
    private readonly List<string> _activeUsers;

    public ActiveUsersService()
    {
        _activeUsers = new List<string>();
    }

    public void ToggleUserActive(string username, bool active)
    {
        if (active)
        {
            _activeUsers.Add(username);
        }
        else if (!active)
        {
            _activeUsers.Remove(_activeUsers.First(q => q == username));
        }
    }

    public List<string> GetActiveUsers()
    {
        return _activeUsers;
    }
}