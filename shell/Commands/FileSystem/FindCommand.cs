using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class FindCommand
    {
        public static void FindFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name to search");
                showError("find <filename>");
                return;
            }
            
            try
            {
                var files = fileSystem.GetFileList(".");
                bool found = false;
                
                foreach (var file in files)
                {
                    if (file.ToLower().Contains(args.ToLower()))
                    {
                        Console.WriteLine($"Found: {file}");
                        found = true;
                    }
                }
                
                if (!found)
                {
                    Console.WriteLine($"No files found matching '{args}'");
                }
            }
            catch (Exception ex)
            {
                showError($"Error finding file: {ex.Message}");
            }
        }
    }
}
