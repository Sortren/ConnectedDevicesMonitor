using ARPNetworkScannerModule;
using ConnectedDevicesMonitor.Configuration;
using ConnectedDevicesMonitor.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectedDevicesMonitor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = hostContext.Configuration
                                    .GetSection("ConnectedDevicesMonitorSettings")
                                    .Get<ConnectedDevicesMonitorSettings>();

                    services.AddSingleton(settings);

                    services.AddDbContextFactory<SqliteDbContext>();
                    services.AddHostedService<Worker>();
                });
    }
}