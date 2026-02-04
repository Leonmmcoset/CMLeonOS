using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class TouchCommand
    {
        public static void CreateEmptyFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name");
                showError("touch <filename>");
                return;
            }
            
            try
            {
                fileSystem.WriteFile(args, "");
                showSuccess($"Empty file '{args}' created successfully");
            }
            catch (Exception ex)
            {
                showError($"Error creating file: {ex.Message}");
            }
        }
    }
}
