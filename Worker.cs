using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ARPNetworkScannerModule;
using ConnectedDevicesMonitor.DataAccess;
using ConnectedDevicesMonitor.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SharpPcap.LibPcap;
using ConnectedDevicesMonitor.Configuration;

namespace ConnectedDevicesMonitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
   
        private readonly IDbContextFactory<SqliteDbContext> _sqliteDbContext;
        private readonly ConnectedDevicesMonitorSettings _settings;
        
        private LibPcapLiveDevice _networkInterface;
        private Subnet _subnet;

        public Worker(ILogger<Worker> logger, ConnectedDevicesMonitorSettings settings, IDbContextFactory<SqliteDbContext> sqliteDbContext)
        {
            _logger = logger;
            _settings = settings;
            _sqliteDbContext = sqliteDbContext;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int _msTimeout = _settings.ScannerConfig.ScanTimeoutMs;
            int _scanIntervalMs = _settings.ScannerConfig.ScanEveryXMinutes * 60_000;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogWarning("Starts the scan...");

                    Dictionary<string, string> answeredHosts = ARPScanner.Resolve(
                        _subnet,
                        _msTimeout,
                        _networkInterface
                    );

                    HashSet<string> answeredHostsMacAdresses = answeredHosts.Keys.ToHashSet();


                    _logger.LogWarning("Hosts that have answered the ARP Scan:");
                    foreach (var host in answeredHosts)
                    {
                        _logger.LogInformation($"{host.Key} -> {host.Value}");
                    }


                    using (var dbContext = _sqliteDbContext.CreateDbContext())
                    {
                        _logger.LogWarning("Opened db connection");

                        var devicesInDB = dbContext.Devices?.ToHashSet();

                        var devicesInDBMacAddresses = devicesInDB
                                                                .Select(device => device.MACAddress)
                                                                .ToHashSet();

                        var devicesInScanAndDB = devicesInDBMacAddresses
                                                                .Where(device => answeredHostsMacAdresses.Contains(device))
                                                                .ToHashSet();

                        var devicesInDBNotInScan = devicesInDBMacAddresses
                                                                .Where(device => !answeredHostsMacAdresses.Contains(device))
                                                                .ToHashSet();

                        var devicesInScanNotInDB = answeredHostsMacAdresses
                                                                .Where(device => !devicesInDBMacAddresses.Contains(device))
                                                                .ToHashSet();



                        foreach (var deviceMac in devicesInScanAndDB)
                        {
                            var activeDevice = devicesInDB.FirstOrDefault(device => device.MACAddress.Equals(deviceMac));
                            activeDevice.IsActive = true;

                            _logger.LogWarning($"Device >{deviceMac}< has changed state to active");
                        }


                        foreach (var deviceMac in devicesInDBNotInScan)
                        {
                            var inactiveDevice = devicesInDB.FirstOrDefault(device => device.MACAddress.Equals(deviceMac));
                            inactiveDevice.IsActive = false;

                            _logger.LogWarning($"Device >{deviceMac}< has changed state to inactive");
                        }


                        foreach (var deviceMac in devicesInScanNotInDB)
                        {
                            await dbContext.AddAsync(new Device()
                            {
                                IPAddress = answeredHosts[deviceMac],
                                MACAddress = deviceMac,
                                IsActive = true
                            }, stoppingToken);

                            _logger.LogWarning($"New device >{deviceMac}< has been added to the database");
                        }


                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogWarning($"Devices Table has been saved");


                        await dbContext.AddAsync(new Scan()
                        {
                            AnsweredDevicesAmount = answeredHosts.Count,
                            Type = "ARP"
                        }, stoppingToken);

                        _logger.LogInformation($"Added new scan to Scans Table");

                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogWarning($"Scans Table has been saved");

                    }


                    await Task.Delay(_scanIntervalMs, stoppingToken);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unknown error has occured");
                }

                finally
                {
                    _logger.LogInformation("ConnectedDevicesMonitor running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                };
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            string networkInterfaceMac = _settings.NetworkConfig.NetworkInterfaceMacAddress;
            string ipAddress = _settings.NetworkConfig.IPAddress;
            int cidrMask = _settings.NetworkConfig.CIDRMask;

            _logger.LogWarning("Assigned config variables");
            
            using (var dbContext = _sqliteDbContext.CreateDbContext())
            {
                dbContext.Database.EnsureCreatedAsync(cancellationToken);
            };

            _logger.LogWarning("Ensured if the database is properly created");

            _networkInterface = NetworkUtils.SelectNetInterfaceByMac(networkInterfaceMac);
            _logger.LogWarning("Network interface selected by Mac Address");

            _subnet = new Subnet(ipAddress, cidrMask);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ConnectedDevicesMonitor has stoppped runnig");
            return base.StopAsync(cancellationToken);
        }
    }
}
