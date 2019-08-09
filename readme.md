# ExplorerSearchBox

A WPF (Windows Presentation Foundation)
custom search box like the Windows File Explorer's.
**Along with a demo**.

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

No more than **10 lines** of code are needed for the
search box to be integrated into your window.

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

4. Subscribe to text change event.

   ```xaml
       ......
       <some_name:ExplorerSearchBox ......
           FocusManager.IsFocusScope="True"
   +       SearchRequested="..."
   +       DefaultFocusedElement="{Binding ElementName=YourMainListOrWhatever}"
          ...... />
   ```
   While user types the filter text would change after
   a certain delay. For instance the user typed "meoow",
   paused and found a typo, backspace x2 and typed "w",
   would trigger this event twice with text "meoow"
   and "meow". Another pause and typed "\~" would trigger
   the event with "meow\~".
   
   `DefaultFocusedElement` is used for focus restoration after the
   search being deactivated. If it's a list, its previous
   selected then first item will be preferred than itself,
   just like Windows File Explorer's search box.
   
   **Done!**
5. (optional) Events:
   * `SearchTextFocused`
   * `SearchTextUnfocused`
   
   And dependency properties:
   * `bool` `HandleClickOutsides` default: `true`
   * `int` `UpdateDelayMillisProperty` default: `1000` values:
      * value > 0: Update after `value` ms. Continuous inputs will be batched together.
      * 0: Update instantly. Every keypress will trigger an update.
      * value < 0: Don't update.
      
      `Enter` and `ESC` keypresses would always trigger an instant update.
   
   * `string` `HintTextProperty`

   All can be set in `xaml` or code behind.
6. (optional) You can add key binding e.g. Ctrl + F using `commandbinding`.

   In you SomeWindow.xaml.cs:
   ```cs
   // Static constructor
   static SomeWindow() {
       ......
      // Bind for Ctrl + F => Activate search.
   +   CommandManager.RegisterClassCommandBinding(
   +       typeof(SomeWindow),
   +       new CommandBinding(
   +           new RoutedCommand(
   +               "Find", typeof(SomeWindow),
   +               new InputGestureCollection(){
   +                   new KeyGesture(Key.F, ModifierKeys.Control)
   +               }
   +           ),
   +           (a, b) => (a as SomeWindow)?.SearchBox.ActivateSearch()
   +       )
   +   );
   }
   ```
6. You can also extend the poor functionality of this poor
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