using Avalonia.Controls;
using neXn.Ui.Avalonia;
using Serilog;
using YamAva.Logic;
using YamAva.ViewModels;

namespace YamAva.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        private readonly WindowManager _wm;
        private WindowDragHandler _dragHandler;

        public MainWindow()
        {
            if (!Design.IsDesignMode)
            {
                HelperFunctions.SetTheme(Globals.UserConfig.RuntimeConfiguration.DarkTheme);
            }
            this.InitializeComponent();

            _vm = new MainWindowViewModel
            {
                Instance = this
            };
            this.DataContext = _vm;
            _wm = new(this);
        }

        private void Window_Opened(object sender, System.EventArgs e)
        {
            base.Topmost = Globals.UserConfig.RuntimeConfiguration.TopMost;

            _dragHandler = new(this)
            {
                IsEnabled = true
            };
            _dragHandler.PointerReleased += (s, e) =>
            {
                Globals.UserConfig.RuntimeConfiguration.LastWindowPosition = new(this.Position.X, this.Position.Y);
                Globals.UserConfig.Save();
            };

            _wm.RestoreWindowLocation(Globals.UserConfig.RuntimeConfiguration.LastWindowPosition);

            _wm.HideFromAltTab();
        }

#pragma warning disable S2325 // Suppress "Method can be static" warning since this is an event handler and should not be static
        private void Window_Closing(object sender, WindowClosingEventArgs e)
        {
            Log.CloseAndFlush();
        }
    }
}