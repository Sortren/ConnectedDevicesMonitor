using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedDevicesMonitor.Configuration
{
    public class ConnectedDevicesMonitorSettings
    {
        public ScannerConfig ScannerConfig { get; set; }
        public NetworkConfig NetworkConfig { get; set; }
    }
    
    public class ScannerConfig
    {
        public int ScanEveryXMinutes { get; set; }
        public int ScanTimeoutMs { get; set; }

    }

    public class NetworkConfig
    {
        public string IPAddress { get; set; }
        public int CIDRMask { get; set; }
        public string NetworkInterfaceMacAddress { get; set; }
    }
}
