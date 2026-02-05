using CosmosFtpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sys = Cosmos.System;
using Cosmos.System.Network;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
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
            fixMode = Kernel.FixMode;
            envManager = EnvironmentVariableManager.Instance;
            
            Commands.AliasCommand.LoadAliases();
            
            User currentUser = userSystem.CurrentLoggedInUser;
            if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Username))
            {
                string userHomePath = $@"0:\user\{currentUser.Username}";
                try
                {
                    if (!Directory.Exists(userHomePath))
                    {
                        Directory.CreateDirectory(userHomePath);
                    }
                    fileSystem = new FileSystem(userHomePath);
                }
                catch (Exception)
                {
                    fileSystem = new FileSystem();
                }
            }
            else
            {
                fileSystem = new FileSystem();
            }
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
            string expandedCommand = command;
            string expandedArgs = args;
            
            string aliasValue = Commands.AliasCommand.GetAlias(command);
            if (aliasValue != null)
            {
                var aliasParts = aliasValue.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (aliasParts.Length > 0)
                {
                    expandedCommand = aliasParts[0];
                    if (aliasParts.Length > 1)
                    {
                        expandedArgs = aliasParts[1] + (string.IsNullOrEmpty(args) ? "" : " " + args);
                    }
                    else
                    {
                        expandedArgs = args;
                    }
                }
            }
            
            shell.CommandList.ProcessCommand(this, expandedCommand, expandedArgs);
        }

        public void ProcessEcho(string args)
        {
            Commands.System.EchoCommand.ProcessEcho(args);
        }

        public void ProcessClear()
        {
            Commands.System.ClearCommand.ProcessClear();
        }

        public void ProcessRestart()
        {
            Commands.Power.RestartCommand.ProcessRestart();
        }

        public void ProcessShutdown()
        {
            Commands.Power.ShutdownCommand.ProcessShutdown();
        }

        public void ProcessHelp(string args)
        {
            Commands.HelpCommand.ProcessHelp(args);
        }

        public void ProcessTime()
        {
            Commands.System.TimeCommand.ProcessTime();
        }

        public void ProcessDate()
        {
            Commands.System.DateCommand.ProcessDate();
        }

        public void ProcessLs(string args)
        {
            Commands.FileSystem.LsCommand.ProcessLs(fileSystem, args);
        }

        public void ProcessCd(string args)
        {
            Commands.FileSystem.CdCommand.ProcessCd(fileSystem, args);
        }

        public void ProcessPwd()
        {
            Commands.FileSystem.PwdCommand.ProcessPwd(fileSystem);
        }

        public void ProcessMkdir(string args)
        {
            Commands.FileSystem.MkdirCommand.ProcessMkdir(fileSystem, args);
        }

        public void ProcessRm(string args)
        {
            Commands.FileSystem.RmCommand.ProcessRm(fileSystem, args, fixMode, ShowError);
        }

        public void ProcessRmdir(string args)
        {
            Commands.FileSystem.RmdirCommand.ProcessRmdir(fileSystem, args, fixMode, ShowError);
        }

        public void ProcessCat(string args)
        {
            Commands.FileSystem.CatCommand.ProcessCat(fileSystem, args, ShowError);
        }

        public void ProcessVersion()
        {
            Commands.VersionCommand.ProcessVersion();
        }

        public void ProcessAbout()
        {
            Commands.AboutCommand.ProcessAbout();
        }

        public void ProcessCpass()
        {
            Commands.User.CpassCommand.ProcessCpass(userSystem);
        }

        public void ProcessBeep()
        {
            Commands.Utility.BeepCommand.ProcessBeep();
        }

        public void ChangePrompt(string args)
        {
            Commands.Utility.PromptCommand.ChangePrompt(args, ref prompt);
        }

        public void Calculate(string expression)
        {
            Commands.Utility.CalcCommand.Calculate(expression, ShowError);
        }

        public void ShowHistory()
        {
            Commands.Utility.HistoryCommand.ShowHistory(commandHistory);
        }

        public void ChangeBackground(string hexColor)
        {
            Commands.Utility.BackgroundCommand.ChangeBackground(hexColor, ShowError);
        }

        public void TestCUI()
        {
            var cui = new CUI("CMLeonOS CUI Test");
            
            cui.SetStatus("Testing CUI...");
            
            cui.Render();
            
            Console.WriteLine();
            Console.WriteLine("CUI Framework Test");
            Console.WriteLine("-------------------");
            Console.WriteLine("Testing CUI functionality...");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: Success message test");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Error message test");
            Console.ResetColor();
            
            Console.WriteLine("Normal message test");
            Console.WriteLine();
            
            Console.Write("Enter your name: ");
            var input = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine($"Hello, {input}!");
            Console.WriteLine();
            
            cui.RenderBottomBar();
            
            Console.WriteLine();
            Console.WriteLine("Press any key to return to shell...");
            Console.ReadKey(true);
            
            Console.Clear();
        }

        public void EditFile(string fileName)
        {
            Commands.Editor.EditCommand.EditFile(fileName, fileSystem, ShowError);
        }

        public void NanoFile(string fileName)
        {
            Commands.Editor.NanoCommand.NanoFile(fileName, fileSystem, userSystem, this, ShowError);
        }

        public void DiffFiles(string args)
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

        public void ShowCalendar(string args)
        {
            Commands.Utility.CalendarCommand.ShowCalendar(args);
        }

        public void SleepCommand(string args)
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

        public void ExecuteCommandFile(string args)
        {
            Commands.Script.ComCommand.ExecuteCommandFile(args, fileSystem, this, ShowError, ShowWarning);
        }

        public void HeadFile(string args)
        {
            Commands.FileSystem.HeadCommand.HeadFile(fileSystem, args, ShowError);
        }

        public void TailFile(string args)
        {
            Commands.FileSystem.TailCommand.TailFile(fileSystem, args, ShowError);
        }

        public void WordCount(string args)
        {
            Commands.FileSystem.WordCountCommand.WordCount(fileSystem, args, ShowError);
        }

        public void CopyFile(string args)
        {
            Commands.FileSystem.CopyCommand.CopyFile(fileSystem, args, ShowError, ShowSuccess);
        }

        public void MoveFile(string args)
        {
            Commands.FileSystem.MoveCommand.MoveFile(fileSystem, args, ShowError, ShowSuccess);
        }

        public void RenameFile(string args)
        {
            Commands.FileSystem.RenameCommand.RenameFile(fileSystem, args, ShowError, ShowSuccess);
        }

        public void CreateEmptyFile(string args)
        {
            Commands.FileSystem.TouchCommand.CreateEmptyFile(fileSystem, args, ShowError, ShowSuccess);
        }

        public void FindFile(string args)
        {
            Commands.FileSystem.FindCommand.FindFile(fileSystem, args, ShowError);
        }

        public void ShowTree(string args)
        {
            Commands.FileSystem.TreeCommand.ShowTree(fileSystem, args, ShowError);
        }

        public void GetDiskInfo()
        {
            Commands.FileSystem.DiskInfoCommand.GetDiskInfo(ShowError);
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

        public void ProcessHostnameCommand(string args)
        {
            Commands.User.HostnameCommand.ProcessHostnameCommand(args, userSystem, ShowError);
        }

        public void ProcessUserCommand(string args)
        {
            Commands.User.UserCommand.ProcessUserCommand(args, userSystem, ShowError);
        }

        public void ProcessBransweCommand(string args)
        {
            Commands.Script.BransweCommand.ProcessBransweCommand(args, fileSystem, ShowError);
        }

        public void GrepFile(string args)
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

        public void ProcessEnvCommand(string args)
        {
            Commands.Environment.EnvCommand.ProcessEnvCommand(args, envManager, ShowError);
        }

        public void BackupSystem(string args)
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

        public void RestoreSystem(string args)
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

        public void ShowUptime()
        {
            Commands.System.UptimeCommand.ShowUptime(ShowError, ShowWarning);
        }

        public void CreateFTP()
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

        public void PingIP(string args)
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

        public void StartTcpServer(string args)
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

        public void ConnectTcpClient(string args)
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

        public void DownloadFile(string args)
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
                
                HttpRequest request = new HttpRequest();
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

        public void ShowCurrentUsername()
        {
            Commands.System.WhoamiCommand.ShowCurrentUsername(userSystem);
        }

        public void ProcessBase64Command(string args)
        {
            Commands.Utility.Base64Command.ProcessBase64Command(args, ShowError, ShowSuccess);
        }

        public void ExecuteLuaScript(string args)
        {
            Commands.Script.LuaCommand.ExecuteLuaScript(args, fileSystem, this, ShowError, ShowWarning);
        }

        public void ProcessTestGui()
        {
            Commands.TestGuiCommand.RunTestGui();
        }

        public void ProcessAlias(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Commands.AliasCommand.ListAliases();
            }
            else
            {
                string name = "";
                string command = "";
                
                int i = 0;
                while (i < args.Length && char.IsWhiteSpace(args[i]))
                {
                    i++;
                }
                
                int nameStart = i;
                while (i < args.Length && !char.IsWhiteSpace(args[i]))
                {
                    i++;
                }
                name = args.Substring(nameStart, i - nameStart).Trim();
                
                while (i < args.Length && char.IsWhiteSpace(args[i]))
                {
                    i++;
                }
                
                if (i < args.Length && (args[i] == '\'' || args[i] == '"'))
                {
                    char quoteChar = args[i];
                    i++;
                    int commandStart = i;
                    while (i < args.Length && args[i] != quoteChar)
                    {
                        i++;
                    }
                    if (i < args.Length)
                    {
                        command = args.Substring(commandStart, i - commandStart);
                    }
                    else
                    {
                        command = args.Substring(commandStart);
                    }
                }
                else
                {
                    command = args.Substring(i).Trim();
                }
                
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(command))
                {
                    ShowError("Usage: alias <name> <command>");
                    ShowError("Example: alias ll 'ls -l'");
                    ShowError("Example: alias home \"cd /home\"");
                    ShowError("Example: alias cls clear");
                    return;
                }
                
                Commands.AliasCommand.AddAlias(name, command);
            }
        }

        public void ProcessUnalias(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                ShowError("Usage: unalias <name>");
                ShowError("Example: unalias ll");
                return;
            }
            
            string name = args.Trim();
            Commands.AliasCommand.RemoveAlias(name);
        }

        public void SetDnsServer(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                ShowError("Usage: setdns <ip_address>");
                ShowError("Example: setdns 8.8.8.8");
                return;
            }
            
            string dnsIp = args.Trim();
            
            try
            {
                NetworkConfigManager.Instance.SetDNS(dnsIp);
                ShowSuccess($"DNS server set to: {dnsIp}");
            }
            catch (Exception ex)
            {
                ShowError($"Error setting DNS: {ex.Message}");
            }
        }

        public void SetGateway(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                ShowError("Usage: setgateway <ip_address>");
                ShowError("Example: setgateway 192.168.1.1");
                return;
            }
            
            string gatewayIp = args.Trim();
            
            try
            {
                NetworkConfigManager.Instance.SetGateway(gatewayIp);
                ShowSuccess($"Gateway set to: {gatewayIp}");
            }
            catch (Exception ex)
            {
                ShowError($"Error setting gateway: {ex.Message}");
            }
        }

        public void ShowNetworkConfig()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Network Configuration");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            Console.WriteLine($"Network Device: {Kernel.NetworkDevice?.Name ?? "Not available"}");
            Console.WriteLine($"Local IP: {Kernel.IPAddress}");
            
            string gateway = NetworkConfigManager.Instance.GetGateway();
            Console.WriteLine($"Gateway: {gateway}");
            
            string dns = NetworkConfigManager.Instance.GetDNS();
            Console.WriteLine($"DNS Server: {dns}");
            
            Console.WriteLine();
        }

        public void NsLookup(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                ShowError("Usage: nslookup <domain>");
                ShowError("Example: nslookup github.com");
                return;
            }
            
            string domain = args.Trim();
            
            string dnsStr = NetworkConfigManager.Instance.GetDNS();
            if (string.IsNullOrWhiteSpace(dnsStr))
            {
                ShowError("DNS server not configured. Use 'setdns' command first.");
                return;
            }
            
            Address dns = Address.Parse(dnsStr);
            
            Console.WriteLine("====================================");
            Console.WriteLine("        DNS Lookup");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Domain: {domain}");
            Console.WriteLine($"DNS Server: {dnsStr}");
            Console.WriteLine();
            
            try
            {
                using (var dnsClient = new DnsClient())
                {
                    dnsClient.Connect(dns);
                    Console.WriteLine("Sending DNS query...");
                    
                    dnsClient.SendAsk(domain);
                    
                    Address result = dnsClient.Receive();
                    
                    if (result != null)
                    {
                        ShowSuccess($"Resolved: {domain} -> {result}");
                    }
                    else
                    {
                        ShowError("DNS query failed or timed out");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"DNS lookup error: {ex.Message}");
            }
        }
    }
}