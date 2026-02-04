using System;

namespace CMLeonOS.Commands.Utility
{
    public static class BackgroundCommand
    {
        public static void ChangeBackground(string hexColor, Action<string> showError)
        {
            try
            {
                hexColor = hexColor.TrimStart('#');

                if (hexColor.Length == 6)
                {
                    int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

                    ConsoleColor color = GetClosestConsoleColor(r, g, b);
                    Console.BackgroundColor = color;
                    Console.Clear();
                    Console.WriteLine($"Background color changed to: #{hexColor}");
                }
                else
                {
                    showError("Invalid hex color format. Use format: #RRGGBB or RRGGBB");
                }
            }
            catch (Exception ex)
            {
                showError($"Error changing background color: {ex.Message}");
            }
        }

        private static ConsoleColor GetClosestConsoleColor(int r, int g, int b)
        {
            ConsoleColor[] colors = new ConsoleColor[]
            {
                ConsoleColor.Black,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkYellow,
                ConsoleColor.Gray,
                ConsoleColor.DarkGray,
                ConsoleColor.Blue,
                ConsoleColor.Green,
                ConsoleColor.Cyan,
                ConsoleColor.Red,
                ConsoleColor.Magenta,
                ConsoleColor.Yellow,
                ConsoleColor.White
            };
            ConsoleColor closestColor = ConsoleColor.Black;
            double smallestDistance = double.MaxValue;

            foreach (ConsoleColor color in colors)
            {
                int cr, cg, cb;
                GetRGBFromConsoleColor(color, out cr, out cg, out cb);

                double distance = Math.Sqrt(Math.Pow(r - cr, 2) + Math.Pow(g - cg, 2) + Math.Pow(b - cb, 2));
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestColor = color;
                }
            }

            return closestColor;
        }

        private static void GetRGBFromConsoleColor(ConsoleColor color, out int r, out int g, out int b)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    r = 0; g = 0; b = 0; break;
                case ConsoleColor.DarkBlue:
                    r = 0; g = 0; b = 128; break;
                case ConsoleColor.DarkGreen:
                    r = 0; g = 128; b = 0; break;
                case ConsoleColor.DarkCyan:
                    r = 0; g = 128; b = 128; break;
                case ConsoleColor.DarkRed:
                    r = 128; g = 0; b = 0; break;
                case ConsoleColor.DarkMagenta:
                    r = 128; g = 0; b = 128; break;
                case ConsoleColor.DarkYellow:
                    r = 128; g = 128; b = 0; break;
                case ConsoleColor.Gray:
                    r = 192; g = 192; b = 192; break;
                case ConsoleColor.DarkGray:
                    r = 128; g = 128; b = 128; break;
                case ConsoleColor.Blue:
                    r = 0; g = 0; b = 255; break;
                case ConsoleColor.Green:
                    r = 0; g = 255; b = 0; break;
                case ConsoleColor.Cyan:
                    r = 0; g = 255; b = 255; break;
                case ConsoleColor.Red:
                    r = 255; g = 0; b = 0; break;
                case ConsoleColor.Magenta:
                    r = 255; g = 0; b = 255; break;
                case ConsoleColor.Yellow:
                    r = 255; g = 255; b = 0; break;
                case ConsoleColor.White:
                    r = 255; g = 255; b = 255; break;
                default:
                    r = 0; g = 0; b = 0; break;
            }
        }
    }
}
