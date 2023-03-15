using Elektrifikatsiya.Models;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Database;

public class MainDatabaseContext : DbContext
{
    public MainDatabaseContext(DbContextOptions<MainDatabaseContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
}