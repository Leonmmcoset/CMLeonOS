using System;
using System.Threading;
using CMLeonOS.UI;

namespace CMLeonOS.UI.Examples
{
    public static class TUIDemo
    {
        public static void Run()
        {
            Console.Clear();
            Console.Title = "CMLeonOS TUI Library Demo";

            bool running = true;
            while (running)
            {
                ShowMainMenu();
            }
        }

        private static void ShowMainMenu()
        {
            Console.Clear();
            
            var menu = new Menu(new Rect(5, 5, 70, 15));
            menu.Items.Add(new MenuItem("Button Demo", () => ShowButtonDemo()));
            menu.Items.Add(new MenuItem("Input Box Demo", () => ShowInputBoxDemo()));
            menu.Items.Add(new MenuItem("Dialog Demo", () => ShowDialogDemo()));
            menu.Items.Add(new MenuItem("Progress Bar Demo", () => ShowProgressBarDemo()));
            menu.Items.Add(new MenuItem("Tab Control Demo", () => ShowTabControlDemo()));
            menu.Items.Add(new MenuItem("Exit", () => Environment.Exit(0)));

            menu.Render();

            bool menuRunning = true;
            while (menuRunning)
            {
                var key = Console.ReadKey(true);
                if (menu.HandleKey(key))
                {
                    Thread.Sleep(100);
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    menuRunning = false;
                }
            }
        }

        private static void ShowButtonDemo()
        {
            Console.Clear();

            var titleBar = new Window(new Rect(0, 0, 80, 3), "Button Demo", () => 
            {
                Console.Clear();
                ShowMainMenu();
            }, false);
            titleBar.Render();

            var button1 = new Button(new Rect(10, 8, 20, 3), "Click Me!", ConsoleColor.Cyan, ConsoleColor.DarkBlue, ConsoleColor.White);
            var button2 = new Button(new Rect(35, 8, 20, 3), "Disabled", ConsoleColor.Gray, ConsoleColor.Black, ConsoleColor.DarkGray);
            button2.IsEnabled = false;
            var button3 = new Button(new Rect(60, 8, 20, 3), "Hover Me", ConsoleColor.Yellow, ConsoleColor.DarkRed, ConsoleColor.White);

            button1.Render();
            button2.Render();
            button3.Render();

            var label = new Label(new Point(10, 13), "Button Component Examples:");
            label.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to click, ESC to return"));
            statusBar.Render();

            int focusedButton = 0;
            button1.IsFocused = true;

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (focusedButton == 0 && button1.IsEnabled)
                    {
                        Console.SetCursorPosition(10, 14);
                        Console.Write("Clicked!");
                        Thread.Sleep(1000);
                    }
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    button1.IsFocused = false;
                    button3.IsFocused = false;
                    focusedButton = (focusedButton - 1 + 3) % 3;
                    
                    if (focusedButton == 0 && button1.IsEnabled) button1.IsFocused = true;
                    if (focusedButton == 2 && button2.IsEnabled) button2.IsFocused = true;
                    if (focusedButton == 1 && button3.IsEnabled) button3.IsFocused = true;
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    button1.IsFocused = false;
                    button3.IsFocused = false;
                    focusedButton = (focusedButton + 1) % 3;
                    
                    if (focusedButton == 0 && button1.IsEnabled) button1.IsFocused = true;
                    if (focusedButton == 2 && button2.IsEnabled) button2.IsFocused = true;
                    if (focusedButton == 1 && button3.IsEnabled) button3.IsFocused = true;
                }

