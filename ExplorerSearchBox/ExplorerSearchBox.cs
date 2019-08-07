using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ExplorerSearchBox {
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ExplorerSearchBox"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ExplorerSearchBox;assembly=ExplorerSearchBox"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    /// 

    // As https://www.wpftutorial.net/CustomVsUserControl.html said
    // and
    // https://github.com/fluentribbon/Fluent.Ribbon/blob/develop/Fluent.Ribbon/Controls/Ribbon.cs
    // did.
    [TemplatePart(Name = PartRootPanelName, Type = typeof(Panel))]
    [TemplatePart(Name = PartTextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = PartSearchIconName, Type = typeof(Button))]
    [TemplatePart(Name = PartCloseButtonName, Type = typeof(Button))]
    public class ExplorerSearchBox : ContentControl {

        private const string PartRootPanelName = "PART_RootPanel";
        private const string PartTextBoxName = "PART_TextBox";
        private const string PartSearchIconName = "PART_SearchIcon";
        private const string PartCloseButtonName = "PART_CloseButton";

        public static readonly RoutedCommand ActivateSearchCommand = new RoutedCommand();
        public static readonly RoutedCommand CancelSearchCommand = new RoutedCommand();

        public static readonly DependencyProperty DefaultFocusedElementProperty;

        static ExplorerSearchBox() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExplorerSearchBox),
                new FrameworkPropertyMetadata(typeof(ExplorerSearchBox)));

            // Using of CommandManager.
            // https://www.codeproject.com/Articles/43295/ZoomBoxPanel-add-custom-commands-to-a-WPF-control
            CommandManager.RegisterClassCommandBinding(typeof(ExplorerSearchBox),
                new CommandBinding(ActivateSearchCommand, ActivateSearchCommand_Invoke, (s, e) => e.CanExecute = true));
            CommandManager.RegisterClassCommandBinding(typeof(ExplorerSearchBox),
                new CommandBinding(CancelSearchCommand, CancelSearchCommand_Invoke));

            DefaultFocusedElementProperty = DependencyProperty.Register(
                nameof(DefaultFocusedElement), typeof(UIElement), typeof(ExplorerSearchBox));

        }

        private static void ActivateSearchCommand_Invoke(object sender, ExecutedRoutedEventArgs e) {
            if (sender is ExplorerSearchBox self)
                self.ActivateSearch();
        }

        private static void CancelSearchCommand_Invoke(object sender, ExecutedRoutedEventArgs e) {
            if (sender is ExplorerSearchBox self) {
                self.textBox.Text = "";
                self.CancelPreviousSearchFilterUpdateTask();
                self.UpdateFilterText();
                self.DeactivateSearch();
            }
        }

        // Using of events.
        // https://stackoverflow.com/questions/13447940/how-to-create-user-define-new-event-for-user-control-in-wpf-one-small-example
        public event EventHandler SearchTextFocused;
        public event EventHandler SearchTextUnfocused;
        // Parameter passing:
        // https://stackoverflow.com/questions/4254636/how-to-create-a-custom-event-handling-class-like-eventargs
        public event EventHandler<string> SearchRequested;

        private Panel rootPanel;
        private TextBox textBox;
        private Button searchIcon;
        private Button closeButton;

        private CancellationTokenSource waitingSearchUpdateTaskCancellationTokenSource;

        public UIElement DefaultFocusedElement {
            get => (UIElement)GetValue(DefaultFocusedElementProperty);
            set => SetValue(DefaultFocusedElementProperty, value);
        }

        public void ActivateSearch() {
            textBox?.Focus();
        }

        public void DeactivateSearch() {
            // Use keyboard focus instead.
            // https://stackoverflow.com/questions/2914495/wpf-how-to-programmatically-remove-focus-from-a-textbox
            //Keyboard.ClearFocus();
            if (DefaultFocusedElement != null) {
                UIElement focusee = null;
                if (DefaultFocusedElement is Selector list)
                    focusee = list.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                if (focusee == null) focusee = DefaultFocusedElement;
                Keyboard.Focus(focusee);
            } else {
                rootPanel.Focusable = true;
                Keyboard.Focus(rootPanel);
                rootPanel.Focusable = false;
            }
        }

        public void OnTextBox_GotFocus(object sender, RoutedEventArgs e) {
            SearchTextFocused?.Invoke(this, e);
        }

        public void OnTextBox_LostFocus(object sender, RoutedEventArgs e) {
            SearchTextUnfocused?.Invoke(this, e);
        }

        private void OnTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                CancelSearchCommand.Execute(null, this);
            else if (e.Key == Key.Enter) {
                CancelPreviousSearchFilterUpdateTask();
                UpdateFilterText();
            } else {
                CancelPreviousSearchFilterUpdateTask();
                // Delayed task.
                // https://stackoverflow.com/questions/15599884/how-to-put-delay-before-doing-an-operation-in-wpf
                waitingSearchUpdateTaskCancellationTokenSource = new CancellationTokenSource();
                Task.Delay(1000, waitingSearchUpdateTaskCancellationTokenSource.Token)
                   .ContinueWith(self => {
                       if (!self.IsCanceled) Dispatcher.Invoke(() => UpdateFilterText());
                   });
            }
        }

        private void CancelPreviousSearchFilterUpdateTask() {
            if (waitingSearchUpdateTaskCancellationTokenSource != null) {
                waitingSearchUpdateTaskCancellationTokenSource.Cancel();
                waitingSearchUpdateTaskCancellationTokenSource.Dispose();
                waitingSearchUpdateTaskCancellationTokenSource = null;
            }
        }

        private void UpdateFilterText() {
            var args = new RoutedEventArgs();
            SearchRequested?.Invoke(this, textBox.Text);
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // Idea of detaching.
            // https://www.jeff.wilcox.name/2010/04/template-part-tips/

            if (textBox != null) {
                textBox.GotKeyboardFocus -= OnTextBox_GotFocus;
                textBox.LostKeyboardFocus -= OnTextBox_LostFocus;
                textBox.KeyUp -= OnTextBox_KeyUp;
            }

            rootPanel = GetTemplateChild(PartRootPanelName) as Panel;
            textBox = GetTemplateChild(PartTextBoxName) as TextBox;
            searchIcon = GetTemplateChild(PartSearchIconName) as Button;
            closeButton = GetTemplateChild(PartCloseButtonName) as Button;

            if (textBox != null) {
                textBox.GotKeyboardFocus += OnTextBox_GotFocus;
                textBox.LostKeyboardFocus += OnTextBox_LostFocus;
                textBox.KeyUp += OnTextBox_KeyUp;
            }
        }
    }
}
