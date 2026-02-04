using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class RenameCommand
    {
        public static void RenameFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify source and new name");
                showError("rename <source> <newname>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    showError("Please specify both source and new name");
                    showError("rename <source> <newname>");
                    return;
                }
                
                string sourceFile = parts[0];
                string newName = parts[1];
                
                string sourcePath = fileSystem.GetFullPath(sourceFile);
                string destPath = fileSystem.GetFullPath(newName);
                
                if (!global::System.IO.File.Exists(sourcePath))
                {
                    showError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                if (global::System.IO.File.Exists(destPath))
                {
                    showError($"Destination '{newName}' already exists");
                    return;
                }
                
                string content = fileSystem.ReadFile(sourcePath);
                global::System.IO.File.WriteAllText(destPath, content);
                fileSystem.DeleteFile(sourcePath);
                
                showSuccess($"File renamed successfully from '{sourceFile}' to '{newName}'");
            }
            catch (Exception ex)
            {
                showError($"Error renaming file: {ex.Message}");
            }
        }
    }
}
