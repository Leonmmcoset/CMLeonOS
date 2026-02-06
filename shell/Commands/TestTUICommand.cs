using System;
using System.Threading;
using CMLeonOS.UI;

namespace CMLeonOS.Commands
{
    public static class TestTUICommand
    {
        public static void RunTestTUI()
        {
            global::System.Console.Clear();

            while (true)
            {
                ShowMainMenu();
            }
        }

        private static void ShowMainMenu()
        {
            global::System.Console.Clear();

            var menu = new Menu(new Rect(5, 5, 70, 15));
            menu.Items.Add(new MenuItem("Button Demo", () => ShowButtonDemo()));
            menu.Items.Add(new MenuItem("Input Box Demo", () => ShowInputBoxDemo()));
            menu.Items.Add(new MenuItem("Dialog Demo", () => ShowDialogDemo()));
            menu.Items.Add(new MenuItem("Progress Bar Demo", () => ShowProgressBarDemo()));
            menu.Items.Add(new MenuItem("Tab Control Demo", () => ShowTabControlDemo()));
            menu.Items.Add(new MenuItem("Menu Demo", () => ShowMenuDemo()));
            menu.Items.Add(new MenuItem("Exit", () => global::System.Environment.Exit(0)));

            menu.Render();

            bool menuRunning = true;
            while (menuRunning)
            {
                var key = global::System.Console.ReadKey(true);
                if (menu.HandleKey(key))
                {
                    if (key.Key == global::System.ConsoleKey.Enter)
                    {
                        menuRunning = false;
                    }
                    else
                    {
                        menu.Render();
                        global::System.Threading.Thread.Sleep(100);
                    }
                }
                else if (key.Key == global::System.ConsoleKey.Escape)
                {
                    menuRunning = false;
                }
            }
        }

        private static void ShowButtonDemo()
        {
            global::System.Console.Clear();

            var label = new Label(new Point(10, 13), "Button Component Examples:");
            label.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to click, ESC to return"));
            statusBar.Render();

            var button1 = new Button(new Rect(10, 8, 20, 3), "Click Me!", global::System.ConsoleColor.Cyan, global::System.ConsoleColor.DarkBlue, global::System.ConsoleColor.White);
            var button2 = new Button(new Rect(35, 8, 20, 3), "Disabled", global::System.ConsoleColor.Gray, global::System.ConsoleColor.Black, global::System.ConsoleColor.DarkGray);
            button2.IsEnabled = false;
            var button3 = new Button(new Rect(60, 8, 20, 3), "Hover Me", global::System.ConsoleColor.Yellow, global::System.ConsoleColor.DarkRed, global::System.ConsoleColor.White);

            button1.Render();
            button2.Render();
            button3.Render();

            int focusedButton = 0;
            button1.IsFocused = true;

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (key.Key == global::System.ConsoleKey.Enter)
                {
                    if (focusedButton == 0 && button1.IsEnabled)
                    {
                        global::System.Console.SetCursorPosition(10, 14);
                        global::System.Console.Write("Clicked!");
                        global::System.Threading.Thread.Sleep(1000);
                    }
                }
                else if (key.Key == global::System.ConsoleKey.LeftArrow)
                {
                    button1.IsFocused = false;
                    button3.IsFocused = false;
                    focusedButton = (focusedButton - 1 + 3) % 3;
                    
                    if (focusedButton == 0 && button1.IsEnabled) button1.IsFocused = true;
                    if (focusedButton == 1 && button2.IsEnabled) button2.IsFocused = true;
                    if (focusedButton == 2 && button3.IsEnabled) button3.IsFocused = true;
                }
                else if (key.Key == global::System.ConsoleKey.RightArrow)
                {
                    button1.IsFocused = false;
                    button3.IsFocused = false;
                    focusedButton = (focusedButton + 1) % 3;
                    
                    if (focusedButton == 0 && button1.IsEnabled) button1.IsFocused = true;
                    if (focusedButton == 1 && button2.IsEnabled) button2.IsFocused = true;
                    if (focusedButton == 2 && button3.IsEnabled) button3.IsFocused = true;
                }

                button1.Render();
                button2.Render();
                button3.Render();
            }
        }

        private static void ShowInputBoxDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to submit, ESC to return"));
            statusBar.Render();

            var inputBox = new InputBox(new Rect(20, 8, 40, 3), "Enter your name:", "John Doe", 20, false, global::System.ConsoleColor.White, global::System.ConsoleColor.Black, global::System.ConsoleColor.White);
            inputBox.MaxLength = 20;
            inputBox.IsFocused = true;

