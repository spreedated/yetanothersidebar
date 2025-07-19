using CTableConverter.ViewModels;
using System.Windows.Forms;

namespace CTableConverter
{
    public partial class Frm_Main : Form
    {
        private MainViewModel ViewModel => (MainViewModel)this.DataContext;
        internal MainViewModelLoadedTable ViewModelLoadedTable => (MainViewModelLoadedTable)this.Grp_LoadedTable.DataContext;

        public Frm_Main()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel()
            {
                Instance = this
            };
            this.Grp_LoadedTable.DataContext = new MainViewModelLoadedTable(this.Lsv_Data)
            {
                BoundControl = this.Grp_LoadedTable
            };

            this.DataBindings.Add("Text", this.ViewModel, nameof(this.ViewModel.Title), false, DataSourceUpdateMode.OnPropertyChanged);
            this.Txt_Filepath.DataBindings.Add("Text", this.ViewModel, nameof(this.ViewModel.SelectedFilename), false, DataSourceUpdateMode.OnPropertyChanged);
            this.Btn_Load.DataBindings.Add("Enabled", this.ViewModel, nameof(this.ViewModel.BtnLoadEnabled), false, DataSourceUpdateMode.OnPropertyChanged);

            this.Btn_Browse.Command = this.ViewModel.BrowseFileCommand;
            this.Btn_Load.Command = this.ViewModel.LoadFileCommand;

            this.Lbl_Version.DataBindings.Add("Text", this.ViewModelLoadedTable, nameof(this.ViewModelLoadedTable.Version), false, DataSourceUpdateMode.OnPropertyChanged);
            this.Lbl_Rows.DataBindings.Add("Text", this.ViewModelLoadedTable, nameof(this.ViewModelLoadedTable.EntryCount), false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
