using System;

namespace ServiceLGAgentBattery.Models
{
    public class LogitechBatteryResponse
    {
        public float G915_Battery { get; set; }
        public float G935_Battery { get; set; }
        public DateTime Date { get; set; }
    }
}
