using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Utils
{
    public class IniBuilder
    {
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
        private string currentSection = "";

        public IniBuilder()
        {
        }

        public IniBuilder BeginSection(string section)
        {
            currentSection = section;
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, string>();
            }
            return this;
        }

        public IniBuilder SetSection(string section)
        {
            currentSection = section;
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, string>();
            }
            return this;
        }

        public IniBuilder SetValue(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(currentSection))
            {
                currentSection = "General";
                if (!data.ContainsKey(currentSection))
                {
                    data[currentSection] = new Dictionary<string, string>();
                }
            }

            data[currentSection][key] = value;
            return this;
        }

        public IniBuilder SetValue(string section, string key, string value)
        {
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, string>();
            }

            data[section][key] = value;
            return this;
        }

        public IniBuilder AddKey(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(currentSection))
            {
                currentSection = "General";
                if (!data.ContainsKey(currentSection))
                {
                    data[currentSection] = new Dictionary<string, string>();
                }
            }

            data[currentSection][key] = value.ToString();
            return this;
        }

        public IniBuilder AddKey(string section, string key, object value)
        {
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, string>();
            }

            data[section][key] = value.ToString();
            return this;
        }

        public string Build()
        {
            var lines = new List<string>();

            foreach (var section in data.Keys)
            {
                lines.Add($"[{section}]");

                foreach (var kvp in data[section])
                {
                    lines.Add($"{kvp.Key}={kvp.Value}");
                }

                lines.Add("");
            }

            return string.Join("\n", lines);
        }

        public override string ToString()
        {
            return Build();
        }

        public void Save(string filePath)
        {
            string content = Build();
            File.WriteAllText(filePath, content);
        }
    }
}
