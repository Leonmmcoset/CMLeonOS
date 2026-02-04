using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class CatCommand
    {
        public static void ProcessCat(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name");
            }
            else
            {
                Console.WriteLine(fileSystem.ReadFile(args));
            }
        }
    }
}
