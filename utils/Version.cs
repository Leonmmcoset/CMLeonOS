using System;

namespace CMLeonOS
{
    public static class Version
    {
        public static string Major = "1";
        public static string Minor = "0";
        public static string Patch = "0";
        public static string VersionType = "PreRelease";
        public static string GitCommit = "unknown";
        
        public static string FullVersion
        {
            get { return $"{Major}.{Minor}.{Patch}-{VersionType}"; }
        }
        
        public static string ShortVersion
        {
            get { return $"{Major}.{Minor}.{Patch}"; }
        }
        
        public static string DisplayVersion
        {
            get { return $"CMLeonOS v{ShortVersion} ({VersionType})"; }
        }
        
        public static string DisplayVersionWithGit
        {
            get { return $"CMLeonOS v{ShortVersion} ({VersionType}) - Git: {GitCommit}"; }
        }
    }
}
