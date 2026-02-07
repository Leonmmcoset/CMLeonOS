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
            menu.Items.Add(new MenuItem("ListBox Demo", () => ShowListBoxDemo()));
            menu.Items.Add(new MenuItem("CheckBox Demo", () => ShowCheckBoxDemo()));
            menu.Items.Add(new MenuItem("RadioButton Demo", () => ShowRadioButtonDemo()));
            menu.Items.Add(new MenuItem("TreeView Demo", () => ShowTreeViewDemo()));
            menu.Items.Add(new MenuItem("ScrollBar Demo", () => ShowScrollBarDemo()));
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

        private static void ShowListBoxDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN to navigate, SPACE to select, ESC to return"));
            statusBar.Render();

            var listBox = new ListBox(new Rect(10, 5, 60, 15));
            listBox.Items.Add("Item 1: Basic functionality");
            listBox.Items.Add("Item 2: Advanced features");
            listBox.Items.Add("Item 3: Settings");
            listBox.Items.Add("Item 4: Help");
            listBox.Items.Add("Item 5: About");
            listBox.Items.Add("Item 6: Documentation");
            listBox.Items.Add("Item 7: Configuration");
            listBox.Items.Add("Item 8: System info");
            listBox.Items.Add("Item 9: Network settings");
            listBox.Items.Add("Item 10: User management");
            listBox.MultiSelect = true;

            listBox.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (listBox.HandleKey(key))
                {
                    listBox.Render();
                }
            }
        }

        private static void ShowCheckBoxDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN to navigate, SPACE to toggle, ESC to return"));
            statusBar.Render();

            var checkBox1 = new CheckBox(new Point(10, 8), "Enable feature 1");
            checkBox1.IsChecked = true;
            checkBox1.IsFocused = true;
            var checkBox2 = new CheckBox(new Point(10, 10), "Enable feature 2");
            checkBox2.IsChecked = false;
            var checkBox3 = new CheckBox(new Point(10, 12), "Enable feature 3");
            checkBox3.IsChecked = false;
            var checkBox4 = new CheckBox(new Point(10, 14), "Enable feature 4");
            checkBox4.IsChecked = false;

            checkBox1.Render();
            checkBox2.Render();
            checkBox3.Render();
            checkBox4.Render();

            int focusedCheckBox = 0;

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (key.Key == global::System.ConsoleKey.UpArrow)
                {
                    checkBox1.IsFocused = false;
                    checkBox2.IsFocused = false;
                    checkBox3.IsFocused = false;
                    checkBox4.IsFocused = false;
                    focusedCheckBox = (focusedCheckBox - 1 + 4) % 4;
                    
                    if (focusedCheckBox == 0) checkBox1.IsFocused = true;
                    if (focusedCheckBox == 1) checkBox2.IsFocused = true;
                    if (focusedCheckBox == 2) checkBox3.IsFocused = true;
                    if (focusedCheckBox == 3) checkBox4.IsFocused = true;
                }
                else if (key.Key == global::System.ConsoleKey.DownArrow)
                {
                    checkBox1.IsFocused = false;
                    checkBox2.IsFocused = false;
                    checkBox3.IsFocused = false;
                    checkBox4.IsFocused = false;
                    focusedCheckBox = (focusedCheckBox + 1) % 4;
                    
                    if (focusedCheckBox == 0) checkBox1.IsFocused = true;
                    if (focusedCheckBox == 1) checkBox2.IsFocused = true;
                    if (focusedCheckBox == 2) checkBox3.IsFocused = true;
                    if (focusedCheckBox == 3) checkBox4.IsFocused = true;
                }
                else if (key.Key == global::System.ConsoleKey.Enter || key.Key == global::System.ConsoleKey.Spacebar)
                {
                    if (focusedCheckBox == 0) checkBox1.IsChecked = !checkBox1.IsChecked;
                    if (focusedCheckBox == 1) checkBox2.IsChecked = !checkBox2.IsChecked;
                    if (focusedCheckBox == 2) checkBox3.IsChecked = !checkBox3.IsChecked;
                    if (focusedCheckBox == 3) checkBox4.IsChecked = !checkBox4.IsChecked;
                }

                checkBox1.Render();
                checkBox2.Render();
                checkBox3.Render();
                checkBox4.Render();
            }
        }

        private static void ShowRadioButtonDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN to navigate, SPACE to select, ESC to return"));
            statusBar.Render();

            var radioButton1 = new RadioButton(new Point(10, 8), "Option 1");
            radioButton1.IsChecked = true;
            radioButton1.IsFocused = true;
            radioButton1.GroupName = "group1";
            var radioButton2 = new RadioButton(new Point(10, 10), "Option 2");
            radioButton2.IsChecked = false;
            radioButton2.GroupName = "group1";
            var radioButton3 = new RadioButton(new Point(10, 12), "Option 3");
            radioButton3.IsChecked = false;
            radioButton3.GroupName = "group1";
            var radioButton4 = new RadioButton(new Point(10, 14), "Option 4");
            radioButton4.IsChecked = false;
            radioButton4.GroupName = "group1";

            radioButton1.Render();
            radioButton2.Render();
            radioButton3.Render();
            radioButton4.Render();

            int focusedRadioButton = 0;

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (key.Key == global::System.ConsoleKey.UpArrow)
                {
                    radioButton1.IsFocused = false;
                    radioButton2.IsFocused = false;
                    radioButton3.IsFocused = false;
                    radioButton4.IsFocused = false;
                    focusedRadioButton = (focusedRadioButton - 1 + 4) % 4;
                    
                    if (focusedRadioButton == 0) radioButton1.IsFocused = true;
                    if (focusedRadioButton == 1) radioButton2.IsFocused = true;
                    if (focusedRadioButton == 2) radioButton3.IsFocused = true;
                    if (focusedRadioButton == 3) radioButton4.IsFocused = true;
                }
                else if (key.Key == global::System.ConsoleKey.DownArrow)
                {
                    radioButton1.IsFocused = false;
                    radioButton2.IsFocused = false;
                    radioButton3.IsFocused = false;
                    radioButton4.IsFocused = false;
                    focusedRadioButton = (focusedRadioButton + 1) % 4;
                    
                    if (focusedRadioButton == 0) radioButton1.IsFocused = true;
                    if (focusedRadioButton == 1) radioButton2.IsFocused = true;
                    if (focusedRadioButton == 2) radioButton3.IsFocused = true;
                    if (focusedRadioButton == 3) radioButton4.IsFocused = true;
                }
                else if (key.Key == global::System.ConsoleKey.Enter || key.Key == global::System.ConsoleKey.Spacebar)
                {
                    if (focusedRadioButton == 0) radioButton1.IsChecked = true;
                    if (focusedRadioButton == 1) radioButton2.IsChecked = true;
                    if (focusedRadioButton == 2) radioButton3.IsChecked = true;
                    if (focusedRadioButton == 3) radioButton4.IsChecked = true;
                }

                radioButton1.Render();
                radioButton2.Render();
                radioButton3.Render();
                radioButton4.Render();
            }
        }

        private static void ShowTreeViewDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN to navigate, ENTER/SPACE to expand/collapse, ESC to return"));
            statusBar.Render();

            var treeView = new TreeView(new Rect(5, 5, 70, 15));
            treeView.Root = new TreeViewNode { Text = "Root" };
            
            var child1 = new TreeViewNode { Text = "Folder 1", Level = 1, Parent = treeView.Root };
            var child2 = new TreeViewNode { Text = "Folder 2", Level = 1, Parent = treeView.Root };
            var child3 = new TreeViewNode { Text = "File 1", Level = 2, Parent = child1 };
            var child4 = new TreeViewNode { Text = "File 2", Level = 2, Parent = child1 };
            var child5 = new TreeViewNode { Text = "File 3", Level = 2, Parent = child2 };
            
            child1.Children.Add(child3);
            child1.Children.Add(child4);
            child2.Children.Add(child5);
            
            treeView.Root.Children.Add(child1);
            treeView.Root.Children.Add(child2);

            treeView.Render();

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (treeView.HandleKey(key))
                {
                    treeView.Render();
                }
            }
        }

        private static void ShowScrollBarDemo()
        {
            global::System.Console.Clear();

            var statusBar = new StatusBar(new Rect(0, 24, 80, 1));
            statusBar.Items.Add(new StatusBarItem("Use UP/DOWN or LEFT/RIGHT to adjust, ESC to return"));
            statusBar.Render();

            var scrollBar1 = new ScrollBar(new Point(15, 10), 10);
            scrollBar1.MaxValue = 100;
            scrollBar1.Value = 50;
            scrollBar1.IsVertical = true;

            var scrollBar2 = new ScrollBar(new Point(40, 10), 30);
            scrollBar2.MaxValue = 100;
            scrollBar2.Value = 30;
            scrollBar2.IsVertical = false;

            var scrollBar3 = new ScrollBar(new Point(15, 15), 10);
            scrollBar3.MaxValue = 50;
            scrollBar3.Value = 25;
            scrollBar3.IsVertical = true;

            var scrollBar4 = new ScrollBar(new Point(40, 15), 30);
            scrollBar4.MaxValue = 50;
            scrollBar4.Value = 15;
            scrollBar4.IsVertical = false;

            var label1 = new Label(new Point(15, 8), "Vertical ScrollBar:");
            var label2 = new Label(new Point(40, 8), "Horizontal ScrollBar:");
            var label3 = new Label(new Point(15, 13), "Small Vertical:");
            var label4 = new Label(new Point(40, 13), "Small Horizontal:");

            label1.Render();
            label2.Render();
            label3.Render();
            label4.Render();

            scrollBar1.Render();
            scrollBar2.Render();
            scrollBar3.Render();
            scrollBar4.Render();

            int focusedScrollBar = 0;

            bool demoRunning = true;
            while (demoRunning)
            {
                var key = global::System.Console.ReadKey(true);

                if (key.Key == global::System.ConsoleKey.Escape)
                {
                    demoRunning = false;
                }
                else if (key.Key == global::System.ConsoleKey.UpArrow || key.Key == global::System.ConsoleKey.DownArrow)
                {
                    if (focusedScrollBar == 0 && scrollBar1.HandleKey(key)) scrollBar1.Render();
                    if (focusedScrollBar == 1 && scrollBar2.HandleKey(key)) scrollBar2.Render();
                    if (focusedScrollBar == 2 && scrollBar3.HandleKey(key)) scrollBar3.Render();
                    if (focusedScrollBar == 3 && scrollBar4.HandleKey(key)) scrollBar4.Render();
                }
                else if (key.Key == global::System.ConsoleKey.LeftArrow || key.Key == global::System.ConsoleKey.RightArrow)
                {
                    if (focusedScrollBar == 0 && scrollBar1.HandleKey(key)) scrollBar1.Render();
                    if (focusedScrollBar == 1 && scrollBar2.HandleKey(key)) scrollBar2.Render();
                    if (focusedScrollBar == 2 && scrollBar3.HandleKey(key)) scrollBar3.Render();
                    if (focusedScrollBar == 3 && scrollBar4.HandleKey(key)) scrollBar4.Render();
                }
                else if (key.Key == global::System.ConsoleKey.Tab)
                {
                    focusedScrollBar = (focusedScrollBar + 1) % 4;
                }
            }
        }
    }
}
