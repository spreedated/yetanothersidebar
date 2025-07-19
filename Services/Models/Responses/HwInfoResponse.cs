using HwHash.Models;
using System;
using System.Collections.Generic;

namespace Services.Models.Responses
{
    public class HwInfoResponse
    {
        public List<HwInfoHash> Telemetric { get; set; }
        public DateTime Date { get; set; }
    }
}
