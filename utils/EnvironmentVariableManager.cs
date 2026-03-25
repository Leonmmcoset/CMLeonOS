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
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS
{
    public class EnvironmentVariableManager
    {
        private static EnvironmentVariableManager instance;
        private string envFilePath = @"0:\system\env.dat";
        private Dictionary<string, string> environmentVariables;

        private EnvironmentVariableManager()
        {
            environmentVariables = new Dictionary<string, string>();
            LoadEnvironmentVariables();
        }

        public static EnvironmentVariableManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnvironmentVariableManager();
                }
                return instance;
            }
        }

        private void LoadEnvironmentVariables()
        {
            try
            {
                if (File.Exists(envFilePath))
                {
                    string[] lines = File.ReadAllLines(envFilePath);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                        {
                            int equalIndex = line.IndexOf('=');
                            string varName = line.Substring(0, equalIndex).Trim();
                            string varValue = line.Substring(equalIndex + 1).Trim();
                            environmentVariables[varName] = varValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading environment variables: {ex.Message}");
            }
        }

        private void SaveEnvironmentVariables()
        {
            try
            {
                Console.WriteLine($"Saving environment variables to: {envFilePath}");
                Console.WriteLine($"Variables to save: {environmentVariables.Count}");
                
                // 构建文件内容
                string content = "";
                foreach (var kvp in environmentVariables)
                {
                    content += $"{kvp.Key}={kvp.Value}\n";
                }
                
                Console.WriteLine("Environment variables content:");
                Console.WriteLine(content);
                
                // 使用FileSystem的WriteFile方法来确保在Cosmos中正常工作
                // 注意：这里需要访问FileSystem实例，但EnvironmentVariableManager是独立的
                // 所以我们使用File.WriteAllText，但添加重试机制
                
                int retryCount = 0;
                bool success = false;
                
                while (retryCount < 3 && !success)
                {
                    try
                    {
                        File.WriteAllText(envFilePath, content);
                        success = true;
                        Console.WriteLine("Environment variables saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        Console.WriteLine($"Save attempt {retryCount} failed: {ex.Message}");
                        // Thread.Sleep(1000); // 等待1秒后重试
                    }
                }
                
                if (!success)
                {
                    Console.WriteLine("Failed to save environment variables after 3 attempts.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving environment variables: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
            }
        }

        public bool SetVariable(string varName, string varValue)
        {
            if (string.IsNullOrWhiteSpace(varName))
            {
                Console.WriteLine("Error: Variable name cannot be empty");
                return false;
            }

            environmentVariables[varName] = varValue;
            SaveEnvironmentVariables();
            Console.WriteLine($"Environment variable '{varName}' set to '{varValue}'");
            return true;
        }

        public string GetVariable(string varName)
        {
            if (environmentVariables.ContainsKey(varName))
            {
                return environmentVariables[varName];
            }
            else
            {
                Console.WriteLine($"Error: Environment variable '{varName}' not found");
                return null;
            }
        }

        public void ListVariables()
        {
            if (environmentVariables.Count == 0)
            {
                Console.WriteLine("No environment variables set.");
            }
            else
            {
                Console.WriteLine("====================================");
                Console.WriteLine("        Environment Variables");
                Console.WriteLine("====================================");
                Console.WriteLine();
                foreach (var kvp in environmentVariables)
                {
                    Console.WriteLine($"  {kvp.Key}={kvp.Value}");
                }
                Console.WriteLine();
                Console.WriteLine($"Total: {environmentVariables.Count} variables");
            }
        }

        public bool DeleteVariable(string varName)
        {
            if (environmentVariables.ContainsKey(varName))
            {
                environmentVariables.Remove(varName);
                SaveEnvironmentVariables();
                Console.WriteLine($"Environment variable '{varName}' deleted");
                return true;
            }
            else
            {
                Console.WriteLine($"Error: Environment variable '{varName}' not found");
                return false;
            }
        }

        public Dictionary<string, string> GetAllVariables()
        {
            Dictionary<string, string> copy = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in environmentVariables)
            {
                copy[kvp.Key] = kvp.Value;
            }
            return copy;
        }

        public string[] GetAllVariablesAsLines()
        {
            string[] lines = new string[environmentVariables.Count];
            int index = 0;
            foreach (KeyValuePair<string, string> kvp in environmentVariables)
            {
                lines[index] = kvp.Key + "=" + kvp.Value;
                index++;
            }
            return lines;
        }
    }
}
