using System;

namespace CMLeonOS.Commands
{
    public static class VersionCommand
    {
        public static void ProcessVersion()
        {
            Console.WriteLine(Version.DisplayVersion);
            Console.WriteLine($"Major: {Version.Major}");
            Console.WriteLine($"Minor: {Version.Minor}");
            Console.WriteLine($"Patch: {Version.Patch}");
            Console.WriteLine($"Full Version: {Version.FullVersion}");
        }
    }
}
