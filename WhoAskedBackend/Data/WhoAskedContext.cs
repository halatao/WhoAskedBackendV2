using Microsoft.EntityFrameworkCore;
using WorkIT_Backend.Model;

namespace WhoAskedBackend.Data;

public class WhoAskedContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<User>? Users { get; set; }

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
            entity.HasKey(sc => new {sc.UserId, sc.QueueId});

            entity
                .HasOne<User>(uq => uq.User)
                .WithMany(u => u.Queues)
                .HasForeignKey(u => u.QueueId)
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
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("WhoAskedDb"));
    }
}