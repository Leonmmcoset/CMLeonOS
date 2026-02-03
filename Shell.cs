using CosmosFtpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sys = Cosmos.System;
using Cosmos.System.Network.IPv4;
using EndPoint = Cosmos.System.Network.IPv4.EndPoint;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CosmosHttp.Client;
using Cosmos.Core;
using Cosmos.Core.Memory;
using UniLua;
using Cosmos.HAL;

namespace CMLeonOS
{
    public class Shell
    {
        private string prompt = "/";
        private List<string> commandHistory = new List<string>();
        private FileSystem fileSystem;
        private UserSystem userSystem;
        private bool fixMode;
        private EnvironmentVariableManager envManager;

        public void ShowError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{error}");
            Console.ResetColor();
        }

        public void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message}");
            Console.ResetColor();
        }

        public void ShowWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{warning}");
            Console.ResetColor();
        }

        public Shell(UserSystem userSystem)
        {
            this.userSystem = userSystem;
            fileSystem = new FileSystem();
            fixMode = Kernel.FixMode;
            envManager = EnvironmentVariableManager.Instance;
        }

        public void Run()
        {
            bool shouldExit = false;
            while (true)
            {
                // 显示当前文件夹路径作为提示符（彩色）
                string currentPath = fileSystem.CurrentDirectory;
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{currentPath} | /");
                Console.ForegroundColor = originalColor;
                var input = Console.ReadLine();
                
                // 检查是否为退出命令
                if (input != null && input.ToLower().Trim() == "exit")
                {
                    Console.WriteLine("Exiting system...");
                    shouldExit = true;
                    break;
                }
                
                commandHistory.Add(input);
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    var command = parts[0].ToLower();
                    var args = parts.Length > 1 ? string.Join(" ", parts, 1, parts.Length - 1) : "";
                    ProcessCommand(command, args);
                }
                
                // 如果需要退出，返回到登录页面
                if (shouldExit)
                {
                    return;
                }
            }
        }

        public void ExecuteCommand(string commandLine)
        {
            var parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                var command = parts[0].ToLower();
                var args = parts.Length > 1 ? string.Join(" ", parts, 1, parts.Length - 1) : "";
                ProcessCommand(command, args);
            }
        }

        private void ProcessCommand(string command, string args)
        {
            switch (command)
            {
                case "echo":
                    ProcessEcho(args);
                    break;
                case "clear":
                case "cls":
                    Console.Clear();
                    break;
                case "restart":
                    Console.WriteLine("Restarting system...");
                    Sys.Power.Reboot();
                    break;
                case "shutdown":
                    Console.WriteLine("Shutting down system...");
                    Sys.Power.Shutdown();
                    break;
                case "help":
                    // 分页显示帮助信息
                    string[] helpArgs = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int pageNumber = 1;
                    bool showAll = false;
                    
                    if (helpArgs.Length > 0)
                    {
                        if (helpArgs[0].ToLower() == "all")
                        {
                            showAll = true;
                        }
                        else if (int.TryParse(helpArgs[0], out int page))
                        {
                            pageNumber = page;
                        }
                    }
                    
                    string[] allCommands = {
                        "  echo <text>      - Display text (supports \\n for newline)",
                        "  clear/cls        - Clear the screen",
                        "  restart          - Restart the system",
                        "  shutdown         - Shutdown the system",
                        "  time             - Display current time",
                        "  date             - Display current date",
                        "  prompt <text>    - Change command prompt",
                        "  calc <expr>      - Simple calculator",
                        "  history          - Show command history",
                        "  background <hex> - Change background color",
                        "  cuitest          - Test CUI framework",
                        "  edit <file>      - Simple code editor",
                        "                     Tab key inserts 4 spaces",
                        "  ls <dir>         - List files and directories",
                        "  cd <dir>         - Change directory",
                        "                     cd ..              - Go to parent directory",
                        "                     cd dir1/dir2/dir3  - Go to numbered directory",
                        "  pwd              - Show current directory",
                        "  mkdir <dir>      - Create directory",
                        "  rm <file>        - Remove file",
                        "                     Use -norisk to delete files in sys folder",
                        "  rmdir <dir>      - Remove directory",
                        "  cat <file>       - Display file content",
                        "  echo <text> > <file> - Write text to file",
                        "  head <file>      - Display first lines of file",
                        "                     Usage: head <file> <lines>",
                        "  tail <file>      - Display last lines of file",
                        "                     Usage: tail <file> <lines>",
                        "  wc <file>        - Count lines, words, characters",
                        "  cp <src> <dst>   - Copy file",
                        "  mv <src> <dst>   - Move/rename file",
                        "  touch <file>     - Create empty file",
                        "  find <name>      - Find file",
                        "  getdisk          - Show disk information",
                        "  user <cmd>       - User management",
                        "                     user add admin <username> <password>    - Add admin user",
                        "                     user add user <username> <password>     - Add regular user",
                        "                     user delete <username>                  - Delete user",
                        "                     user list                               - List all users",
                        "  cpass            - Change password",
                        "  env <cmd>       - Environment variables",
                        "                     env see <varname>                    - Show variable value",
                        "                     env change <varname> <value>           - Set variable value",
                        "                     env delete <varname>                  - Delete variable",
                        "  beep             - Play beep sound",
                        "  uptime           - Show system uptime",
                        "  branswe <filename> - Execute Branswe code file",
                        "  backup <name>    - Backup system files",
                        "  restore <name>  - Restore system files",
                        "  grep <pattern> <file> - Search text in file",
                        "  ping <ip>        - Ping IP address (5 times)",
                        "  tcpserver <port> - Start TCP server on specified port",
                        "  tcpclient <ip> <port> - Connect to TCP server",
                        "  wget <url>       - Download file from URL",
                        "  whoami          - Show current username",
                        "  base64 encrypt <text> - Encode text to Base64",
                        "  base64 decrypt <text> - Decode Base64 to text",
                        "  lua <file>       - Execute Lua script",
                        "  version          - Show OS version",
                        "  about            - Show about information",
                        "  help <page>      - Show help page (1-3)",
                        "  help all          - Show all help pages",
                        "",
                        "Startup Script: sys\\startup.cm",
                        "  Commands in this file will be executed on startup",
                        "  Each line should contain one command",
                        "  Lines starting with # are treated as comments"
                    };
                    
                    int commandsPerPage = 15;
                    int totalPages = (int)Math.Ceiling((double)allCommands.Length / commandsPerPage);
                    
                    if (showAll)
                    {
                        Console.WriteLine("====================================");
                        Console.WriteLine("        Help - All Pages");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        
                        foreach (var cmd in allCommands)
                        {
                            Console.WriteLine(cmd);
                        }
                        
                        Console.WriteLine();
                        Console.WriteLine("-- End of help --");
                    }
                    else
                    {
                        if (pageNumber > totalPages)
                        {
                            pageNumber = totalPages;
                        }
                        if (pageNumber < 1)
                        {
                            pageNumber = 1;
                        }
                        
                        int startIndex = (pageNumber - 1) * commandsPerPage;
                        int endIndex = Math.Min(startIndex + commandsPerPage, allCommands.Length);
                        
                        Console.WriteLine("====================================");
                        Console.WriteLine($"        Help - Page {pageNumber}/{totalPages}");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        
                        for (int i = startIndex; i < endIndex; i++)
                        {
                            Console.WriteLine(allCommands[i]);
                        }
                        
                        if (pageNumber < totalPages)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"-- More -- Type 'help {pageNumber + 1}' for next page or 'help all' for all pages --");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("-- End of help --");
                        }
                    }
                    
                    // Console.WriteLine("Startup Script: sys\\startup.cm");
                    // Console.WriteLine("  Commands in this file will be executed on startup");
                    // Console.WriteLine("  Each line should contain one command");
                    // Console.WriteLine("  Lines starting with # are treated as comments");
                    break;
                case "time":
                    Console.WriteLine(DateTime.Now.ToString());
                    break;
                case "date":
                    Console.WriteLine(DateTime.Now.ToShortDateString());
                    break;
                case "prompt":
                    ChangePrompt(args);
                    break;
                case "calc":
                    Calculate(args);
                    break;
                case "history":
                    ShowHistory();
                    break;
                case "background":
                    ChangeBackground(args);
                    break;
                case "cuitest":
                    TestCUI();
                    break;
                case "edit":
                    EditFile(args);
                    break;
                case "nano":
                    NanoFile(args);
                    break;
                case "diff":
                    DiffFiles(args);
                    break;
                case "cal":
                    ShowCalendar(args);
                    break;
                case "sleep":
                    SleepCommand(args);
                    break;
                case "com":
                    ExecuteCommandFile(args);
                    break;
                case "ls":
                    fileSystem.ListFiles(args);
                    break;
                case "cd":
                    fileSystem.ChangeDirectory(args);
                    break;
                case "pwd":
                    Console.WriteLine(fileSystem.CurrentDirectory);
                    break;
                case "mkdir":
                    if (string.IsNullOrEmpty(args))
                    {
                        ShowError("Please specify a directory name");
                    }
                    else
                    {
                        fileSystem.MakeDirectory(args);
                    }
                    break;
                case "rm":
                    if (string.IsNullOrEmpty(args))
                    {
                        ShowError("Please specify a file name");
                    }
                    else
                    {
                        // 检查是否在sys文件夹中（修复模式下绕过检测）
                        bool isInSysFolder = (args.Contains(@"\system\") || args.Contains(@"/sys/")) && !fixMode;
                        
                        if (isInSysFolder)
                        {
                            // 检查是否有-norisk参数
                            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            bool hasNorisk = false;
                            string filePath = args;
                            
                            if (parts.Length > 1)
                            {
                                hasNorisk = Array.IndexOf(parts, "-norisk") >= 0;
                                filePath = parts[0];
                            }
                            
                            if (!hasNorisk)
                            {
                                ShowError("Cannot delete files in sys folder without -norisk parameter");
                                ShowError("Usage: rm <file> -norisk");
                            }
                            else
                            {
                                fileSystem.DeleteFile(filePath);
                            }
                        }
                        else
                        {
                            fileSystem.DeleteFile(args);
                        }
                    }
                    break;
                case "rmdir":
                    if (string.IsNullOrEmpty(args))
                    {
                        ShowError("Please specify a directory name");
                    }
                    else
                    {
                        // 检查是否在sys文件夹中（修复模式下绕过检测）
                        bool isInSysFolder = (args.Contains(@"\system\") || args.Contains(@"/sys/")) && !fixMode;
                        
                        if (isInSysFolder)
                        {
                            ShowError("Cannot delete directories in sys folder");
                            ShowError("Use fix mode to bypass this restriction");
                        }
                        else
                        {
                            fileSystem.DeleteDirectory(args);
                        }
                    }
                    break;
                case "cat":
                    if (string.IsNullOrEmpty(args))
                    {
                        ShowError("Please specify a file name");
                    }
                    else
                    {
                        Console.WriteLine(fileSystem.ReadFile(args));
                    }
                    break;
                case "version":
                    Console.WriteLine(Version.DisplayVersion);
                    Console.WriteLine($"Major: {Version.Major}");
                    Console.WriteLine($"Minor: {Version.Minor}");
                    Console.WriteLine($"Patch: {Version.Patch}");
                    Console.WriteLine($"Full Version: {Version.FullVersion}");
                    break;
                case "about":
                    Console.WriteLine("CMLeonOS Project");
                    Console.WriteLine("By LeonOS 2 Developement Team");
                    break;
                case "head":
                    HeadFile(args);
                    break;
                case "tail":
                    TailFile(args);
                    break;
                case "wc":
                    WordCount(args);
                    break;
                case "cp":
                    CopyFile(args);
                    break;
                case "mv":
                    MoveFile(args);
                    break;
                case "rename":
                    RenameFile(args);
                    break;
                case "touch":
                    CreateEmptyFile(args);
                    break;
                case "find":
                    FindFile(args);
                    break;
                case "getdisk":
                    GetDiskInfo();
                    break;
                case "user":
                    ProcessUserCommand(args);
                    break;
                case "cpass":
                    userSystem.ChangePassword();
                    break;
                case "beep":
                    Console.Beep();
                    break;
                case "env":
                    ProcessEnvCommand(args);
                    break;
                case "branswe":
                    ProcessBransweCommand(args);
                    break;
                case "uptime":
                    ShowUptime();
                    break;
                case "grep":
                    GrepFile(args);
                    break;
                case "backup":
                    BackupSystem(args);
                    break;
                case "restore":
                    RestoreSystem(args);
                    break;
                case "ftp":
                    CreateFTP();
                    break;
                case "ping":
                    PingIP(args);
                    break;
                case "tcpserver":
                    StartTcpServer(args);
                    break;
                case "tcpclient":
                    ConnectTcpClient(args);
                    break;
                case "wget":
                    DownloadFile(args);
                    break;
                case "whoami":
                    ShowCurrentUsername();
                    break;
                case "base64":
                    ProcessBase64Command(args);
                    break;
                case "lua":
                    ExecuteLuaScript(args);
                    break;
                default:
                    ShowError($"Unknown command: {command}");
                    break;
            }
        }

        private void ProcessEcho(string args)
        {
            // 支持基本的转义字符
            var processedArgs = args.Replace("\\n", "\n");
            Console.WriteLine(processedArgs);
        }

        private void ChangePrompt(string args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                prompt = args;
            }
            else
            {
                prompt = "/";
            }
        }

        private void Calculate(string expression)
        {
            try
            {
                // 简单的计算器，只支持加减乘除
                var parts = expression.Split(' ');
                if (parts.Length == 3)
                {
                    double num1 = double.Parse(parts[0]);
                    string op = parts[1];
                    double num2 = double.Parse(parts[2]);
                    double result = 0;

                    switch (op)
                    {
                        case "+":
                            result = num1 + num2;
                            break;
                        case "-":
                            result = num1 - num2;
                            break;
                        case "*":
                            result = num1 * num2;
                            break;
                        case "/":
                            if (num2 != 0)
                            {
                                result = num1 / num2;
                            }
                            else
                            {
                                ShowError("Division by zero");
                                return;
                            }
                            break;
                        default:
                            ShowError("Invalid operator. Use +, -, *, /");
                            return;
                    }

                    Console.WriteLine($"Result: {result}");
                }
                else
                {
                    ShowError("Invalid expression. Use format: calc <num> <op> <num>");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowHistory()
        {
            for (int i = 0; i < commandHistory.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {commandHistory[i]}");
            }
        }

        private void ChangeBackground(string hexColor)
        {
            try
            {
                // 移除#前缀（如果有）
                hexColor = hexColor.TrimStart('#');

                // 解析16进制颜色代码
                if (hexColor.Length == 6)
                {
                    int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

                    // 简单的颜色映射，将RGB值映射到最接近的ConsoleColor
                    ConsoleColor color = GetClosestConsoleColor(r, g, b);
                    Console.BackgroundColor = color;
                    Console.Clear();
                    Console.WriteLine($"Background color changed to: #{hexColor}");
                }
                else
                {
                    ShowError("Invalid hex color format. Use format: #RRGGBB or RRGGBB");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error changing background color: {ex.Message}");
            }
        }

        private ConsoleColor GetClosestConsoleColor(int r, int g, int b)
        {
            // 简单的颜色映射逻辑
            // 将RGB值映射到最接近的ConsoleColor
            ConsoleColor[] colors = new ConsoleColor[]
            {
                ConsoleColor.Black,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkYellow,
                ConsoleColor.Gray,
                ConsoleColor.DarkGray,
                ConsoleColor.Blue,
                ConsoleColor.Green,
                ConsoleColor.Cyan,
                ConsoleColor.Red,
                ConsoleColor.Magenta,
                ConsoleColor.Yellow,
                ConsoleColor.White
            };
            ConsoleColor closestColor = ConsoleColor.Black;
            double smallestDistance = double.MaxValue;

            foreach (ConsoleColor color in colors)
            {
                // 为每个ConsoleColor计算RGB值
                // 这里使用简单的映射，实际效果可能不是很准确
                int cr, cg, cb;
                GetRGBFromConsoleColor(color, out cr, out cg, out cb);

                // 计算欧几里得距离
                double distance = Math.Sqrt(Math.Pow(r - cr, 2) + Math.Pow(g - cg, 2) + Math.Pow(b - cb, 2));
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestColor = color;
                }
            }

            return closestColor;
        }

        private void GetRGBFromConsoleColor(ConsoleColor color, out int r, out int g, out int b)
        {
            // 简单的ConsoleColor到RGB的映射
            switch (color)
            {
                case ConsoleColor.Black:
                    r = 0; g = 0; b = 0; break;
                case ConsoleColor.DarkBlue:
                    r = 0; g = 0; b = 128; break;
                case ConsoleColor.DarkGreen:
                    r = 0; g = 128; b = 0; break;
                case ConsoleColor.DarkCyan:
                    r = 0; g = 128; b = 128; break;
                case ConsoleColor.DarkRed:
                    r = 128; g = 0; b = 0; break;
                case ConsoleColor.DarkMagenta:
                    r = 128; g = 0; b = 128; break;
                case ConsoleColor.DarkYellow:
                    r = 128; g = 128; b = 0; break;
                case ConsoleColor.Gray:
                    r = 192; g = 192; b = 192; break;
                case ConsoleColor.DarkGray:
                    r = 128; g = 128; b = 128; break;
                case ConsoleColor.Blue:
                    r = 0; g = 0; b = 255; break;
                case ConsoleColor.Green:
                    r = 0; g = 255; b = 0; break;
                case ConsoleColor.Cyan:
                    r = 0; g = 255; b = 255; break;
                case ConsoleColor.Red:
                    r = 255; g = 0; b = 0; break;
                case ConsoleColor.Magenta:
                    r = 255; g = 0; b = 255; break;
                case ConsoleColor.Yellow:
                    r = 255; g = 255; b = 0; break;
                case ConsoleColor.White:
                    r = 255; g = 255; b = 255; break;
                default:
                    r = 0; g = 0; b = 0; break;
            }
        }

        private void TestCUI()
        {
            // 创建CUI实例
            var cui = new CUI("CMLeonOS CUI Test");
            
            // 设置状态
            cui.SetStatus("Testing CUI...");
            
            // 渲染CUI界面（只渲染顶栏）
            cui.Render();
            
            // 显示测试消息
            Console.WriteLine();
            Console.WriteLine("CUI Framework Test");
            Console.WriteLine("-------------------");
            Console.WriteLine("Testing CUI functionality...");
            Console.WriteLine();
            
            // 测试不同类型的消息
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: Success message test");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Error message test");
            Console.ResetColor();
            
            Console.WriteLine("Normal message test");
            Console.WriteLine();
            
            // 测试用户输入
            Console.Write("Enter your name: ");
            var input = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine($"Hello, {input}!");
            Console.WriteLine();
            
            // 渲染底栏
            cui.RenderBottomBar();
            
            // 等待用户按任意键返回
            Console.WriteLine();
            Console.WriteLine("Press any key to return to shell...");
            Console.ReadKey(true);
            
            // 重置控制台
            Console.Clear();
        }

        private void EditFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ShowError("Please specify a file name");
                return;
            }
            
            try
            {
                var editor = new Editor(fileName, fileSystem);
                editor.Run();
            }
            catch (Exception ex)
            {
                ShowError($"Error starting editor: {ex.Message}");
            }
        }

        private void NanoFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ShowError("Please specify a file name");
                return;
            }
            
            try
            {
                var nano = new Nano(fileName, true, fileSystem, userSystem, this);
                nano.Start();
            }
            catch (Exception ex)
            {
                ShowError($"Error starting nano: {ex.Message}");
            }
        }

        private void DiffFiles(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Usage: diff <file1> <file2>");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 2)
            {
                ShowError("Usage: diff <file1> <file2>");
                return;
            }
            
            string file1Path = fileSystem.GetFullPath(parts[0]);
            string file2Path = fileSystem.GetFullPath(parts[1]);
            
            if (!System.IO.File.Exists(file1Path))
            {
                ShowError($"File not found: {parts[0]}");
                return;
            }
            
            if (!System.IO.File.Exists(file2Path))
            {
                ShowError($"File not found: {parts[1]}");
                return;
            }
            
            try
            {
                string[] file1Lines = fileSystem.ReadFile(file1Path).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                string[] file2Lines = fileSystem.ReadFile(file2Path).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                
                int maxLines = Math.Max(file1Lines.Length, file2Lines.Length);
                bool hasDifferences = false;
                
                Console.WriteLine($"Comparing {parts[0]} and {parts[1]}:");
                Console.WriteLine();
                
                for (int i = 0; i < maxLines; i++)
                {
                    string line1 = i < file1Lines.Length ? file1Lines[i] : "";
                    string line2 = i < file2Lines.Length ? file2Lines[i] : "";
                    
                    if (line1 != line2)
                    {
                        hasDifferences = true;
                        
                        if (i < file1Lines.Length && i < file2Lines.Length)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"< {line1}");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"> {line2}");
                        }
                        else if (i < file1Lines.Length)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"< {line1}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"> {line2}");
                        }
                        
                        Console.ResetColor();
                    }
                }
                
                if (!hasDifferences)
                {
                    ShowSuccess("Files are identical");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"Differences found. Total lines: {maxLines}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error comparing files: {ex.Message}");
            }
        }

        private void ShowCalendar(string args)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            
            if (!string.IsNullOrEmpty(args))
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length >= 1)
                {
                    if (int.TryParse(parts[0], out int m) && m >= 1 && m <= 12)
                    {
                        month = m;
                    }
                }
                
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[1], out int y) && y >= 1 && y <= 9999)
                    {
                        year = y;
                    }
                }
            }
            
            DateTime firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DayOfWeek startDayOfWeek = firstDay.DayOfWeek;
            
            string[] monthNames = { 
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };
            
            Console.WriteLine($"     {monthNames[month - 1]} {year}");
            Console.WriteLine("  Su Mo Tu We Th Fr Sa");
            
            int dayOfWeek = (int)startDayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            
            for (int i = 1; i < dayOfWeek; i++)
            {
                Console.Write("   ");
            }
            
            for (int day = 1; day <= daysInMonth; day++)
            {
                Console.Write($"{day,2} ");
                
                dayOfWeek++;
                if (dayOfWeek > 7)
                {
                    dayOfWeek = 1;
                    Console.WriteLine();
                    Console.Write("  ");
                }
            }
            
            Console.WriteLine();
        }

        private void SleepCommand(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Usage: sleep <seconds>");
                return;
            }
            
            if (int.TryParse(args, out int seconds) && seconds > 0)
            {
                // Console.WriteLine($"Sleeping for {seconds} second(s)...");
                Thread.Sleep(seconds * 1000);
                // Console.WriteLine("Done.");
            }
            else
            {
                ShowError("Invalid time. Please specify a positive integer in seconds.");
            }
        }

        private void ExecuteCommandFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Usage: com <filename.cm>");
                return;
            }
            
            string filePath = fileSystem.GetFullPath(args);
            
            if (!System.IO.File.Exists(filePath))
            {
                ShowError($"File not found: {args}");
                return;
            }
            
            try
            {
                string[] lines = fileSystem.ReadFile(filePath).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                
                if (lines.Length == 0 || (lines.Length == 1 && string.IsNullOrWhiteSpace(lines[0])))
                {
                    ShowWarning("Command file is empty");
                    return;
                }
                
                // Console.WriteLine($"Executing command file: {args}");
                // Console.WriteLine("--------------------------------");
                
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                    {
                        continue;
                    }
                    
                    ExecuteCommand(line);
                }
                
                // Console.WriteLine("--------------------------------");
                // ShowSuccess("Command file execution completed");
            }
            catch (Exception ex)
            {
                ShowError($"Error executing command file: {ex.Message}");
            }
        }

        private void HeadFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify a file name");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string fileName = parts[0];
                int lineCount = 10; // 默认显示10行
                
                if (parts.Length > 1)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        lineCount = count;
                    }
                }
                
                string content = fileSystem.ReadFile(fileName);
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty");
                    return;
                }
                
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int displayLines = Math.Min(lineCount, lines.Length);
                
                Console.WriteLine($"First {displayLines} lines of {fileName}:");
                Console.WriteLine("--------------------------------");
                
                for (int i = 0; i < displayLines; i++)
                {
                    Console.WriteLine($"{i + 1}: {lines[i]}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error reading file: {ex.Message}");
            }
        }

        private void TailFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify a file name");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string fileName = parts[0];
                int lineCount = 10; // 默认显示10行
                
                if (parts.Length > 1)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        lineCount = count;
                    }
                }
                
                string content = fileSystem.ReadFile(fileName);
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty");
                    return;
                }
                
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int displayLines = Math.Min(lineCount, lines.Length);
                int startLine = Math.Max(0, lines.Length - displayLines);
                
                Console.WriteLine($"Last {displayLines} lines of {fileName}:");
                Console.WriteLine("--------------------------------");
                
                for (int i = startLine; i < lines.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {lines[i]}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error reading file: {ex.Message}");
            }
        }

        private void WordCount(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify a file name");
                return;
            }
            
            try
            {
                string content = fileSystem.ReadFile(args);
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty");
                    return;
                }
                
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int lineCount = lines.Length;
                int wordCount = 0;
                int charCount = content.Length;
                
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        wordCount += words.Length;
                    }
                }
                
                Console.WriteLine($"Word count for {args}:");
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Lines: {lineCount}");
                Console.WriteLine($"Words: {wordCount}");
                Console.WriteLine($"Characters: {charCount}");
            }
            catch (Exception ex)
            {
                ShowError($"Error reading file: {ex.Message}");
            }
        }

        private void CopyFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify source and destination files");
                ShowError("cp <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    ShowError("Please specify both source and destination");
                    ShowError("cp <source> <destination>");
                    return;
                }
                
                string sourceFile = parts[0];
                string destFile = parts[1];
                
                // 使用FileSystem读取源文件内容
                string content = fileSystem.ReadFile(sourceFile);
                if (content == null)
                {
                    ShowError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                // 使用FileSystem写入目标文件
                fileSystem.WriteFile(destFile, content);
                ShowSuccess($"File copied successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                ShowError($"Error copying file: {ex.Message}");
            }
        }

        private void MoveFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify source and destination files");
                ShowError("mv <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    ShowError("Please specify both source and destination");
                    ShowError("mv <source> <destination>");
                    return;
                }
                
                string sourceFile = parts[0];
                string destFile = parts[1];
                
                // 使用FileSystem读取源文件内容
                string content = fileSystem.ReadFile(sourceFile);
                if (content == null)
                {
                    ShowError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                // 使用FileSystem写入目标文件
                fileSystem.WriteFile(destFile, content);
                
                // 删除源文件
                fileSystem.DeleteFile(sourceFile);
                
                ShowSuccess($"File moved/renamed successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                ShowError($"Error moving file: {ex.Message}");
            }
        }

        private void RenameFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify source and new name");
                ShowError("rename <source> <newname>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    ShowError("Please specify both source and new name");
                    ShowError("rename <source> <newname>");
                    return;
                }
                
                string sourceFile = parts[0];
                string newName = parts[1];
                
                string sourcePath = fileSystem.GetFullPath(sourceFile);
                string destPath = fileSystem.GetFullPath(newName);
                
                if (!System.IO.File.Exists(sourcePath))
                {
                    ShowError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                if (System.IO.File.Exists(destPath))
                {
                    ShowError($"Destination '{newName}' already exists");
                    return;
                }
                
                string content = fileSystem.ReadFile(sourcePath);
                System.IO.File.WriteAllText(destPath, content);
                fileSystem.DeleteFile(sourcePath);
                
                ShowSuccess($"File renamed successfully from '{sourceFile}' to '{newName}'");
            }
            catch (Exception ex)
            {
                ShowError($"Error renaming file: {ex.Message}");
            }
        }

        private void CreateEmptyFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify a file name");
                ShowError("touch <filename>");
                return;
            }
            
            try
            {
                // 使用FileSystem创建空文件
                fileSystem.WriteFile(args, "");
                ShowSuccess($"Empty file '{args}' created successfully");
            }
            catch (Exception ex)
            {
                ShowError($"Error creating file: {ex.Message}");
            }
        }

        private void FindFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Please specify a file name to search");
                ShowError("find <filename>");
                return;
            }
            
            try
            {
                // 使用FileSystem获取当前目录的文件列表
                var files = fileSystem.GetFileList(".");
                bool found = false;
                
                foreach (var file in files)
                {
                    // 检查文件名是否包含搜索字符串
                    if (file.ToLower().Contains(args.ToLower()))
                    {
                        Console.WriteLine($"Found: {file}");
                        found = true;
                    }
                }
                
                if (!found)
                {
                    Console.WriteLine($"No files found matching '{args}'");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error finding file: {ex.Message}");
            }
        }

        private void GetDiskInfo()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Disk Information");
            Console.WriteLine("====================================");
            
            try
            {
                // 使用VFSManager获取所有磁盘
                var disks = Sys.FileSystem.VFS.VFSManager.GetDisks();
                
                if (disks == null || disks.Count == 0)
                {
                    Console.WriteLine("No disks found.");
                    return;
                }
                
                Console.WriteLine($"Total Disks: {disks.Count}");
            }
            catch (Exception ex)
            {
                ShowError($"Error getting disk info: {ex.Message}");
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

        private void ProcessUserCommand(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Error: Please specify a user command");
                ShowError("Please specify a user command");
                ShowError("user <add|delete> [args]");
                ShowError("  user add admin <username> <password>  - Add admin user");
                ShowError("  user add user <username> <password>      - Add regular user");
                ShowError("  user delete <username>                    - Delete user");
                ShowError("  user list                                    - List all users");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                ShowError("Error: Please specify a user command");
                ShowError("Usage: user <add|delete> [args]");
                return;
            }
            
            string subCommand = parts[0].ToLower();
            
            if (subCommand == "add")
            {
                if (parts.Length < 4)
                {
                    ShowError("Error: Please specify user type and username and password");
                    ShowError("Usage: user add admin <username> <password>");
                    ShowError("Usage: user add user <username> <password>");
                    return;
                }
                
                string userType = parts[1].ToLower();
                string username = parts[2];
                string password = parts[3];
                bool isAdmin = userType == "admin";
                
                userSystem.AddUser($"{username} {password}", isAdmin);
            }
            else if (subCommand == "delete")
            {
                if (parts.Length < 2)
                {
                    ShowError("Error: Please specify username");
                    ShowError("Usage: user delete <username>");
                    return;
                }
                
                string username = parts[1];
                userSystem.DeleteUser(username);
            }
            else if (subCommand == "list")
            {
                userSystem.ListUsers();
            }
            else
            {
                ShowError($"Error: Unknown user command '{subCommand}'");
                ShowError("Available commands: add, delete, list");
            }
        }

        private void ProcessBransweCommand(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Error: Please specify file name");
                ShowError("Usage: branswe <filename>");
                return;
            }
            
            string filePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(filePath))
            {
                ShowError($"Error: File not found: {args}");
                return;
            }
            
            try
            {
                string fileContent = File.ReadAllText(filePath);
                // Console.WriteLine($"Executing Branswe code from: {args}");
                Branswe.Run(fileContent);
                // Console.WriteLine("Branswe execution completed.");
            }
            catch (Exception ex)
            {
                ShowError($"Error executing Branswe: {ex.Message}");
            }
        }

        private void GrepFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Error: Please specify file name and search pattern");
                ShowError("Usage: grep <pattern> <filename>");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                ShowError("Error: Please specify both pattern and filename");
                ShowError("Usage: grep <pattern> <filename>");
                return;
            }
            
            string pattern = parts[0];
            string fileName = parts[1];
            
            try
            {
                string filePath = fileSystem.GetFullPath(fileName);
                
                if (!File.Exists(filePath))
                {
                    ShowError($"Error: File not found: {fileName}");
                    return;
                }
                
                string content = File.ReadAllText(filePath);
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                
                int matchCount = 0;
                Console.WriteLine($"Searching for '{pattern}' in {fileName}:");
                Console.WriteLine("--------------------------------");
                
                foreach (string line in lines)
                {
                    if (line.Contains(pattern))
                    {
                        matchCount++;
                        int lineNumber = Array.IndexOf(lines, line) + 1;
                        Console.WriteLine($"  Line {lineNumber}: {line}");
                    }
                }
                
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Found {matchCount} matches");
            }
            catch (Exception ex)
            {
                ShowError($"Error searching file: {ex.Message}");
            }
        }

        private void ProcessEnvCommand(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                envManager.ListVariables();
                return;
            }
            
            string command = parts[0].ToLower();
            
            switch (command)
            {
                case "list":
                    envManager.ListVariables();
                    break;
                case "see":
                    if (parts.Length >= 2)
                    {
                        string varName = parts[1];
                        string varValue = envManager.GetVariable(varName);
                        if (varValue != null)
                        {
                            Console.WriteLine($"  {varName}={varValue}");
                        }
                        else
                        {
                            ShowError($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        ShowError("Error: Please specify variable name");
                        ShowError("Usage: env see <varname>");
                    }
                    break;
                case "add":
                    if (parts.Length >= 3)
                    {
                        string varName = parts[1];
                        string varValue = parts.Length > 2 ? string.Join(" ", parts.Skip(2).ToArray()) : "";
                        
                        if (envManager.SetVariable(varName, varValue))
                        {
                            Console.WriteLine($"Environment variable '{varName}' added");
                        }
                        else
                        {
                            ShowError($"Error: Failed to add environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        ShowError("Error: Please specify variable name and value");
                        ShowError("Usage: env add <varname> <value>");
                    }
                    break;
                case "change":
                    if (parts.Length >= 3)
                    {
                        string varName = parts[1];
                        string varValue = parts.Length > 2 ? string.Join(" ", parts.Skip(2).ToArray()) : "";
                        
                        if (envManager.SetVariable(varName, varValue))
                        {
                            Console.WriteLine($"Environment variable '{varName}' set to '{varValue}'");
                        }
                        else
                        {
                            ShowError($"Error: Failed to set environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        ShowError("Error: Please specify variable name and value");
                        ShowError("Usage: env change <varname> <value>");
                    }
                    break;
                case "delete":
                    if (parts.Length >= 2)
                    {
                        string varName = parts[1];
                        
                        if (envManager.DeleteVariable(varName))
                        {
                            Console.WriteLine($"Environment variable '{varName}' deleted");
                        }
                        else
                        {
                            ShowError($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        ShowError("Error: Please specify variable name");
                        ShowError("Usage: env delete <varname>");
                    }
                    break;
                default:
                    ShowError("Error: Invalid env command");
                    ShowError("Usage: env [list] | env see <varname> | env add <varname> <value> | env change <varname> <value> | env delete <varname>");
                    break;
            }
        }

        private void BackupSystem(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify backup name");
                ShowError("Usage: backup <backupname>");
                return;
            }
            
            string backupName = parts[0];
            string backupPath = $@"0:\backup\{backupName}";
            
            try
            {
                Console.WriteLine($"BackupSystem: Creating backup '{backupName}'");
                Console.WriteLine($"Backup path: {backupPath}");
                
                if (Directory.Exists(backupPath))
                {
                    ShowWarning($"Backup '{backupName}' already exists");
                    ShowWarning("Returning without creating new backup");
                    return;
                }
                
                Console.WriteLine($"BackupSystem: Creating backup directory: {backupPath}");
                Directory.CreateDirectory(backupPath);
                ShowSuccess($"Backup directory created");
                
                // 备份系统文件
                string sysPath = @"0:\system";
                Console.WriteLine($"BackupSystem: Checking system path: {sysPath}");
                Console.WriteLine($"BackupSystem: System path exists: {Directory.Exists(sysPath)}");
                
                if (Directory.Exists(sysPath))
                {
                    Console.WriteLine($"BackupSystem: Copying system files to backup");
                    CopyDirectory(sysPath, backupPath);
                    Console.WriteLine($"BackupSystem: System files copied");
                }
                else
                {
                    Console.WriteLine($"BackupSystem: System path does not exist, skipping system backup");
                }
                
                // 备份用户文件
                string userPath = @"0:\user";
                Console.WriteLine($"BackupSystem: Checking user path: {userPath}");
                Console.WriteLine($"BackupSystem: User path exists: {Directory.Exists(userPath)}");
                
                if (Directory.Exists(userPath))
                {
                    Console.WriteLine($"BackupSystem: Copying user files to backup");
                    CopyDirectory(userPath, backupPath);
                    Console.WriteLine($"BackupSystem: User files copied");
                }
                else
                {
                    Console.WriteLine($"BackupSystem: User path does not exist, skipping user backup");
                }
                
                ShowSuccess($"Backup '{backupName}' created successfully");
                ShowSuccess($"Backup location: {backupPath}");
            }
            catch (Exception ex)
            {
                ShowError($"Error creating backup: {ex.Message}");
                ShowError($"Exception type: {ex.GetType().Name}");
            }
        }

        private void RestoreSystem(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify backup name");
                ShowError("Usage: restore <backupname>");
                return;
            }
            
            string backupName = parts[0];
            string backupPath = $@"0:\backup\{backupName}";
            
            try
            {
                if (!Directory.Exists(backupPath))
                {
                    ShowError($"Error: Backup '{backupName}' not found");
                    return;
                }
                
                // 恢复系统文件
                string sysPath = @"0:\system";
                if (Directory.Exists(backupPath))
                {
                    CopyDirectory(backupPath, sysPath, true);
                }
                
                // 恢复用户文件
                string userPath = @"0:\user";
                if (Directory.Exists(backupPath))
                {
                    CopyDirectory(backupPath, userPath, true);
                }
                
                ShowSuccess($"Backup '{backupName}' restored successfully");
                ShowSuccess($"Backup location: {backupPath}");
            }
            catch (Exception ex)
            {
                ShowError($"Error restoring backup: {ex.Message}");
            }
        }

        private void CopyDirectory(string sourcePath, string destPath, bool overwrite = false)
        {
            try
            {
                Console.WriteLine($"CopyDirectory: Starting copy from '{sourcePath}' to '{destPath}'");
                Console.WriteLine($"CopyDirectory: Overwrite mode: {overwrite}");
                
                if (!Directory.Exists(destPath))
                {
                    Console.WriteLine($"CopyDirectory: Creating destination directory: {destPath}");
                    Directory.CreateDirectory(destPath);
                    Console.WriteLine($"CopyDirectory: Destination directory created");
                }
                
                string[] sourceFiles = Directory.GetFiles(sourcePath);
                Console.WriteLine($"CopyDirectory: Found {sourceFiles.Length} files in source directory");
                
                if (sourceFiles.Length == 0)
                {
                    ShowWarning($"CopyDirectory: Warning: No files found in source directory");
                    return;
                }
                
                int copiedCount = 0;
                int skippedCount = 0;
                
                foreach (string sourceFile in sourceFiles)
                {
                    string destFile = Path.Combine(destPath, Path.GetFileName(sourceFile));
                    
                    if (File.Exists(destFile) && !overwrite)
                    {
                        Console.WriteLine($"CopyDirectory: Skipping existing file: {destFile}");
                        skippedCount++;
                        continue;
                    }
                    
                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                        copiedCount++;
                        Console.WriteLine($"CopyDirectory: Copied {sourceFile} -> {destFile}");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"CopyDirectory: Error copying {sourceFile}: {ex.Message}");
                        ShowError($"CopyDirectory: Exception type: {ex.GetType().Name}");
                    }
                }
                
                Console.WriteLine($"CopyDirectory: Copy completed");
                Console.WriteLine($"CopyDirectory: Total files: {sourceFiles.Length}");
                Console.WriteLine($"CopyDirectory: Copied: {copiedCount}");
                Console.WriteLine($"CopyDirectory: Skipped: {skippedCount}");
            }
            catch (Exception ex)
            {
                ShowError($"Error copying directory: {ex.Message}");
                ShowError($"Exception type: {ex.GetType().Name}");
            }
        }

        private void ShowUptime()
        {
            try
            {
                Console.WriteLine("====================================");
                Console.WriteLine("        System Uptime");
                Console.WriteLine("====================================");
                Console.WriteLine();
                
                // 计算系统运行时间
                if (Kernel.SystemStartTime != DateTime.MinValue)
                {
                    TimeSpan uptime = DateTime.Now - Kernel.SystemStartTime;
                    
                    Console.WriteLine("System started: " + Kernel.SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    Console.WriteLine("Current time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    Console.WriteLine();
                    
                    // 格式化运行时间
                    int days = uptime.Days;
                    int hours = uptime.Hours;
                    int minutes = uptime.Minutes;
                    int seconds = uptime.Seconds;
                    
                    Console.WriteLine($"System uptime: {days} days, {hours} hours, {minutes} minutes, {seconds} seconds");
                    Console.WriteLine($"Total uptime: {uptime.TotalHours:F2} hours");
                }
                else
                {
                    ShowWarning("System start time not available.");
                    ShowWarning("System may have been started before uptime tracking was implemented.");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                ShowError($"Error showing uptime: {ex.Message}");
            }
        }

        private void CreateFTP()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        FTP Server");
            Console.WriteLine("====================================");
            Console.WriteLine();

            // Console.WriteLine("Starting FTP server...");
            // Console.WriteLine($"Root directory: 0:\\");
            // Console.WriteLine($"File system: {Kernel.fs.GetFileSystemType("0:\\")}");
            // Console.WriteLine($"Available space: {FormatBytes(Kernel.fs.GetAvailableFreeSpace("0:\\"))}");
            // Console.WriteLine();

            // Console.WriteLine("====================================");
            // Console.WriteLine("      Connection Information");
            // Console.WriteLine("====================================");

            try
            {
                if (Kernel.NetworkDevice != null && Kernel.IPAddress != "Unknown")
                {
                    Console.WriteLine($"FTP Server Address: ftp://{Kernel.IPAddress}");
                    Console.WriteLine($"FTP Port: 21");
                    Console.WriteLine($"IP Address: {Kernel.IPAddress}");
                }
                else
                {
                    Console.WriteLine("FTP Server Address: Not available (network not configured)");
                    Console.WriteLine("FTP Port: 21");
                    Console.WriteLine("IP Address: Unknown");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error while running: {ex.Message}");
                Console.WriteLine("FTP Server Address: Not available (network not configured)");
                Console.WriteLine("FTP Port: 21");
                Console.WriteLine("IP Address: Unknown");
            }

            Console.WriteLine();
            Console.WriteLine("====================================");
            Console.WriteLine("      Login Information");
            Console.WriteLine("====================================");
            Console.WriteLine("Default Username: anonymous");
            Console.WriteLine("Password: (no password required)");
            Console.WriteLine();
            Console.WriteLine("Note: You can also use system users to login");
            Console.WriteLine();

            Console.WriteLine("FTP server is listening for connections...");
            Console.WriteLine("Press Ctrl+C to stop the server");
            Console.WriteLine();

            using (var xServer = new FtpServer(Kernel.fs, "0:\\"))
            {
                try
                {
                    ShowSuccess("FTP server started successfully!");
                    // Console.WriteLine($"Server started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    // Console.WriteLine();

                    xServer.Listen();
                }
                catch (Exception ex)
                {
                    ShowError($"FTP server error: {ex.Message}");
                    // Console.WriteLine($"Error occurred at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }
            }

            Console.WriteLine();
            ShowWarning("FTP server stopped");
            // Console.WriteLine($"Server stopped at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        private void PingIP(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify IP address");
                ShowError("Usage: ping <ip>");
                return;
            }
            
            if (Kernel.NetworkDevice == null)
            {
                ShowError("Error: Network device not initialized");
                ShowError("Please check network configuration");
                return;
            }
            
            string ip = parts[0];
            
            if (!ParseAddress(ip, out Address address))
            {
                ShowError("Invalid IP address.");
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("====================================");
            Console.WriteLine($"        Pinging {address.ToString()}");
            Console.WriteLine("====================================");
            Console.WriteLine($"Using network device: {Kernel.NetworkDevice.Name}");
            Console.WriteLine($"Local IP: {Kernel.IPAddress}");
            Console.WriteLine();
            
            try
            {
                EndPoint endpoint = new EndPoint(Address.Zero, 0);
                
                int sent = 0;
                int received = 0;
                const int echoCount = 5;
                
                using (var icmp = new ICMPClient())
                {
                    icmp.Connect(address);
                    
                    for (int i = 0; i < echoCount; i++)
                    {
                        icmp.SendEcho();
                        sent++;
                        int time = icmp.Receive(ref endpoint);
                        
                        if (time != -1)
                        {
                            received++;
                            Console.WriteLine($"Reply from {address.ToString()}: time={time - 1}ms");
                        }
                        else
                        {
                            ShowError("Request timed out.");
                        }
                        
                        if (i < echoCount - 1)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    
                    icmp.Close();
                }
                
                Console.WriteLine();
                Console.WriteLine("====================================");
                Console.WriteLine("        Ping Statistics");
                Console.WriteLine("====================================");
                int lossPercent = (int)((sent - received) / (float)sent * 100);
                Console.WriteLine($"Packets: Sent = {sent}, Received = {received}, Lost = {sent - received} ({lossPercent}% loss)");
                Console.WriteLine();
                
                if (sent == received)
                {
                    ShowSuccess("Ping completed successfully");
                }
                else if (received > 0)
                {
                    ShowWarning("Ping completed with packet loss");
                }
                else
                {
                    ShowError("Ping failed - no response received");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ping error: {ex.Message}");
            }
        }

        private bool ParseAddress(string ip, out Address address)
        {
            string[] octetStrings = ip.Split('.');
            byte[] octets = new byte[4];
            
            if (octetStrings.Length != 4)
            {
                address = Address.Zero;
                return false;
            }
            
            for (int i = 0; i < octetStrings.Length; i++)
            {
                if (byte.TryParse(octetStrings[i], out byte octet))
                {
                    octets[i] = octet;
                }
                else
                {
                    address = Address.Zero;
                    return false;
                }
            }
            
            address = new Address(octets[0], octets[1], octets[2], octets[3]);
            return true;
        }

        private void StartTcpServer(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify port number");
                ShowError("Usage: tcpserver <port>");
                return;
            }
            
            if (!int.TryParse(parts[0], out int port))
            {
                ShowError("Error: Invalid port number");
                return;
            }
            
            if (port < 1 || port > 65535)
            {
                ShowError("Error: Port must be between 1 and 65535");
                return;
            }
            
            Console.WriteLine("====================================");
            Console.WriteLine("        TCP Server");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Starting TCP server on port {port}...");
            Console.WriteLine($"Local IP: {Kernel.IPAddress}");
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop the server");
            Console.WriteLine();
            
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                
                ShowSuccess($"TCP server started on port {port}");
                Console.WriteLine("Waiting for connections...");
                Console.WriteLine();
                
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HandleTcpClient(client);
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError($"TCP server error: {ex.Message}");
            }
        }

        private void HandleTcpClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead;
                
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                
                while (true)
                {
                    bytesRead = 0;
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected");
                        break;
                    }
                    
                    string received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {received}");
                    
                    byte[] response = Encoding.ASCII.GetBytes("OK");
                    stream.Write(response, 0, response.Length);
                    Console.WriteLine("Sent: OK");
                }
                
                stream.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error handling client: {ex.Message}");
            }
        }

        private void ConnectTcpClient(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 2)
            {
                ShowError("Error: Please specify IP address and port");
                ShowError("Usage: tcpclient <ip> <port>");
                return;
            }
            
            string serverIp = parts[0];
            
            if (!int.TryParse(parts[1], out int serverPort))
            {
                ShowError("Error: Invalid port number");
                return;
            }
            
            if (serverPort < 1 || serverPort > 65535)
            {
                ShowError("Error: Port must be between 1 and 65535");
                return;
            }
            
            Console.WriteLine("====================================");
            Console.WriteLine("        TCP Client");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Connecting to {serverIp}:{serverPort}...");
            
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIp, serverPort);
                    ShowSuccess($"Connected to {serverIp}:{serverPort}");
                    Console.WriteLine();
                    
                    NetworkStream stream = client.GetStream();
                    
                    Console.WriteLine("Enter message to send (or 'exit' to quit):");
                    
                    while (true)
                    {
                        Console.Write("> ");
                        string messageToSend = Console.ReadLine();
                        
                        if (messageToSend.ToLower() == "exit")
                        {
                            break;
                        }
                        
                        if (string.IsNullOrWhiteSpace(messageToSend))
                        {
                            continue;
                        }
                        
                        byte[] dataToSend = Encoding.ASCII.GetBytes(messageToSend);
                        stream.Write(dataToSend, 0, dataToSend.Length);
                        Console.WriteLine($"Sent: {messageToSend}");
                        
                        byte[] receivedData = new byte[client.ReceiveBufferSize];
                        int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);
                        Console.WriteLine($"Received: {receivedMessage}");
                        Console.WriteLine();
                    }
                    
                    stream.Close();
                }
                
                ShowSuccess("Connection closed");
            }
            catch (Exception ex)
            {
                ShowError($"TCP client error: {ex.Message}");
            }
        }

        private void DownloadFile(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify URL");
                ShowError("Usage: wget <url> [output]");
                return;
            }
            
            string url = parts[0];
            string outputPath = parts.Length > 1 ? parts[1] : "";
            
            Console.WriteLine("====================================");
            Console.WriteLine("        WGET - File Downloader");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Downloading from: {url}");
            
            try
            {
                HttpRequest request = new HttpRequest();
                
                string domain = "";
                string path = "/";
                
                if (url.StartsWith("http://"))
                {
                    url = url.Substring(7);
                }
                if (url.StartsWith("https://"))
                {
                    url = url.Substring(8);
                }
                
                int slashIndex = url.IndexOf('/');
                if (slashIndex == -1)
                {
                    domain = url;
                }
                else
                {
                    domain = url.Substring(0, slashIndex);
                    path = url.Substring(slashIndex);
                }
                
                if (string.IsNullOrWhiteSpace(domain))
                {
                    ShowError("Error: Invalid URL format");
                    return;
                }
                
                Console.WriteLine($"Domain: {domain}");
                Console.WriteLine($"Path: {path}");
                Console.WriteLine();
                
                request.Domain = domain;
                request.Path = path;
                request.Method = "GET";
                
                Console.WriteLine("Sending request...");
                request.Send();
                
                if (request.Response == null)
                {
                    ShowError("Error: No response received");
                    return;
                }
                
                byte[] content = Encoding.ASCII.GetBytes(request.Response.Content);
                
                if (content == null || content.Length == 0)
                {
                    ShowError("Error: No content received");
                    return;
                }
                
                Console.WriteLine($"Downloaded {content.Length} bytes");
                Console.WriteLine();
                
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    int lastSlash = path.LastIndexOf('/');
                    if (lastSlash >= 0 && lastSlash < path.Length - 1)
                    {
                        outputPath = path.Substring(lastSlash + 1);
                    }
                    else
                    {
                        outputPath = "downloaded_file";
                    }
                }
                
                if (!outputPath.StartsWith("0:\\") && !outputPath.StartsWith("0:/"))
                {
                    outputPath = Path.Combine(prompt, outputPath);
                }
                
                Console.WriteLine($"Saving to: {outputPath}");
                
                File.WriteAllBytes(outputPath, content);
                
                ShowSuccess($"File saved successfully: {outputPath}");
            }
            catch (Exception ex)
            {
                ShowError($"Download error: {ex.Message}");
            }
        }

        private void ShowCurrentUsername()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Current User");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Username: {userSystem.CurrentUsername}");
            Console.WriteLine();
        }

        private void ProcessBase64Command(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify subcommand");
                ShowError("Usage: base64 encrypt <text> | base64 decrypt <text>");
                return;
            }
            
            string subcommand = parts[0].ToLower();
            
            if (subcommand != "encrypt" && subcommand != "decrypt")
            {
                ShowError("Error: Invalid subcommand");
                ShowError("Usage: base64 encrypt <text> | base64 decrypt <text>");
                return;
            }
            
            if (parts.Length < 2)
            {
                ShowError("Error: Please specify text to process");
                ShowError($"Usage: base64 {subcommand} <text>");
                return;
            }
            
            string text = string.Join(" ", parts, 1, parts.Length - 1);
            
            Console.WriteLine("====================================");
            Console.WriteLine("        Base64");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            try
            {
                if (subcommand == "encrypt")
                {
                    string encoded = Base64Helper.Encode(text);
                    Console.WriteLine($"Original: {text}");
                    Console.WriteLine();
                    Console.WriteLine($"Encoded: {encoded}");
                }
                if (subcommand == "decrypt")
                {
                    string decoded = Base64Helper.Decode(text);
                    Console.WriteLine($"Encoded: {text}");
                    Console.WriteLine();
                    Console.WriteLine($"Decoded: {decoded}");
                }
                
                Console.WriteLine();
                ShowSuccess("Base64 operation completed");
            }
            catch (Exception ex)
            {
                ShowError($"Base64 error: {ex.Message}");
            }
        }

        private void ExecuteLuaScript(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                ShowError("Error: Please specify Lua script file");
                ShowError("Usage: lua <file>");
                return;
            }
            
            string filePath = parts[0];
            string originalPath = filePath;
            
            if (!filePath.StartsWith("0:\\") && !filePath.StartsWith("0:/"))
            {
                if (prompt == "/" || prompt == "\\")
                {
                    filePath = "0:\\" + filePath.TrimStart('/').TrimStart('\\');
                }
                else
                {
                    filePath = Path.Combine(prompt, filePath);
                }
            }
            
            if (!File.Exists(filePath))
            {
                ShowError($"Error: File not found: {filePath}");
                return;
            }
            
            // Console.WriteLine($"Executing: {filePath}");
            // Console.WriteLine();
            
            try
            {
                string scriptContent = File.ReadAllText(filePath);
                
                if (string.IsNullOrWhiteSpace(scriptContent))
                {
                    ShowWarning("Script file is empty");
                    return;
                }
                
                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();
                
                UniLua.ThreadStatus loadResult = lua.L_LoadString(scriptContent);
                
                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                    
                    if (callResult == UniLua.ThreadStatus.LUA_OK)
                    {
                        // ShowSuccess("Script run successfully");
                    }
                    else
                    {
                        string errorMsg = lua.ToString(-1);
                        ShowError($"Script execution error: {errorMsg}");
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    ShowError($"Script load error: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lua execution error: {ex.Message}");
            }
        }
    }
}