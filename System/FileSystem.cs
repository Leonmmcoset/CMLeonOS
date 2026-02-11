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
                    // 列出当前目录下的文件和子目录
                    string displayPath = path == "." ? CurrentDirectory : path;
                    Console.WriteLine($"Contents of {displayPath}:");
                    
                    // 列出子目录
                    try
                    {
                        var dirs = Directory.GetDirectories(fullPath);
                        foreach (var dir in dirs)
                        {
                            // 使用Path.GetFileName获取目录名，避免Substring可能导致的问题
                            string dirName = Path.GetFileName(dir);
                            Console.WriteLine($"[DIR]  {dirName}");
                        }
                    }
                    catch
                    {
                        // 可能没有权限或其他错误
                    }
                    
                    // 列出文件
                    try
                    {
                        var files = Directory.GetFiles(fullPath);
                        foreach (var file in files)
                        {
                            // 使用Path.GetFileName获取文件名，避免Substring可能导致的问题
                            string fileName = Path.GetFileName(file);
                            Console.WriteLine($"[FILE] {fileName}");
                        }
                    }
                    catch
                    {
                        // 可能没有权限或其他错误
                    }
                }
                else
                {
                    Console.WriteLine($"Directory not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing files: {ex.Message}");
            }
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
                    return File.ReadAllText(fullPath);
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
            
            if (path.StartsWith(@"0:\"))
            {
                return path;
            }
            
            if (path == ".")
            {
                return currentDirectory;
            }
            
            if (path == "..")
            {
                if (currentDirectory == @"0:\")
                {
                    return @"0:\";
                }
                else
                {
                    int lastSlash = currentDirectory.LastIndexOf('\\');
                    if (lastSlash == 2) // 0:\
                    {
                        return @"0:\";
                    }
                    else
                    {
                        return currentDirectory.Substring(0, lastSlash);
                    }
                }
            }
            
            if (path.StartsWith("../") || path.StartsWith("..\\"))
            {
                if (currentDirectory == @"0:\")
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
                    if (lastSlash == 2) // 0:\
                    {
                        resultPath = @"0:\";
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
