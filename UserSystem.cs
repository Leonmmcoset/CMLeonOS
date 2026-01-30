using System;
using System.IO;

namespace CMLeonOS
{
    public class UserSystem
    {
        private string sysDirectory = @"0:\sys";
        private string adminPasswordFilePath;
        private bool isPasswordSet = false;

        public UserSystem()
        {
            // 确保sys目录存在
            EnsureSysDirectoryExists();
            
            // 设置密码文件路径
            adminPasswordFilePath = Path.Combine(sysDirectory, "admin_password.txt");
            
            CheckPasswordStatus();
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

        private void CheckPasswordStatus()
        {
            try
            {
                isPasswordSet = File.Exists(adminPasswordFilePath);
            }
            catch
            {
                isPasswordSet = false;
            }
        }

        public bool IsPasswordSet
        {
            get { return isPasswordSet; }
        }

        public void SetAdminPassword()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        First Time Setup");
            Console.WriteLine("====================================");
            Console.WriteLine("Please set a password for admin user:");
            
            string password = ReadPassword();
            
            Console.WriteLine("Please confirm your password:");
            string confirmPassword = ReadPassword();
            
            if (password == confirmPassword)
            {
                try
                {
                    // 简单存储密码（实际应用中应使用加密）
                    File.WriteAllText(adminPasswordFilePath, password);
                    Console.WriteLine("Password set successfully!");
                    isPasswordSet = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting password: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Passwords do not match. Please try again.");
                SetAdminPassword();
            }
        }

        public bool Login()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("          System Login");
            Console.WriteLine("====================================");
            Console.WriteLine("Username: admin");
            Console.WriteLine("Password:");
            
            // 检测ALT+Space按键
            bool useFixMode = false;
            ConsoleKeyInfo keyInfo;
            try
            {
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
                else
                {
                    // 正常密码输入
                    string password = "";
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                    
                    while (true)
                    {
                        var passKey = Console.ReadKey(true);
                        if (passKey.Key == ConsoleKey.Enter)
                        {
                            Console.WriteLine();
                            break;
                        }
                        else if (passKey.Key == ConsoleKey.Backspace)
                        {
                            if (password.Length > 0)
                            {
                                password = password.Substring(0, password.Length - 1);
                            }
                        }
                        else
                        {
                            password += passKey.KeyChar;
                            Console.Write("*");
                        }
                    }
                    
                    try
                    {
                        string storedPassword = File.ReadAllText(adminPasswordFilePath);
                        if (password == storedPassword)
                        {
                            Console.WriteLine("Login successful!");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid password. Please try again.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during login: {ex.Message}");
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
                        // 简化退格处理，只修改password字符串
                        // 在Cosmos中，Console.Write("\b \b")可能不被支持
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

        public bool ChangePassword()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Change Password");
            Console.WriteLine("====================================");
            
            // 验证当前密码
            Console.WriteLine("Please enter your current password:");
            string currentPassword = ReadPassword();
            
            try
            {
                string storedPassword = File.ReadAllText(adminPasswordFilePath);
                if (currentPassword != storedPassword)
                {
                    Console.WriteLine("Current password is incorrect.");
                    return false;
                }
                
                // 设置新密码
                Console.WriteLine("Please enter your new password:");
                string newPassword = ReadPassword();
                
                Console.WriteLine("Please confirm your new password:");
                string confirmPassword = ReadPassword();
                
                if (newPassword == confirmPassword)
                {
                    // 存储新密码
                    File.WriteAllText(adminPasswordFilePath, newPassword);
                    Console.WriteLine("Password changed successfully!");
                    return true;
                }
                else
                {
                    Console.WriteLine("New passwords do not match.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }
    }
}