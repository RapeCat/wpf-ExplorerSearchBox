using System.Windows;
using System.Windows.Input;

namespace ExplorerSearchBoxDemo {

    public partial class MainWindow : Window {

        // Not necessarily a view model.
        private readonly MeowModel model;

        static MainWindow() {
            // Bind for Ctrl + F => Activate search.
            CommandManager.RegisterClassCommandBinding(
                typeof(MainWindow),
                new CommandBinding(
                    new RoutedCommand(
                        "Find", typeof(MainWindow),
                        new InputGestureCollection(){
                            new KeyGesture(Key.F, ModifierKeys.Control)
                        }
                    ),
                    (a, b) => (a as MainWindow)?.SearchBox.ActivateSearch()
                )
            );
        }

        public MainWindow() {

            InitializeComponent();

            model = new MeowModel();
            DataContext = model;

            //SearchBox.DefaultFocusedElement = ListBox;

        }

        // Update filter text of the naive model.
        private void SearchBox_SearchRequested(object sender, string e) => model.FilterText = e;
    }
}
