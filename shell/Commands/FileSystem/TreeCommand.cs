using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class TreeCommand
    {
        public static void ShowTree(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            string startPath = string.IsNullOrEmpty(args) ? "." : args;
            string fullPath = fileSystem.GetFullPath(startPath);
            
            if (!System.IO.Directory.Exists(fullPath))
            {
                showError($"Directory not found: {startPath}");
                return;
            }
            
            try
            {
                Console.WriteLine(fullPath);
                PrintDirectoryTree(fileSystem, fullPath, "", true, showError);
            }
            catch (Exception ex)
            {
                showError($"Error displaying tree: {ex.Message}");
            }
        }

        private static void PrintDirectoryTree(CMLeonOS.FileSystem fileSystem, string path, string prefix, bool isLast, Action<string> showError)
        {
            try
            {
                var dirs = fileSystem.GetFullPathDirectoryList(path);
                var files = fileSystem.GetFullPathFileList(path);
                
                int totalItems = dirs.Count + files.Count;
                int current = 0;
                
                foreach (var dir in dirs)
                {
                    current++;
                    bool isLastItem = current == totalItems;
                    string connector = isLastItem ? "+-- " : "|-- ";
                    string newPrefix = prefix + (isLastItem ? "    " : "|   ");
                    
                    string dirName = System.IO.Path.GetFileName(dir);
                    Console.WriteLine($"{prefix}{connector}{dirName}/");
                    
                    PrintDirectoryTree(fileSystem, dir, newPrefix, isLastItem, showError);
                }
                
                foreach (var file in files)
                {
                    current++;
                    bool isLastItem = current == totalItems;
                    string connector = isLastItem ? "+-- " : "|-- ";
                    
                    string fileName = System.IO.Path.GetFileName(file);
                    Console.WriteLine($"{prefix}{connector}{fileName}");
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                showError($"Directory not found: {path}");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = $"Error type: {ex.GetType().Name}";
                }
                showError($"Error reading directory {path}: {errorMsg}");
            }
        }
    }
}
