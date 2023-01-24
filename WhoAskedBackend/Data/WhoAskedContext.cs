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