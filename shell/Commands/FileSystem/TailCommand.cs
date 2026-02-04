using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class TailCommand
    {
        public static void TailFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string fileName = parts[0];
                int lineCount = 10;
                
                if (parts.Length > 1)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        lineCount = count;
                    }
                }
                
                string content = fileSystem.ReadFile(fileName);
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty");
                    return;
                }
                
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int displayLines = Math.Min(lineCount, lines.Length);
                int startLine = Math.Max(0, lines.Length - displayLines);
                
                Console.WriteLine($"Last {displayLines} lines of {fileName}:");
                Console.WriteLine("--------------------------------");
                
                for (int i = startLine; i < lines.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {lines[i]}");
                }
            }
            catch (Exception ex)
            {
                showError($"Error reading file: {ex.Message}");
            }
        }
    }
}
