using Microsoft.EntityFrameworkCore;

namespace GameServer.Db;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TimePassed> TimePassed { get; set; }
}