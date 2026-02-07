using System;
using System.Collections.Generic;
using System.Linq;

namespace CMLeonOS.UI
{
    public delegate void ButtonClickHandler();

    public class Button
    {
        public Rect Bounds { get; set; }
        public string Text { get; set; }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public bool IsEnabled { get; set; } = true;
        public bool IsFocused { get; set; } = false;
        public ButtonClickHandler OnClick { get; set; }

        public Button(Rect bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        public Button(Rect bounds, string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor, ConsoleColor borderColor)
        {
            Bounds = bounds;
            Text = text;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
        }

        public void Render()
        {
            if (!IsEnabled)
            {
                TUIHelper.SetColors(ConsoleColor.DarkGray, ConsoleColor.Black);
                TUIHelper.DrawBox(Bounds, Text, ConsoleColor.DarkGray);
                return;
            }

            if (IsFocused)
            {
                TUIHelper.SetColors(ForegroundColor, BackgroundColor);
                TUIHelper.DrawBox(Bounds, Text, BorderColor);
            }
            else
            {
                TUIHelper.SetColors(ConsoleColor.Gray, ConsoleColor.Black);
                TUIHelper.DrawBox(Bounds, Text, ConsoleColor.DarkGray);
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (!IsEnabled) return false;

            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                OnClick?.Invoke();
                return true;
            }

            return false;
        }
    }

    public class InputBox
    {
        public Rect Bounds { get; set; }
        public string Label { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public int MaxLength { get; set; } = 50;
        public bool IsPassword { get; set; } = false;
        public bool IsFocused { get; set; } = false;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;

        public InputBox(Rect bounds, string label, string placeholder, int maxLength, bool isPassword, ConsoleColor foregroundColor, ConsoleColor backgroundColor, ConsoleColor borderColor)
        {
            Bounds = bounds;
            Label = label;
            Placeholder = placeholder;
            MaxLength = maxLength;
            IsPassword = isPassword;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
        }

        public void Render()
        {
            TUIHelper.SetColors(ForegroundColor, BackgroundColor);
            TUIHelper.DrawBox(Bounds, Label, BorderColor);

            int inputY = Bounds.Top + 1;
            string displayValue = IsPassword ? new string('*', Value.Length) : Value;
            string displayText = string.IsNullOrEmpty(Value) ? Placeholder : displayValue;

            Console.SetCursorPosition(Bounds.Left + 1, inputY);
            Console.Write(TUIHelper.PadText(displayText, Bounds.Width - 2));

            if (IsFocused)
            {
                TUIHelper.SaveCursor();
                Console.SetCursorPosition(Bounds.Left + 1 + displayText.Length, inputY);
                TUIHelper.ShowCursor();
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (!IsFocused) return false;

            if (key.Key == ConsoleKey.Backspace)
            {
                if (Value.Length > 0)
                {
                    Value = Value.Substring(0, Value.Length - 1);
                }
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                return true;
            }

            if (char.IsLetterOrDigit(key.KeyChar))
            {
                if (Value.Length < MaxLength)
                {
                    Value += key.KeyChar;
                }
                return true;
            }

            return false;
        }
    }

    public class Dialog
    {
        public Rect Bounds { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<Button> Buttons { get; set; } = new List<Button>();
        public int SelectedButtonIndex { get; set; } = 0;
        public ConsoleColor TitleColor { get; set; } = ConsoleColor.White;
        public ConsoleColor MessageColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public Dialog(Rect bounds, string title, string message)
        {
            Bounds = bounds;
            Title = title;
            Message = message;
        }

        public Dialog(Rect bounds, string title, string message, List<Button> buttons)
        {
            Bounds = bounds;
            Title = title;
            Message = message;
            Buttons = buttons;
        }

        public Dialog(Rect bounds, string title, string message, List<Button> buttons, int selectedIndex)
        {
            Bounds = bounds;
            Title = title;
            Message = message;
            Buttons = buttons;
            SelectedButtonIndex = selectedIndex;
            
            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].IsFocused = (i == SelectedButtonIndex);
            }
        }

        public Dialog(Rect bounds, string title, string message, List<Button> buttons, int selectedIndex, ConsoleColor titleColor, ConsoleColor messageColor, ConsoleColor borderColor)
        {
            Bounds = bounds;
            Title = title;
            Message = message;
            Buttons = buttons;
            SelectedButtonIndex = selectedIndex;
            TitleColor = titleColor;
            MessageColor = messageColor;
            BorderColor = borderColor;
        }

        public void Render()
        {
            TUIHelper.SetColors(TitleColor, BackgroundColor);
            TUIHelper.DrawBox(Bounds, Title, BorderColor);

            int messageY = Bounds.Top + 2;
            TUIHelper.SetColors(MessageColor, BackgroundColor);
            Console.SetCursorPosition(Bounds.Left + 1, messageY);
            Console.Write(TUIHelper.TruncateText(Message, Bounds.Width - 2));

            int buttonY = Bounds.Bottom - 2;
            int buttonWidth = Bounds.Width / Buttons.Count;
            int buttonX = Bounds.Left + 1;

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].IsFocused = (i == SelectedButtonIndex);
                Buttons[i].Bounds = new Rect(buttonX, buttonY, buttonWidth - 2, 3);
                Buttons[i].Render();
                buttonX += buttonWidth;
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.LeftArrow)
            {
                Buttons[SelectedButtonIndex].IsFocused = false;
                SelectedButtonIndex = (SelectedButtonIndex - 1 + Buttons.Count) % Buttons.Count;
                Buttons[SelectedButtonIndex].IsFocused = true;
                return true;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                Buttons[SelectedButtonIndex].IsFocused = false;
                SelectedButtonIndex = (SelectedButtonIndex + 1) % Buttons.Count;
                Buttons[SelectedButtonIndex].IsFocused = true;
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                if (SelectedButtonIndex >= 0 && SelectedButtonIndex < Buttons.Count)
                {
                    Buttons[SelectedButtonIndex].OnClick?.Invoke();
                }
                return true;
            }

