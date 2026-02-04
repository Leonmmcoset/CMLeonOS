using System;
using System.IO;

namespace CMLeonOS
{
    public class NetworkConfigManager
    {
        private static NetworkConfigManager instance;
        private string configFilePath = @"0:\system\network.dat";
        
        private string dns = "1.1.1.1";
        private string gateway = "192.168.0.1";
        
        private NetworkConfigManager()
        {
            LoadConfig();
        }
        
        public static NetworkConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkConfigManager();
                }
                return instance;
            }
        }
        
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);
                    
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        {
                            continue;
                        }
                        
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim().ToLower();
                            string value = parts[1].Trim();
                            
                            if (key == "dns")
                            {
                                dns = value;
                            }
                            else if (key == "gateway")
                            {
                                gateway = value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading network config: {ex.Message}");
            }
        }
        
        private void SaveConfig()
        {
            try
            {
                string directory = Path.GetDirectoryName(configFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                using (StreamWriter writer = new StreamWriter(configFilePath))
                {
                    writer.WriteLine("# CMLeonOS Network Configuration");
                    writer.WriteLine($"# Generated on {DateTime.Now}");
                    writer.WriteLine();
                    writer.WriteLine($"DNS={dns}");
                    writer.WriteLine($"Gateway={gateway}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving network config: {ex.Message}");
            }
        }
        
        public string GetDNS()
        {
            return dns;
        }
        
        public string GetGateway()
        {
            return gateway;
        }
        
        public void SetDNS(string dnsIp)
        {
            if (string.IsNullOrWhiteSpace(dnsIp))
            {
                throw new ArgumentException("DNS address cannot be empty");
            }
            
            dns = dnsIp;
            SaveConfig();
        }
        
        public void SetGateway(string gatewayIp)
        {
            if (string.IsNullOrWhiteSpace(gatewayIp))
            {
                throw new ArgumentException("Gateway address cannot be empty");
            }
            
            gateway = gatewayIp;
            SaveConfig();
        }
    }
}