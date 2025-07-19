using Serilog;
using Serilog.Events;

namespace YetAnotherMonitor.Handlers
{
    public class LoggerHandler
    {
        public bool IsRunning { get; private set; }

#if DEBUG
        private static readonly LogEventLevel level = LogEventLevel.Verbose;
#else
        private static readonly LogEventLevel level = LogEventLevel.Information;
#endif

        public void Start()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Debug(restrictedToMinimumLevel: level)
                .CreateLogger();

            Log.Debug("Logger initialized");
            this.IsRunning = true;
        }

        public void Stop()
        {
            Log.Debug("Logger stopped");
            Log.CloseAndFlush();
            this.IsRunning = false;
        }
    }
}
