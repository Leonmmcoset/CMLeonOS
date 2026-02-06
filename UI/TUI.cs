using System;
using System.Collections.Generic;

namespace CMLeonOS.UI
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
    }

    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public struct Rect
    {
        public Point Position { get; set; }
        public Size Size { get; set; }

        public Rect(Point position, Size size)
        {
            Position = position;
            Size = size;
        }

        public Rect(int x, int y, int width, int height)
        {
            Position = new Point(x, y);
            Size = new Size(width, height);
        }

        public int Left => Position.X;
        public int Top => Position.Y;
        public int Right => Position.X + Size.Width;
        public int Bottom => Position.Y + Size.Height;
        public int Width => Size.Width;
        public int Height => Size.Height;

        public bool Contains(Point point)
        {
            return point.X >= Left && point.X < Right && point.Y >= Top && point.Y < Bottom;
        }
    }

    public enum ConsoleColorEx
    {
        Black = ConsoleColor.Black,
        DarkBlue = ConsoleColor.DarkBlue,
        DarkGreen = ConsoleColor.DarkGreen,
        DarkCyan = ConsoleColor.DarkCyan,
        DarkRed = ConsoleColor.DarkRed,
        DarkMagenta = ConsoleColor.DarkMagenta,
        DarkYellow = ConsoleColor.DarkYellow,
        Gray = ConsoleColor.Gray,
        DarkGray = ConsoleColor.DarkGray,
        Blue = ConsoleColor.Blue,
        Green = ConsoleColor.Green,
        Cyan = ConsoleColor.Cyan,
        Red = ConsoleColor.Red,
        Magenta = ConsoleColor.Magenta,
        Yellow = ConsoleColor.Yellow,
        White = ConsoleColor.White
    }

    public static class TUIHelper
    {
        public static int ConsoleWidth => 80;
        public static int ConsoleHeight => 25;

        public static void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public static void ResetColors()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void DrawBox(Rect rect, char horizontal = '-', char vertical = '|', char corner = '+', ConsoleColor? borderColor = null)
        {
            if (borderColor != null)
            {
                SetColors(borderColor.Value, ConsoleColor.Black);
            }

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                Console.SetCursorPosition(rect.Left, y);

                if (y == rect.Top || y == rect.Bottom - 1)
                {
                    Console.Write(corner);
                    for (int x = rect.Left + 1; x < rect.Right - 1; x++)
                    {
                        Console.Write(horizontal);
                    }
                    Console.Write(corner);
                }
                else
                {
                    Console.Write(vertical);
                    for (int x = rect.Left + 1; x < rect.Right - 1; x++)
                    {
                        Console.Write(' ');
                    }
                    Console.Write(vertical);
                }
            }

            ResetColors();
        }

        public static void DrawBox(Rect rect, string title, ConsoleColor? borderColor = null)
        {
            if (borderColor != null)
            {
                SetColors(borderColor.Value, ConsoleColor.Black);
            }

            char horizontal = '-';
            char vertical = '|';
            char corner = '+';

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                Console.SetCursorPosition(rect.Left, y);

                if (y == rect.Top)
                {
                    Console.Write(corner);
                    for (int x = rect.Left + 1; x < rect.Right - 1; x++)
                    {
                        Console.Write(horizontal);
                    }
                    Console.Write(corner);
                }
                else if (y == rect.Bottom - 1)
                {
                    Console.Write(corner);
                    for (int x = rect.Left + 1; x < rect.Right - 1; x++)
                    {
                        Console.Write(horizontal);
                    }
                    Console.Write(corner);
                }
                else
                {
                    Console.Write(vertical);
                    for (int x = rect.Left + 1; x < rect.Right - 1; x++)
                    {
                        Console.Write(' ');
                    }
                    Console.Write(vertical);
                }
            }

            if (!string.IsNullOrEmpty(title))
            {
                int titleX = rect.Left + (rect.Width - title.Length) / 2;
                Console.SetCursorPosition(titleX, rect.Top);
                Console.Write(title);
            }

            ResetColors();
        }

        public static void DrawText(Point position, string text, ConsoleColor? color = null)
        {
            if (color != null)
            {
                SetColors(color.Value, ConsoleColor.Black);
            }

            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(text);
            ResetColors();
        }

        public static void DrawCenteredText(int y, string text, ConsoleColor? color = null)
        {
            if (color != null)
            {
                SetColors(color.Value, ConsoleColor.Black);
            }

            int x = (ConsoleWidth - text.Length) / 2;
            Console.SetCursorPosition(x, y);
            Console.Write(text);
            ResetColors();
        }

        public static void DrawHorizontalLine(int x, int y, int width, char c = '-', ConsoleColor? color = null)
        {
            if (color != null)
            {
                SetColors(color.Value, ConsoleColor.Black);
            }

            Console.SetCursorPosition(x, y);
            Console.Write(new string(c, width));
            ResetColors();
        }

        public static void DrawVerticalLine(int x, int y, int height, char c = '|', ConsoleColor? color = null)
        {
            if (color != null)
            {
                SetColors(color.Value, ConsoleColor.Black);
            }

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(c);
            }
            ResetColors();
        }

        public static void ClearArea(Rect rect)
        {
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                Console.SetCursorPosition(rect.Left, y);
                Console.Write(new string(' ', rect.Width));
            }
        }

        public static void SaveCursor()
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
        }

        public static void HideCursor()
        {
            Console.CursorVisible = false;
        }

        public static void ShowCursor()
        {
            Console.CursorVisible = true;
        }

        public static string TruncateText(string text, int maxLength)
        {
            if (text.Length <= maxLength)
            {
                return text;
            }
            return text.Substring(0, maxLength - 3) + "...";
        }

        public static string PadText(string text, int totalWidth, char padChar = ' ')
        {
            if (text.Length >= totalWidth)
            {
                return text;
            }
            return text + new string(padChar, totalWidth - text.Length);
        }
    }
}