                button1.Render();
                button2.Render();
                button3.Render();
            }
        }

        private static void ShowInputBoxDemo()
        {
            Console.Clear();

            var titleBar = new Window(new Rect(0, 0, 80, 3), "Input Box Demo", () => 
            {
                Console.Clear();
                ShowMainMenu();
            }, false);
            titleBar.Render();

            var inputBox = new InputBox(new Rect(20, 8, 40, 3), "Enter your name:", "John Doe", 20, false, ConsoleColor.White, ConsoleColor.Black, ConsoleColor.White);
            inputBox.MaxLength = 20;

            var button = new Button(new Rect(65, 8, 15, 3), "Submit");
            button.OnClick = () => 
            {
                Console.SetCursorPosition(20, 12);
                Console.Write($"Submitted: {inputBox.Value}");
                Thread.Sleep(2000);
            };

            inputBox.Render();
            button.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to submit, ESC to return"));
            statusBar.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
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
            Console.Clear();

            var titleBar = new Window(new Rect(0, 0, 80, 3), "Dialog Demo", () => 
            {
                Console.Clear();
                ShowMainMenu();
            }, false);
            titleBar.Render();

            var dialog = new Dialog(new Rect(15, 5, 50, 7), "Confirm Action", "Are you sure you want to proceed?");
            
            dialog.Buttons.Add(new Button(new Rect(17, 14, 12, 3), "Yes", ConsoleColor.White, ConsoleColor.DarkGreen, ConsoleColor.White));
            dialog.Buttons[0].OnClick = () => 
            {
                Console.SetCursorPosition(15, 13);
                Console.Write("Confirmed!");
                Thread.Sleep(1500);
            };
            
            dialog.Buttons.Add(new Button(new Rect(31, 14, 12, 3), "No", ConsoleColor.White, ConsoleColor.DarkRed, ConsoleColor.White));
            dialog.Buttons[1].OnClick = () => 
            {
                Console.SetCursorPosition(31, 13);
                Console.Write("Cancelled!");
                Thread.Sleep(1500);
            };

            dialog.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use LEFT/RIGHT to select, ENTER to choose"));
            statusBar.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
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
            Console.Clear();

            var titleBar = new Window(new Rect(0, 0, 80, 3), "Progress Bar Demo", () => 
            {
                Console.Clear();
                ShowMainMenu();
            }, false);
            titleBar.Render();

            var progressBar = new ProgressBar(new Point(15, 10), 50, 100);
            progressBar.Value = 0;

            var button = new Button(new Rect(35, 13, 15, 3), "Start");
            button.OnClick = () => 
            {
                for (int i = 0; i <= 100; i++)
                {
                    progressBar.SetValue(i);
                    progressBar.Render();
                    Thread.Sleep(30);
                }
            };

            progressBar.Render();
            button.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Press ENTER to start, ESC to return"));
            statusBar.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
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
            Console.Clear();

            var titleBar = new Window(new Rect(0, 0, 80, 3), "Tab Control Demo", () => 
            {
                Console.Clear();
                ShowMainMenu();
            }, false);
            titleBar.Render();

            var tabControl = new TabControl(new Rect(5, 5, 70, 15));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 1", () => 
            {
                TUIHelper.SetColors(ConsoleColor.Cyan, ConsoleColor.Black);
                Console.SetCursorPosition(10, 10);
                Console.Write("Tab 1 Content");
                TUIHelper.ResetColors();
            }));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 2", () => 
            {
                TUIHelper.SetColors(ConsoleColor.Green, ConsoleColor.Black);
                Console.SetCursorPosition(10, 10);
                Console.Write("Tab 2 Content");
                TUIHelper.ResetColors();
            }));
            
            tabControl.Pages.Add(new TabPage(new Rect(5, 8, 70, 11), "Tab 3", () => 
            {
                TUIHelper.SetColors(ConsoleColor.Magenta, ConsoleColor.Black);
                Console.SetCursorPosition(10, 10);
                Console.Write("Tab 3 Content");
                TUIHelper.ResetColors();
            }));

            tabControl.Render();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use LEFT/RIGHT to switch tabs, ESC to return"));
            statusBar.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (tabControl.HandleKey(key))
                {
                    tabControl.Render();
                }
            }
        }
    }
}
