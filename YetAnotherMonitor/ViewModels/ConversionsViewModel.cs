using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using YetAnotherMonitor.Views.Pages;

namespace YetAnotherMonitor.ViewModels
{
    internal partial class ConversionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Window instance;

        [ObservableProperty]
        private Page asciiPage = new ConversionPage(ConversionPage.ConversionPageAlgorithm.Ascii);

        [ObservableProperty]
        private Page base64Page = new ConversionPage(ConversionPage.ConversionPageAlgorithm.Base64);

        [ObservableProperty]
        private Page unicodePage = new ConversionPage(ConversionPage.ConversionPageAlgorithm.Unicode);

        [ObservableProperty]
        private Page urlEncodePage = new ConversionPage(ConversionPage.ConversionPageAlgorithm.UrlEncode);

        [RelayCommand]
        private void Exit()
        {
            this.Instance?.Close();
        }
    }
}
