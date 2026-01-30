using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sys = Cosmos.System;

namespace CMLeonOS
{
    public class Kernel : Sys.Kernel
    {
        // 创建全局CosmosVFS实例
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();

        private Shell shell;
        private UserSystem userSystem;
        
        // 修复模式变量（硬编码，用于控制是否启用修复模式）
        public static bool FixMode = false;

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine(@"   ____ __  __ _                      ___  ____  ");
            Console.WriteLine(@"  / ___|  \/  | |    ___  ___  _ __  / _ \/ ___| ");
            Console.WriteLine(@" | |   | |\/| | |   / _ \/ _ \| '_ \| | | \___ \ ");
            Console.WriteLine(@" | |___| |  | | |__|  __/ (_) | | | | |_| |___) |");
            Console.WriteLine(@"  \____|_|  |_|_____\___|\___/|_| |_|____/|____/ ");
            Console.WriteLine();
            Console.WriteLine("CMLeonOS Test Project");
            Console.WriteLine("By LeonOS 2 Developement Team");
            
            // 注册VFS
            try
            {
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                Console.WriteLine("VFS initialized successfully");
                
                // 显示可用空间
                var available_space = fs.GetAvailableFreeSpace(@"0:\");
                Console.WriteLine("Available Free Space: " + available_space + " bytes");
                
                // 显示文件系统类型
                var fs_type = fs.GetFileSystemType(@"0:\");
                Console.WriteLine("File System Type: " + fs_type);
                
                // 删除默认示例文件和文件夹
                try
                {
                    // 删除示例文件
                    if (System.IO.File.Exists(@"0:\example.txt"))
                    {
                        System.IO.File.Delete(@"0:\example.txt");
                        Console.WriteLine("Deleted example.txt");
                    }
                    
                    // 删除示例文件夹
                    if (System.IO.Directory.Exists(@"0:\example"))
                    {
                        System.IO.Directory.Delete(@"0:\example", true);
                        Console.WriteLine("Deleted example directory");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting example files: {ex.Message}");
                }
                
                // 初始化用户系统
                userSystem = new UserSystem();
                
                // 第一次启动，设置管理员密码
                if (!userSystem.IsPasswordSet)
                {
                    userSystem.SetAdminPassword();
                }
                // 后续启动，需要登录
                else
                {
                    // 循环直到登录成功
                    while (!userSystem.Login())
                    {
                        // 登录失败，继续尝试
                    }
                }
                
                // 登录成功后，初始化Shell
                shell = new Shell();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing system: {ex.Message}");
            }
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
