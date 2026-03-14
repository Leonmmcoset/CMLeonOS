using System;

namespace CMLeonOS.Commands
{
    public static class MarkitCommand
    {
        public static void Execute(string args, Shell shell, CMLeonOS.FileSystem fileSystem)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Usage: markit <filename>");
                Console.WriteLine("Markit syntax:");
                Console.WriteLine("  {color:red}Red text{/color}");
                Console.WriteLine("  {bg:blue}Blue background{/bg}");
                Console.WriteLine("  {color:green}{bg:yellow}Green text on yellow background{/bg}{/color}");
                return;
            }

            string filePath = args.Trim();

            try
            {
                string content = fileSystem.ReadFile(filePath);
                if (content == null)
                {
                    Console.WriteLine($"Error: File '{filePath}' not found.");
                    return;
                }
                RenderMarkit(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }

        private static void RenderMarkit(string content)
        {
            int i = 0;
            while (i < content.Length)
            {
                if (content.Substring(i).StartsWith("{color:"))
                {
                    int end = content.IndexOf('}', i);
                    if (end > i + 7)
                    {
                        string colorName = content.Substring(i + 7, end - i - 7).ToLower();
                        ConsoleColor color = ParseColor(colorName);
                        Console.ForegroundColor = color;
                        i = end + 1;
                        continue;
                    }
                }
                else if (content.Substring(i).StartsWith("{bg:"))
                {
                    int end = content.IndexOf('}', i);
                    if (end > i + 4)
                    {
                        string colorName = content.Substring(i + 4, end - i - 4).ToLower();
                        ConsoleColor color = ParseColor(colorName);
                        Console.BackgroundColor = color;
                        i = end + 1;
                        continue;
                    }
                }
                else if (content.Substring(i).StartsWith("{/color}"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    i += 8;
                    continue;
                }
                else if (content.Substring(i).StartsWith("{/bg}"))
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    i += 5;
                    continue;
                }

                Console.Write(content[i]);
                i++;
            }

            Console.ResetColor();
        }

        private static ConsoleColor ParseColor(string colorName)
        {
            if (colorName == "black") return ConsoleColor.Black;
            if (colorName == "darkblue") return ConsoleColor.DarkBlue;
            if (colorName == "darkgreen") return ConsoleColor.DarkGreen;
            if (colorName == "darkcyan") return ConsoleColor.DarkCyan;
            if (colorName == "darkred") return ConsoleColor.DarkRed;
            if (colorName == "darkmagenta") return ConsoleColor.DarkMagenta;
            if (colorName == "darkyellow") return ConsoleColor.DarkYellow;
            if (colorName == "gray") return ConsoleColor.Gray;
            if (colorName == "darkgray") return ConsoleColor.DarkGray;
            if (colorName == "blue") return ConsoleColor.Blue;
            if (colorName == "green") return ConsoleColor.Green;
            if (colorName == "cyan") return ConsoleColor.Cyan;
            if (colorName == "red") return ConsoleColor.Red;
            if (colorName == "magenta") return ConsoleColor.Magenta;
            if (colorName == "yellow") return ConsoleColor.Yellow;
            if (colorName == "white") return ConsoleColor.White;
            return ConsoleColor.White;
        }
    }
}