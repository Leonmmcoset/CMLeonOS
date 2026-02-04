using System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class PwdCommand
    {
        public static void ProcessPwd(CMLeonOS.FileSystem fileSystem)
        {
            Console.WriteLine(fileSystem.CurrentDirectory);
        }
    }
}