            return false;
        }
    }

    public class Menu
    {
        public Rect Bounds { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
        public int SelectedIndex { get; set; } = 0;
        public int ScrollOffset { get; set; } = 0;
        public ConsoleColor SelectedColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor NormalColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public Menu(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Render()
        {
            TUIHelper.DrawBox(Bounds, string.Empty, BorderColor);

            int startY = Bounds.Top + 1;
            int maxHeight = Bounds.Height - 2;

            for (int i = ScrollOffset; i < Items.Count && i < ScrollOffset + maxHeight; i++)
            {
                int y = startY + (i - ScrollOffset);
                ConsoleColor color = (i == SelectedIndex) ? SelectedColor : NormalColor;
                string prefix = (i == SelectedIndex) ? "> " : " ";

                TUIHelper.SetColors(color, BackgroundColor);
                Console.SetCursorPosition(Bounds.Left + 1, y);
                Console.Write(prefix + TUIHelper.TruncateText(Items[i].Text, Bounds.Width - 3));
            }

            if (Items.Count > maxHeight)
            {
                int scrollY = Bounds.Bottom - 1;
                TUIHelper.SetColors(ConsoleColor.Gray, BackgroundColor);
                Console.SetCursorPosition(Bounds.Right - 5, scrollY);
                Console.Write($"({ScrollOffset + 1}/{(int)Math.Ceiling((double)Items.Count / maxHeight)})");
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.PageUp)
            {
                SelectedIndex = (SelectedIndex - 1 + Items.Count) % Items.Count;
                if (SelectedIndex < ScrollOffset) ScrollOffset = SelectedIndex;
                return true;
            }

            if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.PageDown)
            {
                SelectedIndex = (SelectedIndex + 1) % Items.Count;
                int maxScroll = Items.Count - (Bounds.Height - 2);
                if (SelectedIndex >= ScrollOffset + (Bounds.Height - 2))
                {
                    ScrollOffset = SelectedIndex - (Bounds.Height - 3);
                }
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                {
                    Items[SelectedIndex].OnSelect?.Invoke();
                }
                return true;
            }

            return false;
        }
    }

    public class MenuItem
    {
        public string Text { get; set; }
        public Action OnSelect { get; set; }

        public MenuItem(string text)
        {
            Text = text;
        }

        public MenuItem(string text, Action onSelect)
        {
            Text = text;
            OnSelect = onSelect;
        }
    }

    public class Label
    {
        public Point Position { get; set; }
        public string Text { get; set; }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor? BackgroundColor { get; set; } = null;

        public Label(Point position, string text)
        {
            Position = position;
            Text = text;
        }

