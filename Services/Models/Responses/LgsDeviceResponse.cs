using System;
using System.Collections.Generic;

namespace Services.Models.Responses
{
    public class LgsDeviceResponse
    {
        public List<LogitechDevice> Devices { get; set; } = [];
        public DateTime Date { get; set; }
    }
}
