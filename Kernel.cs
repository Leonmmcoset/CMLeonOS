using Cosmos.HAL;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.FAT;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.HAL.BlockDevice;
using Cosmos.System.Network.IPv4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Sys = Cosmos.System;
using CMLeonOS.Logger;

namespace CMLeonOS
{
    public class Kernel : Sys.Kernel
    {
        private static CMLeonOS.Logger.Logger _logger = CMLeonOS.Logger.Logger.Instance;

        // 创建全局CosmosVFS实例
        public static Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
        public static Shell shell;
        public static UserSystem userSystem;
        
        public static bool FixMode = false;
        public static DateTime SystemStartTime;
        
        public static Cosmos.HAL.NetworkDevice NetworkDevice = null;
        public static string IPAddress = "Unknown";
        
        protected override void BeforeRun()
        {
            Console.WriteLine("Kernel load done!");
            Console.WriteLine(@"-------------------------------------------------");
            Console.WriteLine(@"   ____ __  __ _                      ___  ____  ");
            Console.WriteLine(@"  / ___|  \/  | |    ___  ___  _ __  / _ \/ ___| ");
            Console.WriteLine(@" | |   | |\/| | |   / _ \/ _ \| '_ \| | | \___ \ ");
            Console.WriteLine(@" | |___| |  | | |__|  __/ (_) | | | | |_| |___) |");
            Console.WriteLine(@"  \____|_|  |_|_____\___|\___/|_| |_|____/|____/ ");
            Console.WriteLine();
            Console.WriteLine("The CMLeonOS Project");
            Console.WriteLine("By LeonOS 2 Developement Team");
            Console.WriteLine(@"-------------------------------------------------");

            // 记录系统启动时间（用于uptime命令）
            SystemStartTime = DateTime.Now;
            _logger.Info("Kernel", $"System started at: {SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            // 初始化网络
            _logger.Info("Kernel", "Starting network initialization");
            try
            {
                if (Cosmos.HAL.NetworkDevice.Devices.Count == 0)
                {
                    throw new Exception("No network devices are available.");
                }
                NetworkDevice = NetworkDevice.Devices[0];
                _logger.Info("Kernel", $"Network device found: {NetworkDevice.Name}");
                
                using var dhcp = new DHCPClient();
                if (NetworkDevice.Ready == true) {
                    _logger.Success("Kernel", "Network device ready.");
                }
                else
                {
                    _logger.Error("Kernel", "Network device is not ready");
                }
                dhcp.SendDiscoverPacket();
                
                IPAddress = NetworkConfiguration.CurrentAddress.ToString();
                _logger.Info("Kernel", $"Local IP: {IPAddress}");
                
                string gateway = NetworkConfigManager.Instance.GetGateway();
                _logger.Info("Kernel", $"Gateway: {gateway}");
                
                string dns = NetworkConfigManager.Instance.GetDNS();
                _logger.Info("Kernel", $"DNS Server: {dns}");
                
                _logger.Success("Kernel", "Network started successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Kernel", $"Network initialization failed: {ex.Message}");
                // ShowError($"Could not start network: {ex.ToString()}");
            }

            // 注册VFS
            _logger.Info("Kernel", "Starting VFS initialization");
            try
            {
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                _logger.Success("Kernel", "VFS initialized successfully");
                
                // 显示可用空间（动态单位）
                var available_space = fs.GetAvailableFreeSpace(@"0:\");
                string spaceWithUnit = FormatBytes(available_space);
                _logger.Info("Kernel", $"Available Free Space: {spaceWithUnit}");
                
                // 显示文件系统类型
                var fs_type = fs.GetFileSystemType(@"0:\");
                _logger.Info("Kernel", $"File System Type: {fs_type}");
                
                // 检查并创建system文件夹
                string systemFolderPath = @"0:\system";
                if (!System.IO.Directory.Exists(systemFolderPath))
                {
                    System.IO.Directory.CreateDirectory(systemFolderPath);
                    _logger.Info("Kernel", "Created system folder at 0:\\system");
                }
                
                // 初始化用户系统
                _logger.Info("Kernel", "Initializing user system");
                userSystem = new UserSystem();
                _logger.Success("Kernel", "User system initialized");
                
                // 检查env.dat文件是否存在，如果不存在则创建并设置Test环境变量
                string envFilePath = @"0:\system\env.dat";
                if (!System.IO.File.Exists(envFilePath))
                {
                    System.IO.File.WriteAllText(envFilePath, "Test=123");
                    _logger.Info("Kernel", "Created env.dat with Test=123");
                }

                // 输出系统启动-初始化完成后的时间
                TimeSpan uptime = DateTime.Now - Kernel.SystemStartTime;
                
                // Console.WriteLine("System started: " + Kernel.SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                // Console.WriteLine("Current time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                // Console.WriteLine();
                
                // 格式化运行时间
                int days = uptime.Days;
                int hours = uptime.Hours;
                int minutes = uptime.Minutes;
                int seconds = uptime.Seconds;
                
                // Console.WriteLine($"System uptime: {days} days, {hours} hours, {minutes} minutes, {seconds} seconds");
                _logger.Info("Kernel", $"System initialization completed in {days} days, {hours} hours, {minutes} minutes, {seconds} seconds");
                // Console.WriteLine($"Total uptime: {uptime.TotalHours:F2} hours");
                
                // 循环直到登录成功或退出
                while (true)
                {
                    // 第一次启动，设置管理员账户
                    if (!userSystem.HasUsers)
                    {
                        _logger.Info("Kernel", "First time setup - creating admin account");
                        userSystem.FirstTimeSetup();
                        _logger.Success("Kernel", "Admin account created successfully");
                    }
                    // 后续启动，需要登录
                    else
                    {
                        // 循环直到登录成功
                        while (!userSystem.Login())
                        {
                            // 登录失败，继续尝试
                        }
                        
                        _logger.Info("Kernel", $"User '{userSystem.CurrentUsername}' logged in successfully");
                        
                        // 登录成功后，初始化Shell
                        shell = new Shell(userSystem);
                        
                        // 检查并执行启动脚本
                        ExecuteStartupScript();

                        if (System.IO.File.Exists("0:\\system\\zen"))
                        {
                            Console.WriteLine("=====================================");
                            Console.WriteLine("        The Zen of CMLeonOS         ");
                            Console.WriteLine("(For the dreamer at 0x100000)");
                            Console.WriteLine("=====================================");
                            Console.WriteLine();
                            Console.WriteLine("Memory has bounds, but thought breaks all frame,");
                            Console.WriteLine("Bare metal no layers, code bears its name.");
                            Console.WriteLine("A boot's brief spark, all systems ignite,");
                            Console.WriteLine("Errors in registers, roots in code's flight.");
                            Console.WriteLine();
                            Console.WriteLine("Simplicity beats the redundant's vain race,");
                            Console.WriteLine("Stability outshines the radical's chase.");
                            Console.WriteLine("Hardware ne'er lies, code holds the wise key,");
                            Console.WriteLine("Interrupts not chaos, scheduling sets free.");
                            Console.WriteLine();
                            Console.WriteLine("Binary's cold shell, no breath, no soul,");
                            Console.WriteLine("Kernel's warm core, makes the machine whole.");
                            Console.WriteLine("From zero to one, the boot path we tread,");
                            Console.WriteLine("From one to forever, guard every thread.");
                            Console.WriteLine();
                            Console.WriteLine("Build the kernel in zen, step by step, line by line,");
                            Console.WriteLine("A bug brings new wake, a line brings new shine.");
                        }
                        
                        // 运行Shell（用户可以输入exit退出）
                        _logger.Info("Kernel", "Starting Shell");
                        shell.Run();
                        _logger.Info("Kernel", "Shell exited");
                        
                        // 如果用户输入了exit，Shell.Run()会返回，继续循环
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Kernel", $"System initialization error: {ex.Message}");
                _logger.Error("Kernel", $"Please contact the developers, Email: leonmmcoset@outlook.com");
                // 这条横线居然和上一条信息的字符数是一样的
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine($"Maybe your disk isn't formatted, please format your disk using like PE or something else.");
                Console.WriteLine("(CMLeonOS need more than 512 MB disk space and FAT32 file system.)");
                Console.WriteLine("Press any key to restart...");
                Console.ReadKey();
                Sys.Power.Reboot();
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
                    _logger.Info("Kernel", "Startup script found, executing...");
                    
                    // 读取启动脚本内容
                    string[] lines = System.IO.File.ReadAllLines(startupFilePath);
                    
                    // 检查文件是否为空
                    if (lines.Length == 0 || (lines.Length == 1 && string.IsNullOrWhiteSpace(lines[0])))
                    {
                        _logger.Warning("Kernel", "Startup script is empty, skipping");
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
                    _logger.Success("Kernel", "Startup script execution completed");
                }
                else
                {
                    // 启动脚本不存在，创建空文件
                    _logger.Info("Kernel", "Startup script not found, creating empty file...");
                    System.IO.File.WriteAllText(startupFilePath, "");
                    _logger.Info("Kernel", "Created empty startup script at 0:\\system\\startup.cm");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Kernel", $"Error executing startup script: {ex.Message}");
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
