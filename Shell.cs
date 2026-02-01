using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sys = Cosmos.System;

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
                        "  branswe <filename> - Execute Branswe code file",
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
                        Console.WriteLine("Error: Please specify a directory name");
                    }
                    else
                    {
                        fileSystem.MakeDirectory(args);
                    }
                    break;
                case "rm":
                    if (string.IsNullOrEmpty(args))
                    {
                        Console.WriteLine("Error: Please specify a file name");
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
                                Console.WriteLine("Error: Cannot delete files in sys folder without -norisk parameter");
                                Console.WriteLine("Usage: rm <file> -norisk");
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
                        Console.WriteLine("Error: Please specify a directory name");
                    }
                    else
                    {
                        // 检查是否在sys文件夹中（修复模式下绕过检测）
                        bool isInSysFolder = (args.Contains(@"\system\") || args.Contains(@"/sys/")) && !fixMode;
                        
                        if (isInSysFolder)
                        {
                            Console.WriteLine("Error: Cannot delete directories in sys folder");
                            Console.WriteLine("Use fix mode to bypass this restriction");
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
                        Console.WriteLine("Error: Please specify a file name");
                    }
                    else
                    {
                        Console.WriteLine(fileSystem.ReadFile(args));
                    }
                    break;
                case "version":
                    Console.WriteLine("CMLeonOS v1.0");
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
                case "grep":
                    GrepFile(args);
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
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
                                Console.WriteLine("Error: Division by zero");
                                return;
                            }
                            break;
                        default:
                            Console.WriteLine("Error: Invalid operator. Use +, -, *, /");
                            return;
                    }

                    Console.WriteLine($"Result: {result}");
                }
                else
                {
                    Console.WriteLine("Error: Invalid expression. Use format: calc <num> <op> <num>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                    Console.WriteLine("Error: Invalid hex color format. Use format: #RRGGBB or RRGGBB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing background color: {ex.Message}");
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
                Console.WriteLine("Error: Please specify a file name");
                return;
            }
            
            try
            {
                var editor = new Editor(fileName, fileSystem);
                editor.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting editor: {ex.Message}");
            }
        }

        private void HeadFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify a file name");
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
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }

        private void TailFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify a file name");
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
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }

        private void WordCount(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify a file name");
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
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }

        private void CopyFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify source and destination files");
                Console.WriteLine("Usage: cp <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    Console.WriteLine("Error: Please specify both source and destination");
                    Console.WriteLine("Usage: cp <source> <destination>");
                    return;
                }
                
                string sourceFile = parts[0];
                string destFile = parts[1];
                
                // 使用FileSystem读取源文件内容
                string content = fileSystem.ReadFile(sourceFile);
                if (content == null)
                {
                    Console.WriteLine($"Error: Source file '{sourceFile}' does not exist");
                    return;
                }
                
                // 使用FileSystem写入目标文件
                fileSystem.WriteFile(destFile, content);
                Console.WriteLine($"File copied successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file: {ex.Message}");
            }
        }

        private void MoveFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify source and destination files");
                Console.WriteLine("Usage: mv <source> <destination>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    Console.WriteLine("Error: Please specify both source and destination");
                    Console.WriteLine("Usage: mv <source> <destination>");
                    return;
                }
                
                string sourceFile = parts[0];
                string destFile = parts[1];
                
                // 使用FileSystem读取源文件内容
                string content = fileSystem.ReadFile(sourceFile);
                if (content == null)
                {
                    Console.WriteLine($"Error: Source file '{sourceFile}' does not exist");
                    return;
                }
                
                // 使用FileSystem写入目标文件
                fileSystem.WriteFile(destFile, content);
                
                // 删除源文件
                fileSystem.DeleteFile(sourceFile);
                
                Console.WriteLine($"File moved/renamed successfully from '{sourceFile}' to '{destFile}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving file: {ex.Message}");
            }
        }

        private void CreateEmptyFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify a file name");
                Console.WriteLine("Usage: touch <filename>");
                return;
            }
            
            try
            {
                // 使用FileSystem创建空文件
                fileSystem.WriteFile(args, "");
                Console.WriteLine($"Empty file '{args}' created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
            }
        }

        private void FindFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify a file name to search");
                Console.WriteLine("Usage: find <filename>");
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
                Console.WriteLine($"Error finding file: {ex.Message}");
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
                Console.WriteLine($"Error getting disk info: {ex.Message}");
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
                Console.WriteLine("Error: Please specify a user command");
                Console.WriteLine("Usage: user <add|delete> [args]");
                Console.WriteLine("  user add admin <username> <password>  - Add admin user");
                Console.WriteLine("  user add user <username> <password>      - Add regular user");
                Console.WriteLine("  user delete <username>                    - Delete user");
                Console.WriteLine("  user list                                    - List all users");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                Console.WriteLine("Error: Please specify a user command");
                Console.WriteLine("Usage: user <add|delete> [args]");
                return;
            }
            
            string subCommand = parts[0].ToLower();
            
            if (subCommand == "add")
            {
                if (parts.Length < 4)
                {
                    Console.WriteLine("Error: Please specify user type and username and password");
                    Console.WriteLine("Usage: user add admin <username> <password>");
                    Console.WriteLine("Usage: user add user <username> <password>");
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
                    Console.WriteLine("Error: Please specify username");
                    Console.WriteLine("Usage: user delete <username>");
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
                Console.WriteLine($"Error: Unknown user command '{subCommand}'");
                Console.WriteLine("Available commands: add, delete, list");
            }
        }

        private void ProcessBransweCommand(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify file name");
                Console.WriteLine("Usage: branswe <filename>");
                return;
            }
            
            string filePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found: {args}");
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
                Console.WriteLine($"Error executing Branswe: {ex.Message}");
            }
        }

        private void GrepFile(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Error: Please specify file name and search pattern");
                Console.WriteLine("Usage: grep <pattern> <filename>");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                Console.WriteLine("Error: Please specify both pattern and filename");
                Console.WriteLine("Usage: grep <pattern> <filename>");
                return;
            }
            
            string pattern = parts[0];
            string fileName = parts[1];
            
            try
            {
                string filePath = fileSystem.GetFullPath(fileName);
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File not found: {fileName}");
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
                Console.WriteLine($"Error searching file: {ex.Message}");
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
                            Console.WriteLine($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Please specify variable name");
                        Console.WriteLine("Usage: env see <varname>");
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
                            Console.WriteLine($"Error: Failed to add environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Please specify variable name and value");
                        Console.WriteLine("Usage: env add <varname> <value>");
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
                            Console.WriteLine($"Error: Failed to set environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Please specify variable name and value");
                        Console.WriteLine("Usage: env change <varname> <value>");
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
                            Console.WriteLine($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Please specify variable name");
                        Console.WriteLine("Usage: env delete <varname>");
                    }
                    break;
                default:
                    Console.WriteLine("Error: Invalid env command");
                    Console.WriteLine("Usage: env [list] | env see <varname> | env add <varname> <value> | env change <varname> <value> | env delete <varname>");
                    break;
            }
        }
    }
}