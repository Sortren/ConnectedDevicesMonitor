using System;


namespace ConnectedDevicesMonitor.DatabaseModels
{
    public class Scan
    {
        public int Id { get; set; }
        public int AnsweredDevicesAmount { get; set; }
        public DateTime RanAt { get; set; }
        public string Type { get; set; }

        public Scan()
        {
            RanAt = DateTime.UtcNow;
        }
    }
}
