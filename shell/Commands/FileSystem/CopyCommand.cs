using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class CopyCommand
    {
        public static void CopyFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify source and destination files");
                showError("cp <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    showError("Please specify both source and destination");
                    showError("cp <source> <destination>");
                    return;
                }
                
                string sourceFile = parts[0];
                string destFile = parts[1];
                
                string content = fileSystem.ReadFile(sourceFile);
                if (content == null)
                {
                    showError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                fileSystem.WriteFile(destFile, content);
                showSuccess($"File copied successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                showError($"Error copying file: {ex.Message}");
            }
        }
    }
}
