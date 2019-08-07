using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExplorerSearchBoxDemo {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        private readonly MeowModel model;

        private readonly MouseButtonEventHandler windowWideMouseButtonEventHandler;

        static MainWindow() {
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
            windowWideMouseButtonEventHandler = new MouseButtonEventHandler(OnWindowWideMouseEvent);
            SearchBox.DefaultFocusedElement = ListBox;
        }

        private void OnWindowWideMouseEvent(object sender, MouseButtonEventArgs e) {
            if (!SearchBox.IsMouseOver) SearchBox.DeactivateSearch();
        }

        private void SearchBox_SearchTextFocused(object sender, System.EventArgs e) {
            AddHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);
            if (FocusManager.GetFocusedElement(this) is UIElement element)
                SearchBox.DefaultFocusedElement = element;
        }

        private void SearchBox_SearchTextUnfocused(object sender, System.EventArgs e) {
            RemoveHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);
        }

        private void SearchBox_SearchRequested(object sender, string e) {
            model.FilterText = e;
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            //MessageBox.Show((sender as ListBox).se
            var lb = sender as ListBox;
            //MessageBox.Show("" + FocusManager.GetFocusScope(VisualTreeHelper.GetChild(lb, 0)));
            //lb.ItemContainerGenerator.ContainerFromItem(lb.Get)
            //var a = VisualTreeHelper.GetChild(sender as ListBox, 0);
            //if (a != null) SearchBox.DefaultFocusedElement = a as UIElement;
        }
    }
}
