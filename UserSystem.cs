using System;
using System.Collections.Generic;
using System.IO;

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
                    string line = $"{user.Username}|{user.Password}|{(user.IsAdmin ? "admin" : "user")}";
                    lines.Add(line);
                }
                File.WriteAllLines(userFilePath, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
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
                Console.WriteLine("Username cannot be empty.");
                Console.Write("Username: ");
                username = Console.ReadLine();
            }
            
            Console.WriteLine("Password: ");
            string password = ReadPassword();
            
            Console.WriteLine("Please confirm your password:");
            string confirmPassword = ReadPassword();
            
            while (password != confirmPassword)
            {
                Console.WriteLine("Passwords do not match. Please try again.");
                
                Console.Write("Username: ");
                username = Console.ReadLine();
                
                while (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username cannot be empty.");
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
                Console.WriteLine("Admin user created successfully!");
                
                // 创建用户文件夹
                CreateUserFolder(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating admin user: {ex.Message}");
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
                Console.WriteLine($"Error creating user folder: {ex.Message}");
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
                        Console.WriteLine("Username cannot be empty.");
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
                        Console.WriteLine("User not found.");
                        return false;
                    }
                    
                    if (foundUser.Password == password)
                    {
                        Console.WriteLine("Login successful!");
                        Console.Beep();
                        
                        // 设置当前登录用户
                        currentLoggedInUser = foundUser;
                        
                        // 创建用户文件夹
                        CreateUserFolder(foundUser.Username);
                        
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid password. Please try again.");
                        return false;
                    }
                }
            }
            catch
            {
                // 如果读取按键失败，使用普通登录
                Console.WriteLine("Error reading key input. Using normal login.");
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
                Console.WriteLine("Error: Please specify username and password");
                Console.WriteLine($"Usage: user add {(isAdmin ? "admin" : "user")} <username> <password>");
                return false;
            }
            
            string username = parts[0];
            string password = parts[1];
            
            // 检查用户名是否已存在
            foreach (User user in users)
            {
                if (user.Username.ToLower() == username.ToLower())
                {
                    Console.WriteLine($"Error: User '{username}' already exists.");
                    return false;
                }
            }
            
            try
            {
                User newUser = new User
                {
                    Username = username,
                    Password = password,
                    IsAdmin = isAdmin
                };
                users.Add(newUser);
                SaveUsers();

                // 创建用户文件夹
                CreateUserFolder(username);

                Console.WriteLine($"{(isAdmin ? "Admin" : "User")} '{username}' created successfully!");
                Console.WriteLine("You shall restart the system to apply the changes.");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
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
                Console.WriteLine("Error: Please specify username");
                Console.WriteLine("Usage: user delete <username>");
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
                Console.WriteLine($"Error: User '{username}' not found.");
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
                Console.WriteLine("Error: Cannot delete the last admin user.");
                return false;
            }
            
            try
            {
                users.Remove(foundUser);
                SaveUsers();
                Console.WriteLine($"User '{username}' deleted successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
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
                Console.WriteLine("Error: No user logged in.");
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
                    Console.WriteLine("Password changed successfully!");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error changing password: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("New passwords do not match.");
                return false;
            }
        }

        public void Logout()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        User Logout");
            Console.WriteLine("====================================");
            Console.WriteLine("Logging out...");
            Console.WriteLine("Logout successful!");
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
