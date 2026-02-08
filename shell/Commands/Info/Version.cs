using System;

namespace CMLeonOS.Commands
{
    public static class VersionCommand
    {
        public static void ProcessVersion()
        {
            Console.WriteLine(Version.DisplayVersionWithGit);
            Console.WriteLine($"Major: {Version.Major}");
            Console.WriteLine($"Minor: {Version.Minor}");
            Console.WriteLine($"Patch: {Version.Patch}");
            Console.WriteLine($"Type: {Version.VersionType}");
            Console.WriteLine($"Full Version: {Version.FullVersion}");
            Console.WriteLine($"Git Commit: {Version.GitCommit}");
        }
    }
}
