using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CMLeonOS;

namespace CMLeonOS.Commands.Script
{
    public static class SCLCommand
    {
        public static void ExecuteSCLScript(string args, CMLeonOS.FileSystem fileSystem, Shell shell, Action<string> showError, Action<string> showWarning)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                    {
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "<file>", 
                            Description = "Execute SCL (SunsetCodeLang) script file",
                            IsOptional = false 
                        },
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "--shell", 
                            Description = "Enter SCL interactive shell",
                            IsOptional = false 
                        }
                    };

                showError(UsageGenerator.GenerateUsage("scl", commandInfos));
                return;
            }
            
            if (parts.Length == 1 && parts[0] == "--shell")
            {
                EnterSCLShell(showError);
                return;
            }
            
            string filePath = parts[0];
            string originalPath = filePath;
            
            if (!filePath.StartsWith("0:\\") && !filePath.StartsWith("0:/"))
            {
                string currentDir = fileSystem.CurrentDirectory;
                if (currentDir == "/" || currentDir == "\\")
                {
                    filePath = "0:\\" + filePath.TrimStart('/').TrimStart('\\');
                }
                else
                {
                    filePath = Path.Combine(currentDir, filePath);
                }
            }
            
            if (!File.Exists(filePath))
            {
                showError($"Error: File not found: {filePath}");
                return;
            }
            
            try
            {
                string scriptContent = File.ReadAllText(filePath);
                
                if (string.IsNullOrWhiteSpace(scriptContent))
                {
                    showWarning("Script file is empty");
                    return;
                }
                
                ExecuteSCLWithPython(scriptContent, filePath, showError);
            }
            catch (Exception ex)
            {
                showError($"SCL execution error: {ex.Message}");
            }
        }

        private static void ExecuteSCLWithPython(string scriptContent, string scriptPath, Action<string> showError)
        {
            try
            {
                string pythonPath = FindPythonExecutable();
                
                if (string.IsNullOrEmpty(pythonPath))
                {
                    showError("Error: Python not found. Please install Python 3.6 or higher.");
                    return;
                }
                
                string tempScriptPath = Path.Combine(Path.GetTempPath(), $"scl_temp_{Guid.NewGuid()}.py");
                
                try
                {
                    File.WriteAllText(tempScriptPath, scriptContent);
                    
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = $"\"{tempScriptPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(scriptPath) ?? Path.GetTempPath()
                    };
                    
                    using (var process = Process.Start(processStartInfo))
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        
                        if (!string.IsNullOrEmpty(output))
                        {
                            Console.Write(output);
                        }
                        
                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(error);
                            Console.ResetColor();
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempScriptPath))
                    {
                        File.Delete(tempScriptPath);
                    }
                }
            }
            catch (Exception ex)
            {
                showError($"Failed to execute SCL script: {ex.Message}");
            }
        }

        private static string FindPythonExecutable()
        {
            string[] possiblePaths = new string[]
            {
                "python",
                "python3",
                @"C:\Python312\python.exe",
                @"C:\Python311\python.exe",
                @"C:\Python310\python.exe",
                @"C:\Python39\python.exe",
                @"C:\Python38\python.exe"
            };
            
            foreach (string path in possiblePaths)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    
                    using (var process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            return path;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            return null;
        }

        private static void EnterSCLShell(Action<string> showError)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("     SCL Interactive Shell");
            Console.WriteLine("====================================");
            Console.WriteLine("Type 'exit' or 'quit' to exit");
            Console.WriteLine();
            
            string pythonPath = FindPythonExecutable();
            
            if (string.IsNullOrEmpty(pythonPath))
            {
                showError("Error: Python not found. Please install Python 3.6 or higher.");
                return;
            }
            
            string tempScriptPath = Path.Combine(Path.GetTempPath(), $"scl_shell_{Guid.NewGuid()}.py");
            
            try
            {
                while (true)
                {
                    Console.Write("scl> ");
                    string input = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }
                    
                    if (input.ToLower() == "exit" || input.ToLower() == "quit")
                    {
                        Console.WriteLine("Exiting SCL shell...");
                        break;
                    }
                    
                    try
                    {
                        File.WriteAllText(tempScriptPath, input);
                        
                        var processStartInfo = new ProcessStartInfo
                        {
                            FileName = pythonPath,
                            Arguments = $"\"{tempScriptPath}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };
                        
                        using (var process = Process.Start(processStartInfo))
                        {
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            process.WaitForExit();
                            
                            if (!string.IsNullOrEmpty(output))
                            {
                                Console.Write(output);
                            }
                            
                            if (!string.IsNullOrEmpty(error))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(error);
                                Console.ResetColor();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        showError($"SCL error: {ex.Message}");
                    }
                }
            }
            finally
            {
                if (File.Exists(tempScriptPath))
                {
                    File.Delete(tempScriptPath);
                }
            }
        }
    }
}