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

namespace CMLeonOS.Settings
{
    public static class SettingsManager
    {
        private static string settingsFilePath = @"0:\system\settings.dat";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        private static bool savePending = false;
        private static int pendingSaveCount = 0;
        private const int MAX_PENDING_SAVES = 5;
        
        private static Dictionary<string, string> defaultSettings = new Dictionary<string, string>
        {
            { "LoggerEnabled", "true" },
            { "MaxLoginAttempts", "3" },
            { "GUI_LeftHandStartButton", "false" },
            { "GUI_ShowFps", "true" },
            { "GUI_TwelveHourClock", "false" },
            { "GUI_MouseSensitivity", "1.0" },
            { "GUI_ScreenWidth", "1280" },
            { "GUI_ScreenHeight", "800" },
            { "GUI_DarkNotepad", "false" }
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

        public static int MaxLoginAttempts
        {
            get
            {
                if (settings.TryGetValue("MaxLoginAttempts", out string value))
                {
                    if (int.TryParse(value, out int result))
                    {
                        return result;
                    }
                }
                return 3;
            }
            set
            {
                settings["MaxLoginAttempts"] = value.ToString();
                SaveSettings();
            }
        }

        public static bool GUI_LeftHandStartButton
        {
            get
            {
                if (settings.TryGetValue("GUI_LeftHandStartButton", out string value))
                {
                    return value.ToLower() == "true";
                }
                return false;
            }
            set
            {
                settings["GUI_LeftHandStartButton"] = value ? "true" : "false";
                SaveSettings();
            }
        }

        public static bool GUI_ShowFps
        {
            get
            {
                if (settings.TryGetValue("GUI_ShowFps", out string value))
                {
                    return value.ToLower() == "true";
                }
                return true;
            }
            set
            {
                settings["GUI_ShowFps"] = value ? "true" : "false";
                SaveSettings();
            }
        }

        public static bool GUI_TwelveHourClock
        {
            get
            {
                if (settings.TryGetValue("GUI_TwelveHourClock", out string value))
                {
                    return value.ToLower() == "true";
                }
                return false;
            }
            set
            {
                settings["GUI_TwelveHourClock"] = value ? "true" : "false";
                SaveSettings();
            }
        }

        public static float GUI_MouseSensitivity
        {
            get
            {
                if (settings.TryGetValue("GUI_MouseSensitivity", out string value))
                {
                    if (float.TryParse(value, out float result))
                    {
                        return result;
                    }
                }
                return 1.0f;
            }
            set
            {
                settings["GUI_MouseSensitivity"] = value.ToString();
                SaveSettings();
            }
        }

        public static int GUI_ScreenWidth
        {
            get
            {
                if (settings.TryGetValue("GUI_ScreenWidth", out string value))
                {
                    if (int.TryParse(value, out int result))
                    {
                        return result;
                    }
                }
                return 1280;
            }
            set
            {
                settings["GUI_ScreenWidth"] = value.ToString();
                SaveSettings();
            }
        }

        public static int GUI_ScreenHeight
        {
            get
            {
                if (settings.TryGetValue("GUI_ScreenHeight", out string value))
                {
                    if (int.TryParse(value, out int result))
                    {
                        return result;
                    }
                }
                return 800;
            }
            set
            {
                settings["GUI_ScreenHeight"] = value.ToString();
                SaveSettings();
            }
        }

        public static bool GUI_DarkNotepad
        {
            get
            {
                if (settings.TryGetValue("GUI_DarkNotepad", out string value))
                {
                    return value.ToLower() == "true";
                }
                return false;
            }
            set
            {
                settings["GUI_DarkNotepad"] = value ? "true" : "false";
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
                
                FlushSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading settings: {e.Message}");
            }
        }

        public static void SaveSettings()
        {
            savePending = true;
            pendingSaveCount++;
            
            if (pendingSaveCount >= MAX_PENDING_SAVES)
            {
                FlushSettings();
            }
        }
        
        public static void FlushSettings()
        {
            if (!savePending)
            {
                return;
            }
            
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
                
                savePending = false;
                pendingSaveCount = 0;
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
