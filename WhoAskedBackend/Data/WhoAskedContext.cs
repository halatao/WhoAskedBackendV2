﻿using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Model;

namespace WhoAskedBackend.Data;

public class WhoAskedContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<User>? Users { get; set; }
    public DbSet<Queue>? Queue { get; set; }
    public DbSet<UserInQueue>? UserInQueue { get; set; }

    public WhoAskedContext(IConfiguration configuration)
    {
        _configuration = configuration;
        //base.Database.EnsureDeleted();
        //base.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInQueue>(entity =>
        {
            entity.HasKey(sc => sc.UserInQueueId);
            entity.Property(sc => sc.UserInQueueId).ValueGeneratedOnAdd();

            entity
                .HasOne<User>(uq => uq.User)
                .WithMany(u => u.Queues)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity
                .HasOne<Queue>(uq => uq.Queue)
                .WithMany(q => q.Users)
                .HasForeignKey(uq => uq.QueueId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(q => q.UserId);
            entity.Property(q => q.UserId)
                .ValueGeneratedOnAdd();

            entity.Property(q => q.UserName)
                .IsRequired();

            entity.Property(q => q.PasswordHash)
                .IsRequired();

            entity
                .HasMany(u => u.OwnedQueues)
                .WithOne(q => q.Owner)
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.Property(q => q.Avatar);
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity.HasKey(q => q.QueueId);
            entity.Property(q => q.QueueId).ValueGeneratedOnAdd();

            entity.Property(q => q.QueueName).IsRequired();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("WhoAskedDb"));
    }
}