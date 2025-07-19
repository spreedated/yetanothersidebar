using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace YetAnotherMonitor.Views.Pages
{
    [ObservableObject]
    public partial class ConversionPage : Page
    {
        public enum ConversionPageAlgorithm
        {
            Ascii,
            Unicode,
            Base64,
            UrlEncode
        }

        private readonly ConversionPageAlgorithm algorithm;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EncodeCommand))]
        private string inputValue;

        [ObservableProperty]
        private string outputValue;

        #region Ctor
        public ConversionPage(ConversionPageAlgorithm algorithm)
        {
            this.algorithm = algorithm;
            this.InitializeComponent();
            this.DataContext = this;
        }
        #endregion

        [RelayCommand(CanExecute = nameof(CanExecuteEncode))]
        private void Encode()
        {
            switch (this.algorithm)
            {
                case ConversionPageAlgorithm.Ascii:
                    this.OutputValue = ConversionTools.Ascii.Encode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.Unicode:
                    this.OutputValue = ConversionTools.Unicode.Encode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.Base64:
                    this.OutputValue = ConversionTools.Base64.Encode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.UrlEncode:
                    this.OutputValue = ConversionTools.UrlEncode.Encode(this.InputValue);
                    break;
                default:
                    break;
            }
        }

        private bool CanExecuteEncode()
        {
            return !string.IsNullOrEmpty(this.InputValue);
        }

        [RelayCommand]
        private void Decode()
        {
            switch (this.algorithm)
            {
                case ConversionPageAlgorithm.Ascii:
                    this.OutputValue = ConversionTools.Ascii.Decode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.Unicode:
                    this.OutputValue = ConversionTools.Unicode.Decode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.Base64:
                    this.OutputValue = ConversionTools.Base64.Decode(this.InputValue);
                    break;
                case ConversionPageAlgorithm.UrlEncode:
                    this.OutputValue = ConversionTools.UrlEncode.Decode(this.InputValue);
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private void Swap()
        {
            (this.OutputValue, this.InputValue) = (this.InputValue, this.OutputValue);
        }

        [RelayCommand]
        private void CopyToClipboard()
        {
            Clipboard.SetText(this.OutputValue);
        }
    }
}
