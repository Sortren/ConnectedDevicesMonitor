using ConnectedDevicesMonitor.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ConnectedDevicesMonitor.DataAccess
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Scan> Scans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=ConnectedDevicesMonitorDatabase.db", option =>
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }
    }
}
