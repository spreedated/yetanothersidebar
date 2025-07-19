using neXn.Lib.ConfigurationHandler;
using ServiceLGAgentBattery.Models;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace ServiceLGAgentBattery.Logic
{
    internal static class Globals
    {
        internal static List<Step> StepList { get; } = [];
        internal static ConfigurationHandler<Configuration> ConfigurationHandler { get; set; }
        internal static MemoryMappedFile MemoryMapped { get; set; }
    }
}
