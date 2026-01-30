using System;
using System.IO;

namespace CMLeonOS
{
    public class UserSystem
    {
        private string adminPasswordFilePath = @"0:\admin_password.txt";
        private bool isPasswordSet = false;

        public UserSystem()
        {
            CheckPasswordStatus();
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
            
            string password = ReadPassword();
            
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