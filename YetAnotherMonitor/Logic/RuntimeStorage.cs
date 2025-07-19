using neXn.Lib.ConfigurationHandler;
using YetAnotherMonitor.Models;

namespace YetAnotherMonitor.Logic
{
    internal static class RuntimeStorage
    {
        public static ConfigurationHandler<Configuration> Configuration { get; set; }
    }
}
