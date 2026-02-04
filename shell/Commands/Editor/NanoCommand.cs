using System;

namespace CMLeonOS.Commands.Editor
{
    public static class NanoCommand
    {
        public static void NanoFile(string fileName, CMLeonOS.FileSystem fileSystem, CMLeonOS.UserSystem userSystem, Shell shell, Action<string> showError)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                showError("Please specify a file name");
                return;
            }
            
            try
            {
                var nano = new Nano(fileName, true, fileSystem, userSystem, shell);
                nano.Start();
            }
            catch (Exception ex)
            {
                showError($"Error starting nano: {ex.Message}");
            }
        }
    }
}
