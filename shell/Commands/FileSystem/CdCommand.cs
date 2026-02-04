namespace CMLeonOS.Commands.FileSystem
{
    public static class CdCommand
    {
        public static void ProcessCd(CMLeonOS.FileSystem fileSystem, string args)
        {
            fileSystem.ChangeDirectory(args);
        }
    }
}
