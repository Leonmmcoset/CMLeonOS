// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
