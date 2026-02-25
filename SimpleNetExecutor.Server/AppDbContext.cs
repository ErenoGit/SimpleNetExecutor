using Microsoft.EntityFrameworkCore;
using SimpleNetExecutor.Server;

public class AppDbContext : DbContext
{
    public DbSet<SimpleNetExecutor.Server.Endpoint> Endpoints { get; set; }
    public DbSet<Module> Modules { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}