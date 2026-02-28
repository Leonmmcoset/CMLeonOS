using System;
using System.IO;

namespace CMLeonOS.Commands.Utility
{
    public static class HexCommand
    {
        public static void EditHexFile(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Usage: hex <filename>");
                    Console.WriteLine("Example: hex test.bin");
                    Console.WriteLine();
                    Console.WriteLine("Controls:");
                    Console.WriteLine("  Arrow Keys - Move cursor");
                    Console.WriteLine("  Page Up/Down - Scroll view");
                    Console.WriteLine("  0-9, A-F - Edit byte");
                    Console.WriteLine("  S - Save file");
                    Console.WriteLine("  Q - Quit");
                    Console.ResetColor();
                    return;
                }

                string filePath = args.Trim();
                
                if (!File.Exists(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File not found: {filePath}");
                    Console.ResetColor();
                    return;
                }

                var editor = new HexEditor(filePath);
                editor.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error opening hex editor: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
