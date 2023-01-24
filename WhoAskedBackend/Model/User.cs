using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkIT_Backend.Model;

public class User
{
    public User()
    {
    }

    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string Role { get; } = "user";
}