            var button = new Button(new Rect(65, 8, 15, 3), "Submit");
            button.OnClick = () => 
            {
                global::System.Console.SetCursorPosition(20, 12);
                global::System.Console.Write($"Submitted: {inputBox.Value}");
                global::System.Threading.Thread.Sleep(2000);
            };

            inputBox.Render();
            button.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (inputBox.HandleKey(key))
                {
                    inputBox.Render();
                }
                else if (button.HandleKey(key))
                {
                    button.Render();
                }
            }
        }

        private static void ShowDialogDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use LEFT/RIGHT to select, ENTER to choose"));
            statusBar.Render();

            var dialog = new Dialog(new Rect(15, 5, 50, 7), "Confirm Action", "Are you sure you want to proceed?");
            
            dialog.Buttons.Add(new Button(new Rect(17, 14, 12, 3), "Yes", global::System.ConsoleColor.White, global::System.ConsoleColor.DarkGreen, global::System.ConsoleColor.White));
            dialog.Buttons[0].OnClick = () => 
            {
                global::System.Console.SetCursorPosition(15, 13);
                global::System.Console.Write("Confirmed!");
                global::System.Threading.Thread.Sleep(1500);
            };
            
            dialog.Buttons.Add(new Button(new Rect(31, 14, 12, 3), "No", global::System.ConsoleColor.White, global::System.ConsoleColor.DarkRed, global::System.ConsoleColor.White));
            dialog.Buttons[1].OnClick = () => 
            {
                global::System.Console.SetCursorPosition(31, 13);
                global::System.Console.Write("Cancelled!");
                global::System.Threading.Thread.Sleep(1500);
            };

            dialog.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (dialog.HandleKey(key))
                {
                    dialog.Render();
                }
            }
        }

        private static void ShowProgressBarDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to start, ESC to return"));
            statusBar.Render();

            var progressBar = new ProgressBar(new Point(15, 10), 50, 100);
            progressBar.Value = 0;

            var button = new Button(new Rect(35, 13, 15, 3), "Start");
            button.OnClick = () => 
            {
                for (int i = 0; i <= 100; i++)
                {
                    progressBar.SetValue(i);
                    progressBar.Render();
                    global::System.Threading.Thread.Sleep(30);
                }
            };

            progressBar.Render();
            button.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (button.HandleKey(key))
                {
                    button.Render();
                }
            }
        }

        private static void ShowTabControlDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use LEFT/RIGHT to switch tabs, ESC to return"));
            statusBar.Render();

            var tabControl = new TabControl(new Rect(5, 5, 70, 15));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 1", () => 
            {
                TUIHelper.SetColors(global::System.ConsoleColor.Cyan, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(10, 10);
                global::System.Console.Write("Tab 1 Content");
                TUIHelper.ResetColors();
            }));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 2", () => 
            {
                TUIHelper.SetColors(global::System.ConsoleColor.Green, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(10, 10);
                global::System.Console.Write("Tab 2 Content");
                TUIHelper.ResetColors();
            }));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 3", () => 
            {
                TUIHelper.SetColors(global::System.ConsoleColor.Magenta, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(10, 10);
                global::System.Console.Write("Tab 3 Content");
                TUIHelper.ResetColors();
            }));

            tabControl.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (tabControl.HandleKey(key))
                {
                    tabControl.Render();
                }
            }
        }

        private static void ShowMenuDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN to navigate, ENTER to select, ESC to return"));
            statusBar.Render();

            var menu = new Menu(new Rect(15, 5, 50, 15));
            menu.Items.Add(new MenuItem("Option 1: Basic functionality", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 1");
                global::System.Threading.Thread.Sleep(1500);
            }));
            menu.Items.Add(new MenuItem("Option 2: Advanced features", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 2");
                global::System.Threading.Thread.Sleep(1500);
            }));
            menu.Items.Add(new MenuItem("Option 3: Settings", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 3");
                global::System.Threading.Thread.Sleep(1500);
            }));
            menu.Items.Add(new MenuItem("Option 4: Help", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 4");
                global::System.Threading.Thread.Sleep(1500);
            }));
            menu.Items.Add(new MenuItem("Option 5: About", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 5");
                global::System.Threading.Thread.Sleep(1500);
            }));
            menu.Items.Add(new MenuItem("Option 6: Exit", () => 
            {
                global::System.Console.SetCursorPosition(15, 21);
                global::System.Console.Write("Selected: Option 6");
                global::System.Threading.Thread.Sleep(1500);
            }));

            menu.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (menu.HandleKey(key))
                {
                    menu.Render();
                }
            }
        }
    }
}
