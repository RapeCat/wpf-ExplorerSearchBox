using System.Windows;
using System.Windows.Input;

namespace ExplorerSearchBoxDemo {

    public partial class MainWindow : Window {

        // Not necessarily a view model.
        private readonly MeowModel model;

        // Field for click-outsides handling.
        private readonly MouseButtonEventHandler windowWideMouseButtonEventHandler;

        static MainWindow() {
            // Bind for Ctrl + F => Activate search.
            CommandManager.RegisterClassCommandBinding(
                typeof(MainWindow),
                new CommandBinding(
                    new RoutedCommand(
                        "Ctrl + F", typeof(MainWindow),
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

            // Click events in the window will be previewed by
            // function OnWindowWideMouseEvent (defined later)
            // when the handler is on. Now it's off.
            windowWideMouseButtonEventHandler = new MouseButtonEventHandler(OnWindowWideMouseEvent);

            SearchBox.DefaultFocusedElement = ListBox;
        }

        // This would only be on whenever search box is focused.
        private void OnWindowWideMouseEvent(object sender, MouseButtonEventArgs e) {
            // By clicking outsides the search box deactivate the search box.
            if (!SearchBox.IsMouseOver) SearchBox.DeactivateSearch();
        }

        // The two come in pair.
        private void SearchBox_SearchTextFocused(object sender, System.EventArgs e)
            => AddHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);

        private void SearchBox_SearchTextUnfocused(object sender, System.EventArgs e)
            => RemoveHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);

        // Update filter text of the naive model.
        private void SearchBox_SearchRequested(object sender, string e) => model.FilterText = e;
    }
}
