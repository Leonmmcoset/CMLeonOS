using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class MoveCommand
    {
        public static void MoveFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify source and destination files");
                showError("mv <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    showError("Please specify both source and destination");
                    showError("mv <source> <destination>");
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
                fileSystem.DeleteFile(sourceFile);
                
                showSuccess($"File moved/renamed successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                showError($"Error moving file: {ex.Message}");
            }
        }
    }
}
