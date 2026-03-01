using System;
using System.IO;
using System.Text;

namespace CMLeonOS.Utils
{
    public static class FileSecurity
    {
        public static bool IsSecure(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string fileName = Path.GetFileName(path);

            string[] insecureExtensions = { ".exe", ".bat", ".cmd", ".sh", ".ps1" };
            string extension = Path.GetExtension(fileName).ToLower();

            foreach (string insecureExt in insecureExtensions)
            {
                if (extension == insecureExt)
                {
                    return false;
                }
            }

            string[] insecureNames = { "autoexec", "config", "system" };
            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName).ToLower();

            foreach (string insecureName in insecureNames)
            {
                if (nameWithoutExt.Contains(insecureName))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetSecurityLevel(string path)
        {
            if (!IsSecure(path))
            {
                return "Insecure";
            }
            return "Secure";
        }

        public static bool CanExecute(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string extension = Path.GetExtension(path).ToLower();
            string[] executableExtensions = { ".exe", ".bat", ".cmd", ".sh", ".ps1" };

            foreach (string execExt in executableExtensions)
            {
                if (extension == execExt)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanRead(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static bool CanAccess(string path)
        {
            return CanRead(path) && CanWrite(path);
        }

        public static bool CanWrite(string path)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    return false;
                }

                string testFile = Path.Combine(dir, ".write_test");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
