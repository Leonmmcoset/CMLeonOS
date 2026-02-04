using System;
using System.Collections.Generic;
using System.IO;
using CMLeonOS.Logger;

namespace CMLeonOS
{
    public class EnvironmentVariableManager
    {
        private static EnvironmentVariableManager instance;
        private string envFilePath = @"0:\system\env.dat";
        private Dictionary<string, string> environmentVariables;
        private static CMLeonOS.Logger.Logger _logger = CMLeonOS.Logger.Logger.Instance;

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
                    _logger.Info("EnvManager", $"Loaded {environmentVariables.Count} environment variables from file");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EnvManager", $"Error loading environment variables: {ex.Message}");
            }
        }

        private void SaveEnvironmentVariables()
        {
            try
            {
                _logger.Info("EnvManager", $"Saving {environmentVariables.Count} environment variables to file");
                
                // 构建文件内容
                string content = "";
                foreach (var kvp in environmentVariables)
                {
                    content += $"{kvp.Key}={kvp.Value}\n";
                }
                
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
                        _logger.Info("EnvManager", "Environment variables saved successfully");
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        _logger.Warning("EnvManager", $"Save attempt {retryCount} failed: {ex.Message}");
                    }
                }
                
                if (!success)
                {
                    _logger.Error("EnvManager", "Failed to save environment variables after 3 attempts");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EnvManager", $"Error saving environment variables: {ex.Message}");
            }
        }

        public bool SetVariable(string varName, string varValue)
        {
            if (string.IsNullOrWhiteSpace(varName))
            {
                _logger.Warning("EnvManager", "Variable name cannot be empty");
                Console.WriteLine("Error: Variable name cannot be empty");
                return false;
            }
            
            environmentVariables[varName] = varValue;
            SaveEnvironmentVariables();
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
                _logger.Info("EnvManager", "No environment variables set");
            }
            else
            {
                _logger.Info("EnvManager", $"Listing {environmentVariables.Count} environment variables");
                foreach (var kvp in environmentVariables)
                {
                    _logger.Info("EnvManager", $"  {kvp.Key}={kvp.Value}");
                }
            }
        }

        public bool DeleteVariable(string varName)
        {
            if (environmentVariables.ContainsKey(varName))
            {
                environmentVariables.Remove(varName);
                SaveEnvironmentVariables();
                _logger.Info("EnvManager", $"Environment variable '{varName}' deleted");
                return true;
            }
            else
            {
                _logger.Warning("EnvManager", $"Environment variable '{varName}' not found");
                return false;
            }
        }
    }
}