        public void Render()
        {
            if (BackgroundColor != null)
            {
                TUIHelper.SetColors(ForegroundColor, BackgroundColor.Value);
            }
            else
            {
                TUIHelper.SetColors(ForegroundColor, ConsoleColor.Black);
            }

            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(Text);
            TUIHelper.ResetColors();
        }
    }

    public class ProgressBar
    {
        public Point Position { get; set; }
        public int Width { get; set; }
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public ConsoleColor FillColor { get; set; } = ConsoleColor.DarkGreen;
        public ConsoleColor EmptyColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public bool ShowPercentage { get; set; } = true;

        public ProgressBar(Point position, int width, int maxValue)
        {
            Position = position;
            Width = width;
            MaxValue = maxValue;
        }

        public void Render()
        {
            int fillWidth = (int)((double)Value / MaxValue * Width);
            int emptyWidth = Width - fillWidth;

            TUIHelper.SetColors(FillColor, BackgroundColor);
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write('[');

            for (int i = 0; i < fillWidth; i++)
            {
                Console.Write('#');
            }

            TUIHelper.SetColors(EmptyColor, BackgroundColor);
            for (int i = 0; i < emptyWidth; i++)
            {
                Console.Write('-');
            }

            TUIHelper.SetColors(BorderColor, BackgroundColor);
            Console.Write(']');

            if (ShowPercentage)
            {
                string percentage = ((double)Value / MaxValue * 100).ToString("F0") + "%";
                TUIHelper.SetColors(ConsoleColor.White, BackgroundColor);
                Console.Write($" {percentage}");
            }

            TUIHelper.ResetColors();
        }

        public void SetValue(int value)
        {
            Value = Math.Max(0, Math.Min(value, MaxValue));
        }
    }

    public class TabControl
    {
        public Rect Bounds { get; set; }
        public List<TabPage> Pages { get; set; } = new List<TabPage>();
        public int SelectedIndex { get; set; } = 0;
        public ConsoleColor SelectedColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor NormalColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public TabControl(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Render()
        {
            TUIHelper.DrawBox(Bounds, string.Empty, BorderColor);

            int tabWidth = Bounds.Width / Pages.Count;
            int tabX = Bounds.Left + 1;

            for (int i = 0; i < Pages.Count; i++)
            {
                string prefix = (i == SelectedIndex) ? "[" : " ";
                string suffix = (i == SelectedIndex) ? "]" : " ";

                TUIHelper.SetColors((i == SelectedIndex) ? SelectedColor : NormalColor, BackgroundColor);
                Console.SetCursorPosition(tabX, Bounds.Top);
                Console.Write(prefix + Pages[i].Title + suffix);

                tabX += tabWidth;
            }

            TUIHelper.SetColors(ConsoleColor.Gray, BackgroundColor);
            Console.SetCursorPosition(Bounds.Left + 1, Bounds.Bottom - 1);
            Console.Write(new string(' ', Bounds.Width - 2));

            Pages[SelectedIndex].Render();
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.LeftArrow)
            {
                SelectedIndex = (SelectedIndex - 1 + Pages.Count) % Pages.Count;
                return true;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                SelectedIndex = (SelectedIndex + 1) % Pages.Count;
                return true;
            }

            return false;
        }
    }

    public class TabPage
    {
        public Rect Bounds { get; set; }
        public string Title { get; set; }
        public Action Render { get; set; }

        public TabPage(Rect bounds, string title, Action render)
        {
            Bounds = bounds;
            Title = title;
            Render = render;
        }
    }

    public class StatusBar
    {
        public Rect Bounds { get; set; }
        public List<StatusBarItem> Items { get; set; } = new List<StatusBarItem>();
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public StatusBar(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Render()
        {
            TUIHelper.SetColors(ForegroundColor, BackgroundColor);
            TUIHelper.DrawHorizontalLine(Bounds.Left, Bounds.Top, Bounds.Width, '-', null);

            int x = Bounds.Left + 1;
            foreach (var item in Items)
            {
                TUIHelper.SetColors(item.Color, BackgroundColor);
                Console.SetCursorPosition(x, Bounds.Top);
                Console.Write(item.Text);
                x += item.Text.Length + 2;
            }
        }
    }

    public class StatusBarItem
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.White;

        public StatusBarItem(string text)
        {
            Text = text;
        }

        public StatusBarItem(string text, ConsoleColor color)
        {
            Text = text;
            Color = color;
        }
    }

