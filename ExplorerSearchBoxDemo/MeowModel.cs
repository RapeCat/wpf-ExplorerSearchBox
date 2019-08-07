using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExplorerSearchBoxDemo {

    internal class MeowModel : INotifyPropertyChanged {

        private string _filterText;

        private string[] allItems;

        public MeowModel() {
            allItems = new string[]{
                "Meow cat", "is a", "cute cat",
                "Meow", "meow", "0.0", "QAQ", "嘤嘤嘤"
            };
            Items = new ObservableCollection<string>();
            RefreshShownList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FilterText {
            get => _filterText; set {
                if (value == null || value == "" || value.Trim() == "") value = null;
                else value = value.ToLowerInvariant();
                string previous = _filterText;
                _filterText = value;
                if (value != previous) {
                    OnPropertyChanged("FilterText");
                    RefreshShownList();
                }
            }
        }

        public ObservableCollection<string> Items { get; set; }

        private void RefreshShownList() {
            Items.Clear();
            foreach (string item in allItems)
                if (_filterText == null || item.Contains(_filterText)) Items.Add(item);
        }

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
