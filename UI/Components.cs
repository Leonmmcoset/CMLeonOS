using System;
using System.Collections.Generic;

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
}
