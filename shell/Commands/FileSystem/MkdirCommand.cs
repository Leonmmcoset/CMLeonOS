using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class MkdirCommand
    {
        public static void ProcessMkdir(CMLeonOS.FileSystem fileSystem, string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please specify a directory name");
                Console.ResetColor();
            }
            else
            {
                fileSystem.MakeDirectory(args);
            }
        }
    }
}
