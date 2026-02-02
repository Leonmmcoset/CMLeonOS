using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace CMLeonOS
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UserSystem
    {
        private string sysDirectory = @"0:\system";
        private string userFilePath;
        private List<User> users;
        public bool fixmode = Kernel.FixMode;
        private User currentLoggedInUser;

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

        internal static string HashPasswordSha256(string password)
        {
            Sha256 sha256 = new Sha256();

            byte[] passwordBytesUnhashed = Encoding.Unicode.GetBytes(password);
            sha256.AddData(passwordBytesUnhashed, 0, (uint)passwordBytesUnhashed.Length);

            return Convert.ToBase64String(sha256.GetHash());
        }

        public UserSystem()
        {
            EnsureSysDirectoryExists();
            
            // 设置用户文件路径
            userFilePath = Path.Combine(sysDirectory, "user.dat");
            
            // 初始化当前登录用户
            currentLoggedInUser = null;
            
            // 加载用户数据
            LoadUsers();
        }

        private void EnsureSysDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(sysDirectory))
                {
                    Directory.CreateDirectory(sysDirectory);
                }
            }
            catch
            {
                // 忽略目录创建错误
            }
        }

        private void LoadUsers()
        {
            try
            {
                if (File.Exists(userFilePath))
                {
                    string[] lines = File.ReadAllLines(userFilePath);
                    users = new List<User>();
                    
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        {
                            continue;
                        }
                        
                        string[] parts = line.Split('|');
                        if (parts.Length >= 2)
                        {
                            User user = new User
                            {
                                Username = parts[0].Trim(),
                                Password = parts[1].Trim(),
                                IsAdmin = parts.Length >= 3 && parts[2].Trim().ToLower() == "admin"
                            };
                            users.Add(user);
                        }
                    }
                    
                    // Note: Passwords are stored as SHA256 hashes in the file
                    // When comparing passwords during login, hash the input password first
                }
                else
                {
                    users = new List<User>();
                }
            }
            catch
            {
                users = new List<User>();
            }
        }

        private void SaveUsers()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (User user in users)
                {
                    // 使用SHA256加密密码
                    string hashedPassword = HashPasswordSha256(user.Password);
                    string line = $"{user.Username}|{hashedPassword}|{(user.IsAdmin ? "admin" : "user")}";
                    lines.Add(line);
                }
                File.WriteAllLines(userFilePath, lines.ToArray());
            }
            catch (Exception ex)
            {
                ShowError($"Error saving users: {ex.Message}");
            }
        }

        public bool HasUsers
        {
            get { return users.Count > 0; }
        }

        public bool IsAdminSet
        {
            get
            {
                foreach (User user in users)
                {
                    if (user.IsAdmin)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void FirstTimeSetup()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        First Time Setup");
            Console.WriteLine("====================================");
            Console.WriteLine("Please set admin username and password:");
            
            Console.Write("Username: ");
            string username = Console.ReadLine();
            
            while (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Username cannot be empty.");
                Console.Write("Username: ");
                username = Console.ReadLine();
            }
            
            Console.WriteLine("Password: ");
            string password = ReadPassword();
            
            Console.WriteLine("Please confirm your password:");
            string confirmPassword = ReadPassword();
            
            while (password != confirmPassword)
            {
                ShowError("Passwords do not match. Please try again.");
                
                Console.Write("Username: ");
                username = Console.ReadLine();
                
                while (string.IsNullOrWhiteSpace(username))
                {
                    ShowError("Username cannot be empty.");
                    Console.Write("Username: ");
                    username = Console.ReadLine();
                }
                
                Console.WriteLine("Password: ");
                password = ReadPassword();
                
                Console.WriteLine("Please confirm your password:");
                confirmPassword = ReadPassword();
            }
            
            try
            {
                User adminUser = new User
                {
                    Username = username,
                    Password = password,
                    IsAdmin = true
                };
                users.Add(adminUser);
                SaveUsers();
                ShowSuccess("Admin user created successfully!");
                
                Console.WriteLine();
                Console.WriteLine("System will restart in 3 seconds...");
                Console.WriteLine("Please wait...");
                Console.WriteLine();
                
                for (int i = 3; i > 0; i--)
                {
                    Console.Write($"\rRestarting in {i} seconds...   ");
                    Thread.Sleep(1000);
                }
                
                Console.WriteLine("\rRestarting now!");
                Sys.Power.Reboot();
            }
            catch (Exception ex)
            {
                ShowError($"Error creating admin user: {ex.Message}");
            }
        }

        private void CreateUserFolder(string username)
        {
            try
            {
                Console.WriteLine($"Creating user folder for {username}...");

                // 在user文件夹下创建用户文件夹
                string userFolderPath = Path.Combine(@"0:\user", username);
                
                // 检查用户文件夹是否存在
                if (!Directory.Exists(userFolderPath))
                {
                    Directory.CreateDirectory(userFolderPath);
                    Console.WriteLine($"Created user folder for {username}.");
                }
                else 
                {
                    Console.WriteLine($"User folder for {username} already exists.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error creating user folder: {ex.Message}");
            }
        }

        public bool Login()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("          System Login");
            Console.WriteLine("====================================");
            // Console.ReadKey(true);
            
            // 检测ALT+Space按键
            bool useFixMode = false;
            ConsoleKeyInfo keyInfo;
            try
            {
                if (fixmode == true) {
                    keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Spacebar && (keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                    {
                        // 检测到ALT+Space，进入修复模式
                        useFixMode = true;
                        Console.WriteLine();
                        Console.WriteLine("Fix Mode Activated");
                        Console.Write("Enter fix code: ");
                        
                        string fixCode = "";
                        while (true)
                        {
                            var codeKey = Console.ReadKey(true);
                            if (codeKey.Key == ConsoleKey.Enter)
                            {
                                Console.WriteLine();
                                break;
                            }
                            else if (codeKey.Key == ConsoleKey.Backspace)
                            {
                                if (fixCode.Length > 0)
                                {
                                    fixCode = fixCode.Substring(0, fixCode.Length - 1);
                                }
                            }
                            else
                            {
                                fixCode += codeKey.KeyChar;
                                Console.Write(codeKey.KeyChar);
                            }
                        }
                        
                        if (fixCode == "FixMyComputer")
                        {
                            Console.WriteLine("Fix mode enabled!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid fix code. Exiting fix mode.");
                            useFixMode = false;
                        }
                    }
                }
                else
                {
                    // 正常登录流程
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        ShowError("Username cannot be empty.");
                        return false;
                    }
                    
                    Console.Write("Password: ");
                    string password = ReadPassword();
                    
                    // 查找用户
                    User foundUser = null;
                    foreach (User user in users)
                    {
                        if (user.Username.ToLower() == username.ToLower())
                        {
                            foundUser = user;
                            break;
                        }
                    }
                    
                    if (foundUser == null)
                    {
                        ShowError("User not found.");
                        return false;
                    }
                    
                    // 使用SHA256加密输入的密码后比较
                    string hashedInputPassword = HashPasswordSha256(password);
                    // Console.WriteLine($"Hashed Input Password: {hashedInputPassword}");
                    // Console.WriteLine($"Stored Password: {foundUser.Password}");
                    
                    if (foundUser.Password != hashedInputPassword)
                    {
                        ShowError("Invalid password.");
                        return false;
                    }
                    
                    ShowSuccess("Login successful!");
                    Console.Beep();
                    
                    // 设置当前登录用户
                    currentLoggedInUser = foundUser;
                    
                    // 创建用户文件夹
                    CreateUserFolder(foundUser.Username);
                    
                    return true;
                }
            }
            catch
            {
                // 如果读取按键失败，使用普通登录
                ShowError("Error reading key input. Using normal login.");
                return false;
            }
            
            // 如果使用了修复模式，返回true
            if (useFixMode)
            {
                return true;
            }
            
            return false;
        }

        public bool AddUser(string args, bool isAdmin)
        {
            Console.WriteLine("====================================");
            Console.WriteLine($"        Add {(isAdmin ? "Admin" : "User")}");
            Console.WriteLine("====================================");
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                ShowError("Error: Please specify username and password");
                ShowError($"Usage: user add {(isAdmin ? "admin" : "user")} <username> <password>");
                return false;
            }
            
            string username = parts[0];
            string password = parts[1];
            
            // 检查用户名是否已存在
            foreach (User user in users)
            {
                if (user.Username.ToLower() == username.ToLower())
                {
                    ShowError($"Error: User '{username}' already exists.");
                    return false;
                }
            }
            
            try
            {
                User newUser = new User
                {
                    Username = username,
                    Password = HashPasswordSha256(password),
                    IsAdmin = isAdmin
                };
                users.Add(newUser);
                SaveUsers();

                // 创建用户文件夹
                CreateUserFolder(username);

                ShowSuccess($"{(isAdmin ? "Admin" : "User")} '{username}' created successfully!");
                ShowSuccess("You shall restart the system to apply the changes.");
                
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Error adding user: {ex.Message}");
                return false;
            }
        }

        public bool DeleteUser(string username)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Delete User");
            Console.WriteLine("====================================");
            
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Error: Please specify username");
                ShowError("Usage: user delete <username>");
                return false;
            }
            
            // 查找用户
            User foundUser = null;
            foreach (User user in users)
            {
                if (user.Username.ToLower() == username.ToLower())
                {
                    foundUser = user;
                    break;
                }
            }
            
            if (foundUser == null)
            {
                ShowError($"Error: User '{username}' not found.");
                return false;
            }
            
            // 检查是否是最后一个管理员
            int adminCount = 0;
            foreach (User user in users)
            {
                if (user.IsAdmin)
                {
                    adminCount++;
                }
            }
            
            if (foundUser.IsAdmin && adminCount <= 1)
            {
                ShowError("Error: Cannot delete the last admin user.");
                return false;
            }
            
            try
            {
                users.Remove(foundUser);
                SaveUsers();
                ShowSuccess($"User '{username}' deleted successfully!");
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public void ListUsers()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        User List");
            Console.WriteLine("====================================");
            
            if (users.Count == 0)
            {
                Console.WriteLine("No users found.");
                return;
            }
            
            Console.WriteLine();
            foreach (User user in users)
            {
                string userType = user.IsAdmin ? "[ADMIN]" : "[USER]";
                Console.WriteLine($"{userType} {user.Username}");
            }
        }

        public bool ChangePassword()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Change Password");
            Console.WriteLine("====================================");
            
            Console.Write("Please enter your current password: ");
            string currentPassword = ReadPassword();
            
            // 检查是否有用户登录
            if (currentLoggedInUser == null)
            {
                ShowError("Error: No user logged in.");
                return false;
            }
            
            Console.Write("Please enter your new password: ");
            string newPassword = ReadPassword();
            
            Console.WriteLine("Please confirm your new password: ");
            string confirmPassword = ReadPassword();
            
            if (newPassword == confirmPassword)
            {
                try
                {
                    currentLoggedInUser.Password = newPassword;
                    SaveUsers();
                    ShowSuccess("Password changed successfully!");
                    return true;
                }
                catch (Exception ex)
                {
                    ShowError($"Error changing password: {ex.Message}");
                    return false;
                }
            }
            else
            {
                ShowError("New passwords do not match.");
                return false;
            }
        }

        private string ReadPassword()
        {
            string password = "";
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        int cursorLeft = Console.CursorLeft;
                        int cursorTop = Console.CursorTop;
                        if (cursorLeft > 0)
                        {
                            Console.SetCursorPosition(cursorLeft - 1, cursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(cursorLeft - 1, cursorTop);
                        }
                    }
                }
                else
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }
    }
}
