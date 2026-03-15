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

using CosmosFtpServer;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS
{
    public class FileSystem
    {
        private string currentDirectory;

        public FileSystem()
        {
            currentDirectory = @"0:\";
        }

        private static bool ContainsInvalidChars(string input)
        {
            char[] invalidChars = { '<', '>', '"', '|', '?', '*' };
            foreach (char c in invalidChars)
            {
                if (input.Contains(c.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public FileSystem(string initialPath)
        {
            if (string.IsNullOrEmpty(initialPath))
            {
                currentDirectory = @"0:\";
            }
            else
            {
                currentDirectory = initialPath;
            }
        }

        public string CurrentDirectory
        {
            get
            {
                // if (currentDirectory == @"0:\apps")
                // {
                //     return ":";
                // }
                return currentDirectory;
            }
        }

        public void ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                currentDirectory = @"0:\";
                return;
            }

            if (path == @"0:\")
            {
                currentDirectory = @"0:\";
                return;
            }

            if (path == @"1:\")
            {
                currentDirectory = @"1:\";
                return;
            }

            string fullPath = GetFullPath(path);
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    currentDirectory = fullPath;
                }
                else
                {
                    Console.WriteLine($"Directory not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing directory: {ex.Message}");
            }
        }

        public void MakeDirectory(string path)
        {
            if (ContainsInvalidChars(path))
            {
                Console.WriteLine("Error: Directory name contains invalid characters: < > : \" | ?");
                return;
            }
            
            string fullPath = GetFullPath(path);
            
            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    Console.WriteLine($"Directory created: {path}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory: {ex.Message}");
            }
        }

        public void ListFiles(string path = ".")
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    string displayPath = path == "." ? CurrentDirectory : path;
                    Console.WriteLine($"Contents of {fullPath}:");
                    
                    try
                    {
                        var dirs = Directory.GetDirectories(fullPath);
                        foreach (var dir in dirs)
                        {
                            string dirName = Path.GetFileName(dir);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[DIR]  {dirName}");
                            Console.ResetColor();
                        }
                    }
                    catch
                    {
                    }
                    
                    try
                    {
                        var files = Directory.GetFiles(fullPath);
                        foreach (var file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string extension = Path.GetExtension(fileName)?.ToLower();
                            
                            ConsoleColor fileColor = GetFileColor(extension);
                            
                            Console.ForegroundColor = fileColor;
                            Console.WriteLine($"[FILE] {fileName}");
                            Console.ResetColor();
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Directory not found: {path}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error listing files: {ex.Message}");
                Console.ResetColor();
            }
        }

        private ConsoleColor GetFileColor(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return ConsoleColor.Gray;
            
            return extension switch
            {
                // 这是一堆后缀名的颜色标注，但是许多后缀名还不支持，先保留
                ".exe" => ConsoleColor.Green,
                ".cla" => ConsoleColor.Green,
                ".lua" => ConsoleColor.Cyan,
                ".js" => ConsoleColor.Cyan,
                ".py" => ConsoleColor.Cyan,
                ".sh" => ConsoleColor.Cyan,
                ".bat" => ConsoleColor.Cyan,
                ".cmd" => ConsoleColor.Cyan,
                ".ps1" => ConsoleColor.Cyan,
                ".json" => ConsoleColor.Magenta,
                ".xml" => ConsoleColor.Magenta,
                ".ini" => ConsoleColor.Magenta,
                ".cfg" => ConsoleColor.Magenta,
                ".conf" => ConsoleColor.Magenta,
                ".txt" => ConsoleColor.White,
                ".md" => ConsoleColor.White,
                ".rtf" => ConsoleColor.White,
                ".bmp" => ConsoleColor.Blue,
                ".png" => ConsoleColor.Blue,
                ".jpg" => ConsoleColor.Blue,
                ".jpeg" => ConsoleColor.Blue,
                ".gif" => ConsoleColor.Blue,
                ".zip" => ConsoleColor.Red,
                ".rar" => ConsoleColor.Red,
                ".7z" => ConsoleColor.Red,
                ".tar" => ConsoleColor.Red,
                ".gz" => ConsoleColor.Red,
                ".mi" => ConsoleColor.DarkYellow,
                ".dat" => ConsoleColor.DarkBlue,
                _ => ConsoleColor.Gray
            };
        }

        public List<string> GetFileList(string path = ".")
        {
            string fullPath = GetFullPath(path);
            List<string> fileList = new List<string>();
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    // 获取文件列表
                    var files = Directory.GetFiles(fullPath);
                    foreach (var file in files)
                    {
                        // 使用Path.GetFileName获取文件名
                        string fileName = Path.GetFileName(file);
                        fileList.Add(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file list: {ex.Message}");
            }
            
            return fileList;
        }

        public List<string> GetDirectoryList(string path = ".")
        {
            string fullPath = GetFullPath(path);
            List<string> dirList = new List<string>();
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    var dirs = Directory.GetDirectories(fullPath);
                    foreach (var dir in dirs)
                    {
                        string dirName = Path.GetFileName(dir);
                        dirList.Add(dirName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting directory list: {ex.Message}");
            }
            
            return dirList;
        }

        public List<string> GetFullPathFileList(string path = ".")
        {
            string fullPath = GetFullPath(path);
            List<string> fileList = new List<string>();
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    var files = Directory.GetFiles(fullPath);
                    foreach (var file in files)
                    {
                        fileList.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file list: {ex.Message}");
            }
            
            return fileList;
        }

        public List<string> GetFullPathDirectoryList(string path = ".")
        {
            string fullPath = GetFullPath(path);
            List<string> dirList = new List<string>();
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    var dirs = Directory.GetDirectories(fullPath);
                    foreach (var dir in dirs)
                    {
                        dirList.Add(dir);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting directory list: {ex.Message}");
            }
            
            return dirList;
        }

        public void CreateFile(string path, string content = "")
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                // 创建或覆盖文件
                File.WriteAllText(fullPath, content);
                Console.WriteLine($"File created: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
            }
        }

        public void WriteFile(string path, string content)
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                if (File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, content);
                    Console.WriteLine($"File written: {path}");
                }
                else
                {
                    Console.WriteLine($"File not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
            }
        }

        public string ReadFile(string path)
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                if (File.Exists(fullPath))
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        return sr.ReadToEnd();
                    }
                }
                else
                {
                    Console.WriteLine($"File not found: {path}");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return "";
            }
        }

        public void DeleteFile(string path)
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"File deleted: {path}");
                }
                else
                {
                    Console.WriteLine($"File not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }

        public void DeleteDirectory(string path)
        {
            string fullPath = GetFullPath(path);
            
            try
            {
                if (Directory.Exists(fullPath))
                {
                    try
                    {
                        // 尝试删除目录
                        Directory.Delete(fullPath);
                        Console.WriteLine($"Directory deleted: {path}");
                    }
                    catch
                    {
                        // 目录可能不为空
                        Console.WriteLine($"Directory not empty: {path}");
                    }
                }
                else
                {
                    Console.WriteLine($"Directory not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting directory: {ex.Message}");
            }
        }

        public string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return currentDirectory;
            }
            
            if (path.Length > 255)
            {
                Console.WriteLine("Error: Path too long (maximum 255 characters)");
                return currentDirectory;
            }
            
            //char[] invalidChars = { '<', '>', ':', '"', '|', '?', '*' };
            //foreach (char c in invalidChars)
            //{
            //    if (path.Contains(c.ToString()))
            //    {
            //        Console.WriteLine($"Error: Invalid character in path: '{c}'");
            //        return currentDirectory;
            //    }
            //}
            
            if (path.Contains("//") || path.Contains("\\\\"))
            {
                Console.WriteLine("Error: Path contains consecutive slashes");
                return currentDirectory;
            }
            
            if (path.StartsWith(@"0:\"))
            {
                return path;
            }
            
            if (path.StartsWith(@"1:\"))
            {
                return path;
            }
            
            if (path == ".")
            {
                return currentDirectory;
            }
            
            if (path == "..")
            {
                if (currentDirectory == @"0:\" || currentDirectory == @"1:\")
                {
                    return currentDirectory;
                }
                else
                {
                    int lastSlash = currentDirectory.LastIndexOf('\\');
                    if (lastSlash == 2) // 0:\ or 1:\
                    {
                        return currentDirectory.Substring(0, 3);
                    }
                    else
                    {
                        return currentDirectory.Substring(0, lastSlash);
                    }
                }
            }
            
            if (path.StartsWith("../") || path.StartsWith("..\\"))
            {
                if (currentDirectory == @"0:\" || currentDirectory == @"1:\")
                {
                    Console.WriteLine("Error: Cannot go above root directory");
                    return currentDirectory;
                }
                
                int level = 0;
                string tempPath = path;
                while (tempPath.StartsWith("../") || tempPath.StartsWith("..\\"))
                {
                    level++;
                    if (tempPath.StartsWith("../"))
                    {
                        tempPath = tempPath.Substring(3);
                    }
                    else if (tempPath.StartsWith("..\\"))
                    {
                        tempPath = tempPath.Substring(3);
                    }
                    
                    if (level > 10)
                    {
                        Console.WriteLine("Error: Too many parent directory references");
                        return currentDirectory;
                    }
                }
                
                string resultPath = currentDirectory;
                for (int i = 0; i < level; i++)
                {
                    int lastSlash = resultPath.LastIndexOf('\\');
                    if (lastSlash == 2) // 0:\ or 1:\
                    {
                        resultPath = resultPath.Substring(0, 3);
                    }
                    else
                    {
                        resultPath = resultPath.Substring(0, lastSlash);
                    }
                }
                return resultPath;
            }
            
            if (path.StartsWith("dir") || path.StartsWith("DIR"))
            {
                string dirName = path;
                
                string numberPart = "";
                for (int i = 3; i < path.Length; i++)
                {
                    if (char.IsDigit(path[i]))
                    {
                        numberPart += path[i];
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (currentDirectory == @"0:\")
                {
                    return $@"0:\{dirName}";
                }
                else
                {
                    return $@"{currentDirectory}\{dirName}";
                }
            }
            
            string normalizedPath = path;
            
            while (normalizedPath.Contains("//"))
            {
                normalizedPath = normalizedPath.Replace("//", "/");
            }
            
            while (normalizedPath.Contains("\\\\"))
            {
                normalizedPath = normalizedPath.Replace("\\\\", "\\");
            }
            
            if (normalizedPath.StartsWith("/"))
            {
                normalizedPath = normalizedPath.Substring(1);
            }
            
            if (normalizedPath.StartsWith("\\"))
            {
                normalizedPath = normalizedPath.Substring(1);
            }
            
            if (normalizedPath.EndsWith("/"))
            {
                normalizedPath = normalizedPath.Substring(0, normalizedPath.Length - 1);
            }
            
            if (normalizedPath.EndsWith("\\"))
            {
                normalizedPath = normalizedPath.Substring(0, normalizedPath.Length - 1);
            }
            
            if (currentDirectory == @"0:\")
            {
                return $@"0:\{normalizedPath}";
            }
            else
            {
                return $@"{currentDirectory}\{normalizedPath}";
            }
        }
    }
}
