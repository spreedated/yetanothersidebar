using System.Windows;
using System.Windows.Controls;
using static Services.Models.Responses.GasstationPrices;

namespace YetAnotherMonitor.ViewEelements
{
    public partial class GasPriceBox : UserControl
    {
        public static readonly DependencyProperty StationnameProperty = DependencyProperty.Register("Stationname", typeof(string), typeof(GasPriceBox));

        public string Stationname
        {
            get => (string)base.GetValue(StationnameProperty);
            set => base.SetValue(StationnameProperty, value);
        }

        public static readonly DependencyProperty GaspriceProperty = DependencyProperty.Register("Gasprice", typeof(GasPrice), typeof(GasPriceBox));

        public GasPrice Gasprice
        {
            get => (GasPrice)base.GetValue(GaspriceProperty);
            set => base.SetValue(GaspriceProperty, value);
        }

        public static readonly DependencyProperty ShowE5Property = DependencyProperty.Register("ShowE5", typeof(bool), typeof(GasPriceBox), new UIPropertyMetadata(true));

        public bool ShowE5
        {
            get => (bool)base.GetValue(ShowE5Property);
            set => base.SetValue(ShowE5Property, value);
        }

        public static readonly DependencyProperty ShowE10Property = DependencyProperty.Register("ShowE10", typeof(bool), typeof(GasPriceBox), new UIPropertyMetadata(true));

        public bool ShowE10
        {
            get => (bool)base.GetValue(ShowE10Property);
            set => base.SetValue(ShowE10Property, value);
        }

        public static readonly DependencyProperty ShowDieselProperty = DependencyProperty.Register("ShowDiesel", typeof(bool), typeof(GasPriceBox), new UIPropertyMetadata(true));

        public bool ShowDiesel
        {
            get => (bool)base.GetValue(ShowDieselProperty);
            set => base.SetValue(ShowDieselProperty, value);
        }

        public GasPriceBox()
        {
            this.InitializeComponent();
        }
    }
}
