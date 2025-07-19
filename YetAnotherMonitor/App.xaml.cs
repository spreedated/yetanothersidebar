using System.IO;
using System.Windows;
using YetAnotherMonitor.Handlers;
using YetAnotherMonitor.Logic;

namespace YetAnotherMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public LoggerHandler Logger { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Logger = new();
            this.Logger.Start();

            RuntimeStorage.Configuration = new(new(Path.Combine(System.AppContext.BaseDirectory, "config.json")));
            RuntimeStorage.Configuration.Load();
        }

        public void StopLogger()
        {
            if (this.Logger != null && this.Logger.IsRunning)
            {
                this.Logger.Stop();
            }
        }
    }
}
