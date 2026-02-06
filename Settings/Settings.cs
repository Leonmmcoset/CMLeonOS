using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Settings
{
    public static class SettingsManager
    {
        private static string settingsFilePath = @"0:\system\settings.dat";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        
        private static Dictionary<string, string> defaultSettings = new Dictionary<string, string>
        {
            { "LoggerEnabled", "true" }
        };

        public static bool LoggerEnabled
        {
            get
            {
                if (settings.TryGetValue("LoggerEnabled", out string value))
                {
                    return value.ToLower() == "true";
                }
                return true;
            }
            set
            {
                settings["LoggerEnabled"] = value ? "true" : "false";
                SaveSettings();
            }
        }

        public static void LoadSettings()
        {
            settings.Clear();
            
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string[] lines = File.ReadAllLines(settingsFilePath);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                        {
                            int separatorIndex = line.IndexOf('=');
                            if (separatorIndex > 0)
                            {
                                string key = line.Substring(0, separatorIndex).Trim();
                                string value = line.Substring(separatorIndex + 1).Trim();
                                settings[key] = value;
                            }
                        }
                    }
                    
                    foreach (var defaultSetting in defaultSettings)
                    {
                        if (!settings.ContainsKey(defaultSetting.Key))
                        {
                            settings[defaultSetting.Key] = defaultSetting.Value;
                        }
                    }
                }
                else
                {
                    foreach (var defaultSetting in defaultSettings)
                    {
                        settings[defaultSetting.Key] = defaultSetting.Value;
                    }
                    SaveSettings();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading settings: {e.Message}");
            }
        }

        public static void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
                
                using (StreamWriter writer = new StreamWriter(settingsFilePath))
                {
                    writer.WriteLine("# CMLeonOS Settings Configuration");
                    writer.WriteLine("# Format: setting_name=value");
                    writer.WriteLine();
                    
                    foreach (var setting in settings)
                    {
                        writer.WriteLine($"{setting.Key}={setting.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving settings: {e.Message}");
            }
        }

        public static string GetSetting(string key)
        {
            if (settings.TryGetValue(key, out string value))
            {
                return value;
            }
            return null;
        }

        public static void SetSetting(string key, string value)
        {
            settings[key] = value;
            SaveSettings();
        }

        public static bool GetBoolSetting(string key, bool defaultValue)
        {
            if (settings.TryGetValue(key, out string value))
            {
                return value.ToLower() == "true";
            }
            return defaultValue;
        }

        public static int GetIntSetting(string key, int defaultValue)
        {
            if (settings.TryGetValue(key, out string value))
            {
                if (int.TryParse(value, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        public static void ListSettings()
        {
            if (settings.Count == 0)
            {
                Console.WriteLine("No settings defined");
                return;
            }

            Console.WriteLine("Current settings:");
            
            foreach (var setting in settings)
            {
                Console.WriteLine($"  {setting.Key} = {setting.Value}");
            }
        }
    }
}
