using Elektrifikatsiya.Models;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Database;

public class UserDatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
}