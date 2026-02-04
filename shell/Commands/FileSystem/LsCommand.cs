namespace CMLeonOS.Commands.FileSystem
{
    public static class LsCommand
    {
        public static void ProcessLs(CMLeonOS.FileSystem fileSystem, string args)
        {
            fileSystem.ListFiles(args);
        }
    }
}
