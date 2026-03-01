using System;
using System.IO;
using System.Text;

namespace CMLeonOS.Utils
{
    public static class PathUtil
    {
        public static string Sanitize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            string sanitized = path;

            sanitized = sanitized.Replace('\\', '/');
            sanitized = sanitized.Replace(':', '_');
            sanitized = sanitized.Replace('*', '_');
            sanitized = sanitized.Replace('?', '_');
            sanitized = sanitized.Replace('"', '_');
            sanitized = sanitized.Replace('<', '_');
            sanitized = sanitized.Replace('>', '_');
            sanitized = sanitized.Replace('|', '_');

            sanitized = sanitized.Trim('/', '\\');

            return sanitized;
        }

        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            string normalized = path.Replace('\\', '/');
            
            while (normalized.Contains("//"))
            {
                normalized = normalized.Replace("//", "/");
            }

            return normalized;
        }
    }
}
