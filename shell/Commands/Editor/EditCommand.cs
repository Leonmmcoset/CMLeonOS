using System;

namespace CMLeonOS.Commands.Editor
{
    public static class EditCommand
    {
        public static void EditFile(string fileName, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                showError("Please specify a file name");
                return;
            }
            
            try
            {
                var nano = new CMLeonOS.Nano(fileName, true, fileSystem);
                nano.Start();
            }
            catch (Exception ex)
            {
                showError($"Error starting editor: {ex.Message}");
            }
        }
    }
}
