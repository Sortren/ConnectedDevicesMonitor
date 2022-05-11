using System;

namespace ConnectedDevicesMonitor.DatabaseModels
{
    public class Device
    {
        
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string AliasName { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastTimeActive { get; set; }

        public Device()
        {
            LastTimeActive = DateTime.UtcNow;
        }
    }
}
