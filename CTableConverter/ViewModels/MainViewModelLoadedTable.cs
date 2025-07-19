using CommunityToolkit.Mvvm.ComponentModel;
using CTableConverter.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CTableConverter.ViewModels
{
    internal partial class MainViewModelLoadedTable : ObservableObject
    {
        private XmlDocument doc;
        private ListView lsv;

        [ObservableProperty]
        private string rawXml;

        [ObservableProperty]
        private string version;

        [ObservableProperty]
        private Control boundControl;

        [ObservableProperty]
        private ObservableCollection<CheatEntry> entries;

        [ObservableProperty]
        private string entryCount;

        public MainViewModelLoadedTable(ListView lsv)
        {
            this.lsv = lsv;
        }

        public async Task Load()
        {
            this.doc = new();
            this.doc.LoadXml(this.RawXml);

            this.BoundControl.Invoke(() =>
            {
                this.Version = $"Version: {this.doc.SelectSingleNode("/CheatTable").Attributes.OfType<XmlAttribute>().FirstOrDefault(x => x.Name == "CheatEngineTableVersion")?.Value}";
            });

            List<CheatEntry> entryList = [];

            await Task.Run(() =>
            {
                foreach (XmlNode node in this.doc.SelectNodes("/CheatTable/CheatEntries/CheatEntry").OfType<XmlNode>().Where(x => x.SelectSingleNode("Description").InnerText.Contains("pointerscan result")))
                {
                    string a = node.SelectSingleNode("Address").InnerText;

                    entryList.Add(new()
                    {
                        Address = a[(a.IndexOf('+') + 1)..],
                        Offsets = node.SelectSingleNode("Offsets").OfType<XmlNode>().Select(x => x.InnerText).Reverse().ToArray(),
                    });
                }
            });

            this.BoundControl.Invoke(() =>
            {
                this.Entries = new(entryList);
                this.EntryCount = $"Entries: {this.Entries.Count}";

                this.lsv.Items.Clear();
                this.lsv.Columns.Clear();

                this.lsv.Columns.Add($"Entry");
                this.lsv.Columns.Add($"Base address");

                for (int i = 0; i < this.Entries.Max(x => x.Offsets.Length); i++)
                {
                    this.lsv.Columns.Add($"Offset {i}");
                }

                int c = 0;

                foreach (CheatEntry ce in this.Entries)
                {
                    ListViewItem lvi = new($"{c}");
                    lvi.SubItems.Add(ce.Address);
                    lvi.SubItems.AddRange(ce.Offsets);

                    this.lsv.Items.Add(lvi);
                    c++;
                }

                this.lsv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            });
        }
    }
}
