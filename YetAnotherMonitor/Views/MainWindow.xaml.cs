using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using YetAnotherMonitor.Logic;
using YetAnotherMonitor.ViewModels;

namespace YetAnotherMonitor.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ((MainWindowViewModel)this.DataContext).Instance = this;

            ContextMenu s = new();
            MenuItem mm = new() { Header = "Exit" };
            mm.Click += (o, e) => { this.Close(); };
            s.Items.Add(mm);

            ((MainWindowViewModel)this.DataContext).ContextMenuTaskbar = s;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
                if (RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation == null)
                {
                    RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation = new();
                }

                RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation.X = (int)this.Left;
                RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation.Y = (int)this.Top;

                RuntimeStorage.Configuration.Save();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext)?.DisposeNotifyIcon();
        }

        #region Remove from Alt+Tab Menu
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation != null)
            {
                this.Left = RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation.X;
                this.Top = RuntimeStorage.Configuration.RuntimeConfiguration.WindowStartupLocation.Y;
            }
            else
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            _ = SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
        }
    }
}
