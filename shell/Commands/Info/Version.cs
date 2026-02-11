using System;
using IL2CPU.API.Attribs;

namespace CMLeonOS.Commands
{
    public static class VersionCommand
    {
        [ManifestResourceStream(ResourceName = "CMLeonOS.BuildTime.txt")]
        private static byte[] buildTimeResource;

        public static void ProcessVersion()
        {
            string buildTime = global::System.Text.Encoding.UTF8.GetString(buildTimeResource);
            Console.WriteLine(Version.DisplayVersionWithGit);
            Console.WriteLine($"Major: {Version.Major}");
            Console.WriteLine($"Minor: {Version.Minor}");
            Console.WriteLine($"Patch: {Version.Patch}");
            Console.WriteLine($"Type: {Version.VersionType}");
            Console.WriteLine($"Full Version: {Version.FullVersion}");
            Console.WriteLine($"Git Commit: {Version.GitCommit}");
            Console.WriteLine($"Build Time: {buildTime}");
        }
    }
}
