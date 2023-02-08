using Elektrifikatsiya.Models;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Database;

public class UserDatabaseContext : DbContext
{
    public UserDatabaseContext(DbContextOptions<UserDatabaseContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }
}