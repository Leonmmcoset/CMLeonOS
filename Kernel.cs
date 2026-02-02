using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sys = Cosmos.System;
using Cosmos.System.Network.IPv4.UDP.DHCP;

namespace CMLeonOS
{
    public class Kernel : Sys.Kernel
    {
        public void ShowError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{error}");
            Console.ResetColor();
        }

        public void ShowSuccess(string success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{success}");
            Console.ResetColor();
        }

        // 创建全局CosmosVFS实例
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();

        private Shell shell;
        private UserSystem userSystem;
        
        // 修复模式变量（硬编码，用于控制是否启用修复模式）
        public static bool FixMode = false;
        
        // 系统启动时间（用于uptime命令）
        public static DateTime SystemStartTime;
        
        protected override void BeforeRun()
        {
            // Console.Clear();
            Console.WriteLine("Kernel load done!");
            Console.WriteLine(@"-------------------------------------------------");
            Console.WriteLine(@"   ____ __  __ _                      ___  ____  ");
            Console.WriteLine(@"  / ___|  \/  | |    ___  ___  _ __  / _ \/ ___| ");
            Console.WriteLine(@" | |   | |\/| | |   / _ \/ _ \| '_ \| | | \___ \ ");
            Console.WriteLine(@" | |___| |  | | |__|  __/ (_) | | | | |_| |___) |");
            Console.WriteLine(@"  \____|_|  |_|_____\___|\___/|_| |_|____/|____/ ");
            Console.WriteLine();
            Console.WriteLine("CMLeonOS Project");
            Console.WriteLine("By LeonOS 2 Developement Team");
            
            // 记录系统启动时间（用于uptime命令）
            SystemStartTime = DateTime.Now;
            Console.WriteLine($"System started at: {SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            Console.WriteLine("Starting network...");
            try
            {
                if (Cosmos.HAL.NetworkDevice.Devices.Count == 0)
                {
                    throw new Exception("No network devices are available.");
                }
                using var dhcp = new DHCPClient();
                dhcp.SendDiscoverPacket();
                ShowSuccess("Network started.");
            }
            catch (Exception ex)
            {
                ShowError($"Could not start network: {ex.ToString()}");
            }

            // 注册VFS
            try
            {
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                ShowSuccess("VFS initialized successfully");
                
                // 显示可用空间（动态单位）
                var available_space = fs.GetAvailableFreeSpace(@"0:\");
                string spaceWithUnit = FormatBytes(available_space);
                Console.WriteLine("Available Free Space: " + spaceWithUnit);
                
                // 显示文件系统类型
                var fs_type = fs.GetFileSystemType(@"0:\");
                Console.WriteLine("File System Type: " + fs_type);
                
                // 检查并创建system文件夹
                string systemFolderPath = @"0:\system";
                if (!System.IO.Directory.Exists(systemFolderPath))
                {
                    System.IO.Directory.CreateDirectory(systemFolderPath);
                    Console.WriteLine("Created system folder.");
                }
                
                // 初始化用户系统
                userSystem = new UserSystem();
                
                // 检查env.dat文件是否存在，如果不存在则创建并设置Test环境变量
                string envFilePath = @"0:\system\env.dat";
                if (!System.IO.File.Exists(envFilePath))
                {
                    System.IO.File.WriteAllText(envFilePath, "Test=123");
                    ShowSuccess("Created env.dat with Test=123");
                }
                
                // 循环直到登录成功或退出
                while (true)
                {
                    // 第一次启动，设置管理员账户
                    if (!userSystem.HasUsers)
                    {
                        userSystem.FirstTimeSetup();
                    }
                    // 后续启动，需要登录
                    else
                    {
                        // 循环直到登录成功
                        while (!userSystem.Login())
                        {
                            // 登录失败，继续尝试
                        }
                        
                        // 登录成功后，初始化Shell
                        shell = new Shell(userSystem);
                        
                        // 检查并执行启动脚本
                        ExecuteStartupScript();
                        
                        // 运行Shell（用户可以输入exit退出）
                        shell.Run();
                        
                        // 如果用户输入了exit，Shell.Run()会返回，继续循环
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing system: {ex.Message}");
            }
        }

        private void ExecuteStartupScript()
        {
            string startupFilePath = @"0:\system\startup.cm";
            
            try
            {
                // 检查启动脚本文件是否存在
                if (System.IO.File.Exists(startupFilePath))
                {
                    // 读取启动脚本内容
                    string[] lines = System.IO.File.ReadAllLines(startupFilePath);
                    
                    // 检查文件是否为空
                    if (lines.Length == 0 || (lines.Length == 1 && string.IsNullOrWhiteSpace(lines[0])))
                    {
                        Console.WriteLine("Startup script is empty, skipping...");
                        return;
                    }
                    
                    Console.WriteLine("Executing startup script...");
                    Console.WriteLine("--------------------------------");
                    
                    // 逐行执行命令
                    foreach (string line in lines)
                    {
                        // 跳过空行和注释行
                        if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                        {
                            continue;
                        }
                        
                        // 执行命令
                        // Console.WriteLine($"Executing: {line}");
                        shell.ExecuteCommand(line);
                    }
                    
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Startup script execution completed.");
                }
                else
                {
                    // 启动脚本不存在，创建空文件
                    Console.WriteLine("Startup script not found, creating empty file...");
                    System.IO.File.WriteAllText(startupFilePath, "");
                    Console.WriteLine("Created empty startup script at: " + startupFilePath);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error executing startup script: {ex.Message}");
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            int unitIndex = 0;
            double size = bytes;
            
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }
            
            return $"{size:F2} {units[unitIndex]}";
        }

        protected override void Run()
        {
            if (shell != null)
            {
                shell.Run();
            }
        }
    }
}
