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

namespace CMLeonOS.Utils
{
    public class IniReader
    {
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public IniReader(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                string currentSection = "";

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        if (!data.ContainsKey(currentSection))
                        {
                            data[currentSection] = new Dictionary<string, string>();
                        }
                    }
                    else if (trimmedLine.Contains("=") && !string.IsNullOrWhiteSpace(currentSection))
                    {
                        string[] parts = trimmedLine.Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            data[currentSection][key] = value;
                        }
                    }
                }
            }
        }

        public string GetValue(string section, string key, string defaultValue = "")
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                return data[section][key];
            }
            return defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            string value = GetValue(section, key, defaultValue.ToString());
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defaultValue;
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            string value = GetValue(section, key, defaultValue.ToString()).ToLower();
            return value == "true" || value == "1" || value == "yes";
        }

        public List<string> GetSections()
        {
            return new List<string>(data.Keys);
        }

        public List<string> GetKeys(string section)
        {
            if (data.ContainsKey(section))
            {
                return new List<string>(data[section].Keys);
            }
            return new List<string>();
        }

        public bool TryReadBool(string key, out bool value, string section = "")
        {
            string sectionToUse = string.IsNullOrWhiteSpace(section) ? "General" : section;
            string stringValue = GetValue(sectionToUse, key, "false");
            value = stringValue.ToLower() == "true" || stringValue == "1" || stringValue == "yes";
            return true;
        }

        public bool TryReadInt(string key, out int value, string section = "")
        {
            string sectionToUse = string.IsNullOrWhiteSpace(section) ? "General" : section;
            string stringValue = GetValue(sectionToUse, key, "0");
            return int.TryParse(stringValue, out value);
        }

        public bool TryReadFloat(string key, out float value, string section = "")
        {
            string sectionToUse = string.IsNullOrWhiteSpace(section) ? "General" : section;
            string stringValue = GetValue(sectionToUse, key, "0");
            return float.TryParse(stringValue, out value);
        }
    }
}
