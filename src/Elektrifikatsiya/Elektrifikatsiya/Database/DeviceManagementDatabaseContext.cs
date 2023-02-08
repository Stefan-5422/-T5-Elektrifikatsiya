using Elektrifikatsiya.Models;
using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Database
{
    public class DeviceManagmentDatabaseContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
    }
}