    public class Window
    {
        public Rect Bounds { get; set; }
        public string Title { get; set; }
        public Action RenderContent { get; set; }
        public bool HasBorder { get; set; } = true;
        public ConsoleColor TitleColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public Window(Rect bounds, string title, Action renderContent)
        {
            Bounds = bounds;
            Title = title;
            RenderContent = renderContent;
        }

        public Window(Rect bounds, string title, Action renderContent, bool hasBorder)
        {
            Bounds = bounds;
            Title = title;
            RenderContent = renderContent;
            HasBorder = hasBorder;
        }

        public void Render()
        {
            TUIHelper.SetColors(TitleColor, BackgroundColor);
            
            if (HasBorder)
            {
                TUIHelper.DrawBox(Bounds, Title, BorderColor);
            }
            else
            {
                TUIHelper.DrawHorizontalLine(Bounds.Left, Bounds.Top, Bounds.Width, '-', null);
            }

            RenderContent?.Invoke();
        }
    }

    public class ListBox
    {
        public Rect Bounds { get; set; }
        public List<string> Items { get; set; } = new List<string>();
        public int SelectedIndex { get; set; } = 0;
        public int ScrollOffset { get; set; } = 0;
        public ConsoleColor SelectedColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor NormalColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;
        public bool MultiSelect { get; set; } = false;
        public List<int> SelectedIndices { get; set; } = new List<int>();

        public ListBox()
        {
        }

        public ListBox(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Render()
        {
            TUIHelper.SetColors(BorderColor, BackgroundColor);
            TUIHelper.DrawBox(Bounds, string.Empty, BorderColor);

            int startY = Bounds.Top + 1;
            int maxHeight = Bounds.Height - 2;

            for (int i = ScrollOffset; i < Items.Count && i < ScrollOffset + maxHeight; i++)
            {
                int y = startY + (i - ScrollOffset);
                ConsoleColor color = (i == SelectedIndex) ? SelectedColor : NormalColor;
                string prefix = (i == SelectedIndex) ? "> " : " ";

                if (MultiSelect && SelectedIndices.Contains(i))
                {
                    prefix = "[x] ";
                }
                else if (MultiSelect)
                {
                    prefix = "[ ] ";
                }

                TUIHelper.SetColors(color, BackgroundColor);
                Console.SetCursorPosition(Bounds.Left + 1, y);
                Console.Write(prefix + TUIHelper.TruncateText(Items[i], Bounds.Width - 3));
            }

            if (Items.Count > maxHeight)
            {
                int scrollY = Bounds.Bottom - 1;
                TUIHelper.SetColors(ConsoleColor.Gray, BackgroundColor);
                Console.SetCursorPosition(Bounds.Right - 10, scrollY);
                Console.Write($"({ScrollOffset + 1}/{(int)Math.Ceiling((double)Items.Count / maxHeight)})");
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.PageUp)
            {
                SelectedIndex = (SelectedIndex - 1 + Items.Count) % Items.Count;
                if (SelectedIndex < ScrollOffset) ScrollOffset = SelectedIndex;
                return true;
            }

            if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.PageDown)
            {
                SelectedIndex = (SelectedIndex + 1) % Items.Count;
                int maxScroll = Items.Count - (Bounds.Height - 2);
                if (SelectedIndex >= ScrollOffset + (Bounds.Height - 2))
                {
                    ScrollOffset = SelectedIndex - (Bounds.Height - 3);
                }
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                if (MultiSelect)
                {
                    if (SelectedIndices.Contains(SelectedIndex))
                    {
                        SelectedIndices.Remove(SelectedIndex);
                    }
                    else
                    {
                        SelectedIndices.Add(SelectedIndex);
                    }
                }
                return true;
            }

            if (key.Key == ConsoleKey.Spacebar && MultiSelect)
            {
                if (SelectedIndices.Contains(SelectedIndex))
                {
                    SelectedIndices.Remove(SelectedIndex);
                }
                else
                {
                    SelectedIndices.Add(SelectedIndex);
                }
                return true;
            }

            return false;
        }
    }

    public class CheckBox
    {
        public Point Position { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsFocused { get; set; } = false;
        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BoxColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor FocusedTextColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor FocusedBoxColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor FocusedBackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public CheckBox()
        {
        }

        public CheckBox(Point position, string text)
        {
            Position = position;
            Text = text;
        }

        public void Render()
        {
            if (IsFocused)
            {
                TUIHelper.SetColors(FocusedTextColor, FocusedBackgroundColor);
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(IsChecked ? "[X] " : "[ ] ");
                Console.Write(Text);
            }
            else
            {
                TUIHelper.SetColors(TextColor, BackgroundColor);
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(IsChecked ? "[x] " : "[ ] ");
                Console.Write(Text);
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                IsChecked = !IsChecked;
                return true;
            }
            return false;
        }
    }

