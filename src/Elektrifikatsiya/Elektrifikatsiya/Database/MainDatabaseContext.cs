using Elektrifikatsiya.Models;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Database;

public class MainDatabaseContext : DbContext
{
	public DbSet<Device> Devices { get; set; }

	public DbSet<User> Users { get; set; }

	public DbSet<EnergyPriceChange> EnergyPriceChanges { get; set; }

	public MainDatabaseContext(DbContextOptions<MainDatabaseContext> dbContextOptions) : base(dbContextOptions)
	{
	}
}