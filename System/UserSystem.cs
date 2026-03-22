// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;
using CMLeonOS.Logger;
using CMLeonOS.Settings;

namespace CMLeonOS
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Hostname { get; set; }
        public List<string> Messages { get; set; }
        public bool Admin
        {
            get { return IsAdmin; }
        }
        public bool LockedOut { get; set; }
        public DateTime LockoutEnd { get; set; }
        
        public bool Authenticate(string password)
        {
            string hashedInputPassword = UserSystem.HashPasswordSha256(password);
            return Password == hashedInputPassword;
        }
    }

    public class UserSystem
    {
        private string sysDirectory = @"0:\system";
        private string userFilePath;

        private static List<User> users;
        public bool fixmode = Kernel.FixMode;
        private static User currentLoggedInUser;
        private static CMLeonOS.Logger.Logger _logger = CMLeonOS.Logger.Logger.Instance;
        private Dictionary<string, int> loginAttempts = new Dictionary<string, int>();
        
        private static bool initialized = false;

        public static List<User> GetUsers()
        {
            return users;
        }

        public static User CurrentLoggedInUser
        {
            get { return currentLoggedInUser; }
        }

        public static void Initialize()
        {
            // 清空用户列表并重新加载
            users = null;
            _logger.Info("UserSystem", "Initializing user system...");
            var tempUserSystem = new UserSystem();
            _logger.Info("UserSystem", "Loading user data...");
            tempUserSystem.LoadUsers();
            _logger.Info("UserSystem", "User data loaded successfully.");
            initialized = true;
            _logger.Info("UserSystem", $"User system initialized. Loaded {users?.Count ?? 0} users.");
        }

        public static void ReloadUsers()
        {
            // 清空用户列表并重新加载
            users = null;
            var tempUserSystem = new UserSystem();
            tempUserSystem.LoadUsers();
            _logger.Info("UserSystem", $"User data reloaded. Total users: {users?.Count ?? 0}");
        }

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

        public static string HashPasswordSha256(string password)
        {
            Sha256 sha256 = new Sha256();

            byte[] passwordBytesUnhashed = Encoding.Unicode.GetBytes(password);
            sha256.AddData(passwordBytesUnhashed, 0, (uint)passwordBytesUnhashed.Length);

            return Convert.ToBase64String(sha256.GetHash());
        }

        private static bool ContainsInvalidChars(string input)
        {
            char[] invalidChars = { '<', '>', ':', '"', '|', '?', '*', '/', '\\', ' ' };
            foreach (char c in invalidChars)
            {
                if (input.Contains(c.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public UserSystem()
        {
            EnsureSysDirectoryExists();
            
            userFilePath = Path.Combine(sysDirectory, "user.dat");
            
            currentLoggedInUser = null;
            
            // 不在构造函数中调用 LoadUsers()，避免重复初始化
            // LoadUsers() 会在 UserSystem.Initialize() 中调用
            
            LoadHostname();
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

        private void LoadHostname()
        {
            string hostnameFilePath = Path.Combine(sysDirectory, "hostname.dat");
            
            try
            {
                if (File.Exists(hostnameFilePath))
                {
                    string hostname = File.ReadAllText(hostnameFilePath);
                    if (!string.IsNullOrWhiteSpace(hostname))
                    {
                        if (users.Count > 0)
                        {
                            users[0].Hostname = hostname;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveHostname()
        {
            string hostnameFilePath = Path.Combine(sysDirectory, "hostname.dat");
            
            try
            {
                if (users.Count > 0)
                {
                    string hostname = users[0].Hostname;
                    File.WriteAllText(hostnameFilePath, hostname);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving hostname: {ex.Message}");
            }
        }

        public string GetHostname()
        {
            if (users.Count > 0)
            {
                return users[0].Hostname;
            }
            return "Not set";
        }

        public void SetHostname(string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname))
            {
                ShowError("Hostname cannot be empty");
                return;
            }
            
            if (users.Count > 0)
            {
                users[0].Hostname = hostname;
                SaveHostname();
                ShowSuccess($"Hostname set to: {hostname}");
            }
            else
            {
                ShowError("No users found. Please create a user first.");
            }
        }

        public void ProcessHostnameCommand(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ShowError("Usage: hostname <new_hostname>");
                return;
            }
            
            SetHostname(args);
        }

        private void LoadUsers()
        {
            _logger.Info("UserSystem", "LoadUsers() called");
            _logger.Info("UserSystem", $"  - userFilePath: {userFilePath}");
            _logger.Info("UserSystem", $"  - File.Exists: {File.Exists(userFilePath)}");
            _logger.Info("UserSystem", $"  - users is null: {users == null}");
            _logger.Info("UserSystem", $"  - users.Count: {users?.Count ?? 0}");

            try
            {
                if (File.Exists(userFilePath))
                {
                    _logger.Info("UserSystem", "  - Reading user file...");
                    string[] lines = File.ReadAllLines(userFilePath);
                    _logger.Info("UserSystem", $"  - Total lines in file: {lines.Length}");
                    
                    // 只在第一次加载时创建列表，后续加载不清空
                    if (users == null)
                    {
                        users = new List<User>();
                        _logger.Info("UserSystem", "  - Created new users list");
                    }
                    
                    int userCount = 0;
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
                                IsAdmin = parts.Length >= 3 && parts[2].Trim().ToLower() == "admin",
                                Hostname = parts.Length >= 4 ? parts[3].Trim() : ""
                            };
                            users.Add(user);
                            userCount++;
                            _logger.Info("UserSystem", $"  - Loaded user: {user.Username}, Admin: {user.Admin}");
                        }
                    }
                    
                    _logger.Info("UserSystem", $"  - Total users loaded: {userCount}");
                    
                    // Note: Passwords are stored as SHA256 hashes in the file
                    // When comparing passwords during login, hash the input password first
                }
                else
                {
                    // 只在第一次加载时创建列表
                    if (users == null)
                    {
                        users = new List<User>();
                        _logger.Info("UserSystem", "  - Created empty users list (file not found)");
                    }
                    else
                    {
                        _logger.Info("UserSystem", $"  - Using existing users list (count: {users.Count})");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserSystem", $"Error loading users: {ex.Message}");
                // 只在第一次加载时创建列表
                if (users == null)
                {
                    users = new List<User>();
                }
            }
        }

        private void SaveUsers()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (User user in users)
                {
                    string passwordToSave;
                    
                    if (IsPasswordAlreadyHashed(user.Password))
                    {
                        passwordToSave = user.Password;
                    }
                    else
                    {
                        passwordToSave = HashPasswordSha256(user.Password);
                    }
                    
                    string line = $"{user.Username}|{passwordToSave}|{(user.IsAdmin ? "admin" : "user")}|{user.Hostname}";
                    lines.Add(line);
                }
                File.WriteAllLines(userFilePath, lines.ToArray());
            }
            catch (Exception ex)
            {
                ShowError($"Error saving users: {ex.Message}");
            }
        }

        private bool IsPasswordAlreadyHashed(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            
            string trimmedPassword = password.Trim();
            
            if (trimmedPassword.Length < 32)
            {
                return false;
            }
            
            foreach (char c in trimmedPassword)
            {
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '+' || c == '/' || c == '='))
                {
                    return false;
                }
            }
            
            return true;
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

        public string CurrentUsername
        {
            get
            {
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.Username;
                }
                return "Not logged in";
            }
        }

        public bool CurrentUserIsAdmin
        {
            get
            {
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.IsAdmin;
                }
                return false;
            }
        }

        public static void SetCurrentLoggedInUser(User user)
        {
            currentLoggedInUser = user;
        }

        public void FirstTimeSetup()
        {
            CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
            global::System.Console.Clear();

            var termsBox = new CMLeonOS.UI.Window(new CMLeonOS.UI.Rect(5, 5, 70, 18), "User Terms and Conditions", () => { }, true);
                termsBox.Render();

                CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Gray, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(7, 7);
                global::System.Console.WriteLine("1. This operating system is provided as-is without warranty");
                global::System.Console.SetCursorPosition(7, 8);
                global::System.Console.WriteLine("2. You are responsible for your data and backups");
                global::System.Console.SetCursorPosition(7, 9);
                global::System.Console.WriteLine("3. Unauthorized access attempts may be logged");
                global::System.Console.SetCursorPosition(7, 10);
                global::System.Console.WriteLine("4. System administrators have full access to all data");
                global::System.Console.SetCursorPosition(7, 11);
                global::System.Console.WriteLine("5. By using this system, you agree to these terms");
                global::System.Console.SetCursorPosition(7, 12);
                global::System.Console.WriteLine("6. Data privacy: Your personal data is stored locally");
                global::System.Console.SetCursorPosition(7, 13);
                global::System.Console.WriteLine("7. System updates may be installed automatically");
                global::System.Console.SetCursorPosition(7, 14);
                global::System.Console.WriteLine("8. No liability for data loss or corruption");
                global::System.Console.SetCursorPosition(7, 15);
                global::System.Console.WriteLine("9. Support available at: https://lbbs.ecuil.com/#/thread/category/10");
                global::System.Console.SetCursorPosition(7, 16);
                global::System.Console.WriteLine("10. This license is for personal use only");
                global::System.Console.SetCursorPosition(7, 17);
                global::System.Console.WriteLine("11. Use of this OS requires recognition of the one-China principle");

                bool termsAccepted = false;
                while (!termsAccepted)
                {
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(5, 24);
                    global::System.Console.Write("Do you accept the User Terms? (yes/no): ");
                    string response = global::System.Console.ReadLine()?.ToLower();

                    if (response == "yes" || response == "y")
                    {
                        termsAccepted = true;
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Green, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(5, 24);
                        global::System.Console.Write("Terms accepted.                          ");
                    }
                    else if (response == "no" || response == "n")
                    {
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(5, 24);
                        global::System.Console.Write("You must accept the User Terms to continue.");
                        global::System.Threading.Thread.Sleep(2000);
                        Sys.Power.Reboot();
                    }
                    else
                    {
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(5, 24);
                        global::System.Console.Write("Invalid response. Please enter 'yes' or 'no'.");
                    }
                }

                global::System.Console.Clear();

                var setupBox = new CMLeonOS.UI.Window(new CMLeonOS.UI.Rect(5, 5, 70, 12), "Admin Account Setup", () => { }, true);
                setupBox.Render();

                CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(7, 7);
                global::System.Console.Write("Username: ");
                string username = global::System.Console.ReadLine();

                while (string.IsNullOrWhiteSpace(username))
                {
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 24);
                    global::System.Console.Write("Username cannot be empty.                   ");
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 7);
                    global::System.Console.Write("Username: ");
                    username = global::System.Console.ReadLine();
                }

                while (ContainsInvalidChars(username))
                {
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 24);
                    global::System.Console.Write("Username contains invalid characters: < > : \" | ? * / \\ space");
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 7);
                    global::System.Console.Write("Username: ");
                    username = global::System.Console.ReadLine();
                }

                CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(7, 8);
                global::System.Console.Write("Password: ");
                string password = ReadPassword();

                CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(7, 9);
                global::System.Console.Write("Confirm Password: ");
                string confirmPassword = ReadPassword();

                while (password != confirmPassword)
                {
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 24);
                    global::System.Console.Write("Passwords do not match. Please try again.      ");
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 7);
                    global::System.Console.Write("Username: ");
                    username = global::System.Console.ReadLine();

                    while (string.IsNullOrWhiteSpace(username))
                    {
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(7, 24);
                        global::System.Console.Write("Username cannot be empty.                   ");
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(7, 7);
                        global::System.Console.Write("Username: ");
                        username = global::System.Console.ReadLine();
                    }

                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 8);
                    global::System.Console.Write("Password: ");
                    password = ReadPassword();

                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 9);
                    global::System.Console.Write("Confirm Password: ");
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
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Green, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 24);
                    global::System.Console.Write("Admin user created successfully!            ");
                    global::System.Console.Clear();

                    var hostnameBox = new CMLeonOS.UI.Window(new CMLeonOS.UI.Rect(5, 5, 70, 8), "Hostname Setup", () => { }, true);
                    hostnameBox.Render();

                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 7);
                    global::System.Console.Write("Hostname: ");
                    string hostname = global::System.Console.ReadLine();

                    while (string.IsNullOrWhiteSpace(hostname))
                    {
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(7, 24);
                        global::System.Console.Write("Hostname cannot be empty.                   ");
                        CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                        global::System.Console.SetCursorPosition(7, 7);
                        global::System.Console.Write("Hostname: ");
                        hostname = global::System.Console.ReadLine();
                    }

                    if (users.Count > 0)
                    {
                        users[0].Hostname = hostname;
                        SaveUsers();
                    }

                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Green, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(7, 24);
                    global::System.Console.Write("Hostname set successfully!                     ");
                    global::System.Threading.Thread.Sleep(2000);

                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
                    global::System.Console.Clear();
                    global::System.Console.SetCursorPosition(30, 12);
                    global::System.Console.Write("Setup completed!");
                    global::System.Console.SetCursorPosition(20, 13);
                    global::System.Console.Write("System will restart in 3 seconds...");
                    global::System.Threading.Thread.Sleep(3000);

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
                _logger.Info("UserSystem", $"Creating user folder for {username}...");

                // 在user文件夹下创建用户文件夹
                string userFolderPath = Path.Combine(@"0:\user", username);
                
                // 检查用户文件夹是否存在
                if (!Directory.Exists(userFolderPath))
                {
                    Directory.CreateDirectory(userFolderPath);
                    _logger.Info("UserSystem", $"Created user folder for {username}.");
                }
                else 
                {
                    _logger.Info("UserSystem", $"User folder for {username} already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserSystem", $"Error creating user folder for {username}: {ex.Message}");
            }
        }

        public bool Login()
        {
            CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
            global::System.Console.Clear();

            var loginBox = new CMLeonOS.UI.Window(new CMLeonOS.UI.Rect(15, 8, 50, 15), "Login", () => { }, true);
            loginBox.Render();

            CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
            global::System.Console.SetCursorPosition(17, 10);
            global::System.Console.Write("Select User:");

            var userMenu = new CMLeonOS.UI.Menu(new CMLeonOS.UI.Rect(17, 11, 46, 8));
            foreach (User user in users)
            {
                userMenu.Items.Add(new CMLeonOS.UI.MenuItem(user.Username));
            }
            userMenu.Render();

            User selectedUser = null;
            while (selectedUser == null)
            {
                var key = global::System.Console.ReadKey(true);
                
                if (key.Key == global::System.ConsoleKey.Enter)
                {
                    if (userMenu.SelectedIndex >= 0 && userMenu.SelectedIndex < users.Count)
                    {
                        selectedUser = users[userMenu.SelectedIndex];
                    }
                }
                else if (key.Key == global::System.ConsoleKey.Escape)
                {
                    return false;
                }
                else if (userMenu.HandleKey(key))
                {
                    userMenu.Render();
                }
            }

            CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.White, global::System.ConsoleColor.Black);
            global::System.Console.SetCursorPosition(17, 20);
            global::System.Console.Write("Password: ");
            string password = ReadPassword();

            string hashedInputPassword = HashPasswordSha256(password);

            if (selectedUser.Password != hashedInputPassword)
            {
                string usernameLower = selectedUser.Username.ToLower();
                
                if (!loginAttempts.ContainsKey(usernameLower))
                {
                    loginAttempts[usernameLower] = 0;
                }
                
                loginAttempts[usernameLower]++;
                int attempts = loginAttempts[usernameLower];
                int maxAttempts = Settings.SettingsManager.MaxLoginAttempts;
                
                if (attempts >= maxAttempts)
                {
                    CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                    global::System.Console.SetCursorPosition(17, 24);
                    global::System.Console.Write($"Maximum login attempts ({maxAttempts}) reached.  ");
                    global::System.Console.SetCursorPosition(17, 25);
                    global::System.Console.Write("Please restart to enter password again. ");
                    global::System.Threading.Thread.Sleep(3000);
                    Sys.Power.Reboot();
                }
                
                CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Red, global::System.ConsoleColor.Black);
                global::System.Console.SetCursorPosition(17, 24);
                global::System.Console.Write($"Invalid password. Attempts: {attempts}/{maxAttempts}  ");
                global::System.Threading.Thread.Sleep(1000);
                return false;
            }

            loginAttempts.Remove(selectedUser.Username.ToLower());

            CMLeonOS.UI.TUIHelper.SetColors(global::System.ConsoleColor.Green, global::System.ConsoleColor.Black);
            global::System.Console.SetCursorPosition(17, 24);
            global::System.Console.Write("Login successful!                           ");
            global::System.Threading.Thread.Sleep(1500);
            global::System.Console.ResetColor();
            global::System.Console.Clear();

            global::System.Console.Beep();

            currentLoggedInUser = selectedUser;
            CreateUserFolder(selectedUser.Username);

            return true;
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
                    Password = password,
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
