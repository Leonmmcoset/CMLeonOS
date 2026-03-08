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
    public static class PathUtil
    {
        public static string Sanitize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            string sanitized = path;

            // 保留驱动器字母和冒号（如 0:）
            // 检查格式：X:\ 其中 X 是单个字符（字母或数字）
            bool hasDrive = sanitized.Length >= 2 && sanitized[1] == ':';
            string drive = hasDrive ? sanitized.Substring(0, 2) : "";
            string rest = hasDrive ? sanitized.Substring(2) : sanitized;

            // 不替换反斜杠，因为 Cosmos 文件系统需要反斜杠
            rest = rest.Replace(':', '_');
            rest = rest.Replace('*', '_');
            rest = rest.Replace('?', '_');
            rest = rest.Replace('"', '_');
            rest = rest.Replace('<', '_');
            rest = rest.Replace('>', '_');
            rest = rest.Replace('|', '_');

            // 只移除结尾的斜杠，保留开头的斜杠（因为驱动器后面需要斜杠）
            rest = rest.TrimEnd('/');
            rest = rest.TrimEnd('\\');

            return drive + rest;
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