    public class RadioButton
    {
        public Point Position { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsFocused { get; set; } = false;
        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BoxColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public string GroupName { get; set; } = string.Empty;
        public ConsoleColor FocusedTextColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor FocusedBoxColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor FocusedBackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public RadioButton()
        {
        }

        public RadioButton(Point position, string text)
        {
            Position = position;
            Text = text;
        }

        public void Render()
        {
            if (IsFocused)
            {
                TUIHelper.SetColors(FocusedTextColor, FocusedBackgroundColor);
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(IsChecked ? "(*) " : "( ) ");
                Console.Write(Text);
            }
            else
            {
                TUIHelper.SetColors(TextColor, BackgroundColor);
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(IsChecked ? "(*) " : "( ) ");
                Console.Write(Text);
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                IsChecked = !IsChecked;
                return true;
            }
            return false;
        }
    }

    public class TreeViewNode
    {
        public string Text { get; set; }
        public List<TreeViewNode> Children { get; set; } = new List<TreeViewNode>();
        public bool IsExpanded { get; set; } = false;
        public int Level { get; set; } = 0;
        public TreeViewNode Parent { get; set; }

        public bool HasChildren => Children.Count > 0;

        public TreeViewNode()
        {
        }

        public TreeViewNode(string text)
        {
            Text = text;
        }
    }

    public class TreeView
    {
        public Rect Bounds { get; set; }
        public TreeViewNode Root { get; set; }
        public List<TreeViewNode> SelectedNodes { get; set; } = new List<TreeViewNode>();
        public ConsoleColor SelectedColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor NormalColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;
        public int ScrollOffset { get; set; } = 0;

        public TreeView()
        {
        }

        public TreeView(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Render()
        {
            TUIHelper.SetColors(BorderColor, BackgroundColor);
            TUIHelper.DrawBox(Bounds, string.Empty, BorderColor);

            int startY = Bounds.Top + 1;
            int maxHeight = Bounds.Height - 2;

            List<TreeViewNode> visibleNodes = GetVisibleNodes();
            for (int i = ScrollOffset; i < visibleNodes.Count && i < ScrollOffset + maxHeight; i++)
            {
                int y = startY + (i - ScrollOffset);
                var node = visibleNodes[i];
                ConsoleColor color = SelectedNodes.Contains(node) ? SelectedColor : NormalColor;
                string prefix = GetNodePrefix(node);

                TUIHelper.SetColors(color, BackgroundColor);
                Console.SetCursorPosition(Bounds.Left + 1, y);
                Console.Write(prefix + TUIHelper.TruncateText(node.Text, Bounds.Width - 3 - node.Level * 2));
            }

            if (visibleNodes.Count > maxHeight)
            {
                int scrollY = Bounds.Bottom - 1;
                TUIHelper.SetColors(ConsoleColor.Gray, BackgroundColor);
                Console.SetCursorPosition(Bounds.Right - 10, scrollY);
                Console.Write($"({ScrollOffset + 1}/{(int)Math.Ceiling((double)visibleNodes.Count / maxHeight)})");
            }
        }

        private List<TreeViewNode> GetVisibleNodes()
        {
            List<TreeViewNode> nodes = new List<TreeViewNode>();
            if (Root != null)
            {
                AddVisibleNodes(Root, nodes);
            }
            return nodes;
        }

        private void AddVisibleNodes(TreeViewNode node, List<TreeViewNode> nodes)
        {
            nodes.Add(node);
            if (node.IsExpanded)
            {
                foreach (var child in node.Children)
                {
                    AddVisibleNodes(child, nodes);
                }
            }
        }

        private string GetNodePrefix(TreeViewNode node)
        {
            string prefix = "";
            for (int i = 0; i < node.Level; i++)
            {
                prefix += "  ";
            }

            if (node.HasChildren)
            {
                prefix += node.IsExpanded ? "- " : "+ ";
            }
            else
            {
                prefix += "+ ";
            }

            return prefix;
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            var visibleNodes = GetVisibleNodes();
            if (visibleNodes.Count == 0) return false;

            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.PageUp)
            {
                int currentIndex = visibleNodes.FindIndex(n => SelectedNodes.Contains(n));
                if (currentIndex > 0)
                {
                    SelectedNodes.Clear();
                    SelectedNodes.Add(visibleNodes[currentIndex - 1]);
                    if (currentIndex - 1 < ScrollOffset) ScrollOffset = currentIndex - 1;
                }
                return true;
            }

            if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.PageDown)
            {
                int currentIndex = visibleNodes.FindIndex(n => SelectedNodes.Contains(n));
                if (currentIndex < visibleNodes.Count - 1)
                {
                    SelectedNodes.Clear();
                    SelectedNodes.Add(visibleNodes[currentIndex + 1]);
                    int maxScroll = visibleNodes.Count - (Bounds.Height - 2);
                    if (currentIndex + 1 >= ScrollOffset + (Bounds.Height - 2))
                    {
                        ScrollOffset = (currentIndex + 1) - (Bounds.Height - 3);
                    }
                }
                return true;
            }

            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                var currentNode = visibleNodes.FirstOrDefault(n => SelectedNodes.Contains(n));
                if (currentNode != null && currentNode.HasChildren)
                {
                    currentNode.IsExpanded = !currentNode.IsExpanded;
                }
                else if (currentNode != null)
                {
                    if (SelectedNodes.Contains(currentNode))
                    {
                        SelectedNodes.Remove(currentNode);
                    }
                    else
                    {
                        SelectedNodes.Add(currentNode);
                    }
                }
                return true;
            }

