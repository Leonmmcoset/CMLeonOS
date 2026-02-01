using System;

namespace CMLeonOS
{
    public static class Version
    {
        public static string Major = "1";
        public static string Minor = "0";
        public static string Patch = "0";
        public static string Build = "0";
        public static string Revision = "0";
        
        public static string FullVersion
        {
            get { return $"{Major}.{Minor}.{Patch}.{Build}.{Revision}"; }
        }
        
        public static string ShortVersion
        {
            get { return $"{Major}.{Minor}.{Patch}"; }
        }
        
        public static string DisplayVersion
        {
            get { return $"CMLeonOS v{ShortVersion}"; }
        }
    }
}
