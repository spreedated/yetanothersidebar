using System.Windows;
using System.Windows.Input;
using YetAnotherMonitor.ViewModels;

namespace YetAnotherMonitor.Views
{
    public partial class Conversions : Window
    {
        #region Ctor
        public Conversions()
        {
            this.InitializeComponent();
            ((ConversionsViewModel)this.DataContext).Instance = this;
        }
        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
