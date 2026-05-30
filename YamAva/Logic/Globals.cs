using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neXn.Lib.ConfigurationHandler;
using System.Reflection;
using YamAva.Models;

namespace YamAva.Logic
{
    internal static class Globals
    {
        public static string AppLocalBaseUserPath { get; set; }
        public static Assembly Assembly { get; } = typeof(Globals).Assembly;
        public static ConfigurationHandler<Configuration> UserConfig { get; set; }
        public static IHost BackgroundServices { get; set; }
        public static ServiceDescriptor[] ServiceDescriptors { get; set; }
    }
}
