using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CTableConverter.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Assembly assembly;

        [ObservableProperty]
        private Frm_Main instance;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string selectedFilename;

        [ObservableProperty]
        private bool btnLoadEnabled;

        #region Constructor
        public MainViewModel()
        {
            this.Assembly = typeof(MainViewModel).Assembly;
            this.Title = $"{this.Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title} v{this.Assembly.GetName().Version}";
        }
        #endregion

        [RelayCommand]
        private void BrowseFile()
        {
            if (this.Instance.Ofd_File.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            this.Instance.Txt_Filepath.Text = this.Instance.Ofd_File.FileName;
            this.BtnLoadEnabled = true;
        }

        [RelayCommand]
        private void LoadFile()
        {
            if (!File.Exists(this.SelectedFilename))
            {
                this.BtnLoadEnabled = false;
                return;
            }

            Task.Run(async () =>
            {
                using (FileStream fs = File.Open(this.SelectedFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader r = new(fs))
                    {
                        this.Instance.ViewModelLoadedTable.RawXml = await r.ReadToEndAsync();
                    }
                }

                await this.Instance.ViewModelLoadedTable.Load();
            });
        }
    }
}
