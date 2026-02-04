using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class RmdirCommand
    {
        public static void ProcessRmdir(CMLeonOS.FileSystem fileSystem, string args, bool fixMode, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a directory name");
            }
            else
            {
                bool isInSysFolder = (args.Contains(@"\system\") || args.Contains(@"/sys/")) && !fixMode;
                
                if (isInSysFolder)
                {
                    showError("Cannot delete directories in sys folder");
                    showError("Use fix mode to bypass this restriction");
                }
                else
                {
                    fileSystem.DeleteDirectory(args);
                }
            }
        }
    }
}
