using System;
using System.Collections.Generic;
using System.IO;
using CMLeonOS;

namespace CMLeonOS.Commands.Utility
{
    public static class AppManagerCommand
    {
        private static readonly Dictionary<string, byte[]> embeddedApps = new Dictionary<string, byte[]>();
        
        public static void InitializeApps()
        {
            LoadEmbeddedApps();
        }
        
        private static void LoadEmbeddedApps()
        {
            embeddedApps.Clear();
            
            try
            {
                if (LuaApps.helloworld != null && LuaApps.helloworld.Length > 0)
                {
                    embeddedApps["helloworld.lua"] = LuaApps.helloworld;
                }
                
                if (LuaApps.testspeed != null && LuaApps.testspeed.Length > 0)
                {
                    embeddedApps["testspeed.lua"] = LuaApps.testspeed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading embedded apps: {ex.Message}");
            }
        }
        
        public static void ListApps()
        {
            Console.WriteLine("Available Applications:");
            Console.WriteLine("======================");
            
            if (embeddedApps.Count == 0)
            {
                Console.WriteLine("No applications available.");
                return;
            }
            
            foreach (var app in embeddedApps)
            {
                Console.WriteLine($"  - {app.Key}");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Total: {embeddedApps.Count} application(s)");
        }
        
        public static void InstallApp(string appName, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                showError("Error: Application name is required.");
                Console.WriteLine("Usage: app install <appname>");
                Console.WriteLine("Use 'app list' to see available applications.");
                return;
            }
            
            if (!embeddedApps.ContainsKey(appName))
            {
                showError($"Error: Application '{appName}' not found.");
                Console.WriteLine("Use 'app list' to see available applications.");
                return;
            }
            
            try
            {
                string appsDir = "0:\\apps";
                
                if (!Directory.Exists(appsDir))
                {
                    Directory.CreateDirectory(appsDir);
                    Console.WriteLine($"Created directory: {appsDir}");
                }
                
                string filePath = Path.Combine(appsDir, appName);
                
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"Warning: File already exists: {filePath}");
                    Console.Write("Overwrite? (y/n): ");
                    string response = Console.ReadLine()?.ToLower();
                    
                    if (response != "y" && response != "yes")
                    {
                        Console.WriteLine("Installation cancelled.");
                        return;
                    }
                }
                
                File.WriteAllBytes(filePath, embeddedApps[appName]);
                Console.WriteLine($"Successfully installed: {appName}");
                Console.WriteLine($"Location: {filePath}");
            }
            catch (Exception ex)
            {
                showError($"Error installing application: {ex.Message}");
            }
        }
        
        public static void UninstallApp(string appName, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                showError("Error: Application name is required.");
                Console.WriteLine("Usage: app uninstall <appname>");
                return;
            }
            
            string appsDir = "0:\\apps";
            string filePath = Path.Combine(appsDir, appName);
            
            if (!File.Exists(filePath))
            {
                showError($"Error: Application '{appName}' not installed.");
                return;
            }
            
            try
            {
                File.Delete(filePath);
                Console.WriteLine($"Successfully uninstalled: {appName}");
            }
            catch (Exception ex)
            {
                showError($"Error uninstalling application: {ex.Message}");
            }
        }
        
        public static void ShowInstalledApps(CMLeonOS.FileSystem fileSystem)
        {
            string appsDir = "0:\\apps";
            
            if (!Directory.Exists(appsDir))
            {
                Console.WriteLine("No applications installed.");
                return;
            }
            
            Console.WriteLine("Installed Applications:");
            Console.WriteLine("=======================");
            
            var files = Directory.GetFiles(appsDir);
            
            if (files.Length == 0)
            {
                Console.WriteLine("No applications installed.");
                return;
            }
            
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                Console.WriteLine($"  - {fileName}");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Total: {files.Length} application(s)");
        }
        
        public static void ShowHelp()
        {
            Console.WriteLine("Application Manager");
            Console.WriteLine("===================");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  app list          - List available applications");
            Console.WriteLine("  app install <name> - Install an application");
            Console.WriteLine("  app uninstall <name> - Uninstall an application");
            Console.WriteLine("  app installed     - List installed applications");
            Console.WriteLine("  app help          - Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  app list");
            Console.WriteLine("  app install helloworld.lua");
            Console.WriteLine("  app uninstall helloworld.lua");
            Console.WriteLine("  app installed");
        }
    }
}