            return false;
        }
    }

    public class ScrollBar
    {
        public Point Position { get; set; }
        public int Height { get; set; }
        public int MaxValue { get; set; }
        public int Value { get; set; } = 0;
        public ConsoleColor BarColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public bool IsVertical { get; set; } = true;

        public ScrollBar()
        {
        }

        public ScrollBar(Point position, int height)
        {
            Position = position;
            Height = height;
        }

        public void Render()
        {
            TUIHelper.SetColors(BorderColor, BackgroundColor);
            
            if (IsVertical)
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write('┌');
                for (int i = 0; i < Height - 2; i++)
                {
                    Console.SetCursorPosition(Position.X, Position.Y + 1 + i);
                    Console.Write('│');
                }
                Console.SetCursorPosition(Position.X, Position.Y + Height - 1);
                Console.Write('└');

                int barHeight = Height - 2;
                int barPosition = (int)((double)Value / MaxValue * barHeight);
                for (int i = 0; i < barHeight; i++)
                {
                    Console.SetCursorPosition(Position.X, Position.Y + 1 + i);
                    if (i >= barPosition && i < barPosition + (int)((double)barHeight / MaxValue * Value))
                    {
                        Console.Write('█');
                    }
                    else
                    {
                        Console.Write('░');
                    }
                }
            }
            else
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write('┌');
                for (int i = 0; i < Height - 2; i++)
                {
                    Console.SetCursorPosition(Position.X + 1 + i, Position.Y);
                    Console.Write('─');
                }
                Console.SetCursorPosition(Position.X + Height - 1, Position.Y);
                Console.Write('┐');

                int barWidth = Height - 2;
                int barPosition = (int)((double)Value / MaxValue * barWidth);
                for (int i = 0; i < barWidth; i++)
                {
                    Console.SetCursorPosition(Position.X + 1 + i, Position.Y);
                    if (i >= barPosition && i < barPosition + (int)((double)barWidth / MaxValue * Value))
                    {
                        Console.Write('█');
                    }
                    else
                    {
                        Console.Write('░');
                    }
                }
            }
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (IsVertical)
            {
                if (key.Key == ConsoleKey.UpArrow)
                {
                    Value = Math.Max(0, Value - 1);
                    return true;
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    Value = Math.Min(MaxValue, Value + 1);
                    return true;
                }
                if (key.Key == ConsoleKey.PageUp)
                {
                    Value = Math.Max(0, Value - 10);
                    return true;
                }
                if (key.Key == ConsoleKey.PageDown)
                {
                    Value = Math.Min(MaxValue, Value + 10);
                    return true;
                }
            }
            else
            {
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    Value = Math.Max(0, Value - 1);
                    return true;
                }
                if (key.Key == ConsoleKey.RightArrow)
                {
                    Value = Math.Min(MaxValue, Value + 1);
                    return true;
                }
            }
            return false;
        }
    }
}
