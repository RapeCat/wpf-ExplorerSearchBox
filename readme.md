# ExplorerSearchBox

A WPF (Windows Presentation Foundation)
custom search box like the Windows File Explorer's.
Along with a demo.

* Idle:

  ![screenshot_idle](./arts/idle.png)

  (Make sure your font has the search icon.)

* Active:

  ![screenshot_idle](./arts/active.png)

  (Make sure your font has the close icon.)

## Usage

The repository contains a *Visual Studio* solution
consisting of a library project and a showcase application
project. *Visual Studio 2017* or higher is preferred.

1. In your WPF window application project,
add a reference to library project
`ExplorerSearchBox`.
2. Add namespace reference in your `Window`'s
`xaml` tag:

   ```xaml
   <Window
       ......
   +   xmlns:some_name="clr-namespace:ExplorerSearchBox;assembly=ExplorerSearchBox"
       ......>
   ```
3. Add the search box to your window *and* set it a focus scope,
so the previous focus of your window can be restored after the search:

   ```xaml
       ......
   +   <some_name:ExplorerSearchBox x:Name="Whatever"
   +       FocusManager.IsFocusScope="True" ... />
   ```
4. Subscribe to focused and unfocused event, to execute
relevant code, e.g. add & remove click outside handler.
They're just shortcut to the contained `TextBox`'s
corresponding *keyboard focus* events.

   In *.xaml:

   ```xaml
       ......
       <some_name:ExplorerSearchBox ......
   +       SearchTextFocused="Whatever_SearchTextFocused"
   +       SearchTextUnfocused="Whatever_SearchTextUnfocused"
          ...... />
   ```
   In *.xaml.cs:
   ```cs
   ......
   // Field for click-outsides handling.
   private readonly MouseButtonEventHandler windowWideMouseButtonEventHandler;
   ```
   Then
   ```cs
   // Initialize the handler;
   // and setup default focus to "restore".
   public MainWindow() {
   ......
       // Feed it the handling function:
   +   windowWideMouseButtonEventHandler = new MouseButtonEventHandler(OnWindowWideMouseEvent);
       // If it's a `Selector` incl `ListBox` it's
       // Focused child then first child is preferred:
   +   Whatever.DefaultFocusedElement = YourListBox;    
   ```
   And
   ```cs
   // Define the handling function:
   private void OnWindowWideMouseEvent(object sender, MouseButtonEventArgs e) {
       // Deactivate search when click outsides:
       if (!Whatever.IsMouseOver) Whatever.DeactivateSearch();
   }
   ```
   Also
   ```cs
   // Only subscribe to window-wide click handler
   // whenever need to detect click outsides something:
   private void Whatever_SearchTextFocused(object sender, System.EventArgs e)
       => AddHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);
   
   // And remember to unsubscribe~
   // Focus & Unfocus would always be in pair:
   private void Whatever_SearchTextUnfocused(object sender, System.EventArgs e)
       => RemoveHandler(Window.PreviewMouseDownEvent, windowWideMouseButtonEventHandler);
   ```
5. Subscribe to text change event.

   ```xaml
       ......
       <some_name:ExplorerSearchBox ......
   +       SearchRequested="..."
          ...... />
   ```
   While user types the filter text would change after
   a certain delay. For instance the user typed "meoow",
   paused and found a typo, backspace x2 and typed "w",
   would trigger this event twice with text "meoow"
   and "meow". Another pause and typed "\~" would trigger
   the event with "meow\~".
6. You can add key binding e.g. Ctrl + F using `commandbinding`.
You can also extend the poor functionality of this poor
custom control which was created just for fun.
**It's way far from production.**

## Expected Behavior

* Lightgray border, hint text and search icon.
* Gray close button with no default visual effect.
* Text box:
  * Hoverred or focused: *No* visual effect.
  * Hint text shown when *Empty && Not Focused*.
* Search icon:
  * Shown when empty.
  * Click to focus the text box.
  * *No* visual effects. *Nowhere* like a button.
* Close button:
  * *No* visual effect by default.
  * Shown when text box is *Not Empty*.
  * Clicked or hoverred: Button-like glowing effect.
  * Click to
    * Stop searching instantly;
    * Clear text box;
    * And remove focus.
* Clicking outsides is equivelant to clicking
the close button.
* Update searching text when text box recieves key press:
  * Clear delaying update task.
  * Delay for 1 sec by default.
  * Update instantly when the key is *Enter*.
  * *Esc* keypress is equivelant to clicking
the close button.
* "Remove focus" means
  * Previously focused item of
the main list (for File Explorer it's the file list)
would be re-focused.
  * With no previous focused item
the first item would be chosen.