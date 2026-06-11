using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Services.Workers;
using System;
using System.IO;
using System.Threading.Tasks;
using YamAva.Logic;

namespace YamAva
{
    internal static class Program
    {
        private readonly static LogEventLevel minimumLevel = LogEventLevel.Verbose;

        [STAThread]
        public static void Main(string[] args)
        {
            if (OperatingSystem.IsWindows())
            {
                Globals.AppLocalBaseUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neXn-Systems", "YamAva");
            }
            else
            {
                Globals.AppLocalBaseUserPath = AppContext.BaseDirectory;
            }

            // Setup logger
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: minimumLevel)
            .WriteTo.Debug()
            .Enrich.WithProperty("Application", Globals.Assembly.GetName().Name)
            .CreateLogger();

            Microsoft.Extensions.Logging.ILogger logger = new SerilogLoggerProvider().CreateLogger("app");

            logger.LogInformation("Starting up");

            Globals.UserConfig = new(new(Path.Combine(Globals.AppLocalBaseUserPath, "config.json"))
            {
                Autoload = false
            }, new SerilogLoggerProvider().CreateLogger("UserConfig"));
            Globals.UserConfig.LoadAsync().Wait();
            logger.LogInformation("Loaded user config");

            // Worker Service
            Task.Run(async () =>
            {
                HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
                builder.Services.AddSingleton(sp =>
                {
                    return new InstalledSoftwareWorker(new SerilogLoggerProvider().CreateLogger("installed_software"), Globals.AppLocalBaseUserPath);
                });
                builder.Services.AddSingleton(sp =>
                {
                    return new WeatherWorker(new SerilogLoggerProvider().CreateLogger("weather"), Globals.UserConfig.RuntimeConfiguration.WeatherApiComApiKey);
                });
                builder.Services.AddSingleton<BlenderWorker>();
                builder.Services.AddSingleton<GodotWorker>();
                builder.Services.AddSingleton(sp =>
                {
                    return new FpvSoftwareWorker(new SerilogLoggerProvider().CreateLogger("fpv_software"));
                });
                builder.Services.AddSingleton(sp =>
                {
                    return new LgDeviceWorker(new SerilogLoggerProvider().CreateLogger("lg_device"));
                });
                builder.Services.AddSingleton(sp =>
                {
                    return new UniFiWorker(new SerilogLoggerProvider().CreateLogger("unifi"), new()
                    {
                        Username = Globals.UserConfig.RuntimeConfiguration.UniFiUsername,
                        Password = Globals.UserConfig.RuntimeConfiguration.UniFiPassword
                    }, Globals.UserConfig.RuntimeConfiguration.UniFiSslThumbprints);
                });

                builder.Services.AddHostedService(sp => sp.GetRequiredService<InstalledSoftwareWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<GodotWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<BlenderWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<LgDeviceWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<WeatherWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<FpvSoftwareWorker>());
                builder.Services.AddHostedService(sp => sp.GetRequiredService<UniFiWorker>());

                Globals.ServiceDescriptors = [.. builder.Services];

                Globals.BackgroundServices = builder.Build();

                logger.LogInformation("Built worker services");
            });
            // # ### #

            logger.LogTrace("Loading/building app...");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                        .UsePlatformDetect()
                        .WithInterFont()
                        .LogToTrace();
        }
    }
}
