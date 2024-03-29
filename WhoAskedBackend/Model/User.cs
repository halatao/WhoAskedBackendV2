﻿namespace WhoAskedBackend.Model;

public class User
{
    public User()
    {
    }

    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string? Avatar { get; set; }

    public string Role { get; } = "user";

    public ICollection<UserInQueue> Queues { get; set; }
    public ICollection<Queue> OwnedQueues { get; set; }
}