using Cosmos.System.Graphics;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.Logger;

using System;
using System.Drawing;

namespace CMLeonOS.Gui.ShellComponents
{
    internal class Lock : Process
    {
        internal Lock() : base("Lock", ProcessType.Application) { }

        AppWindow window;

        TextBox usernameBox;

        TextBox passwordBox;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        Sound.SoundService soundService = ProcessManager.GetProcess<Sound.SoundService>();

        private static class Images
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Lock.Key.bmp")]
            private static byte[] _iconBytes_Key;
            internal static Bitmap Icon_Key = new Bitmap(_iconBytes_Key);
        }

        private const int width = 352;
        private const int height = 128;
        private const int padding = 12;

        private double shakiness = 0;

        private void RenderBackground()
        {
            window.Clear(Color.LightGray);

            window.DrawImageAlpha(Images.Icon_Key, padding, padding);

            window.DrawString("Enter your username and password,\nthen press Enter to log on", Color.Black, (int)(padding + Images.Icon_Key.Width + padding), padding);
        }

        private void ShowError(string text)
        {
            MessageBox messageBox = new MessageBox(this, "Logon Failed", text);
            messageBox.Show();
        }

        private void Shake()
        {
            shakiness = 24;
        }

        private void LogOn()
        {
            if (usernameBox.Text.Trim() == string.Empty || passwordBox.Text.Trim() == string.Empty)
            {
                return;
            }

            string username = usernameBox.Text.Trim();
            string password = passwordBox.Text.Trim();

            Logger.Logger.Instance.Info("Lock", $"Attempting login for user: {username}");

            var users = UserSystem.GetUsers();
            Logger.Logger.Instance.Info("Lock", $"UserSystem.GetUsers() returned: {(users == null ? "null" : users.Count.ToString())}");

            if (users == null || users.Count == 0)
            {
                Logger.Logger.Instance.Error("Lock", "User list is null or empty!");
                ShowError("User system error. Please restart.");
                Shake();
                return;
            }

            User foundUser = null;
            foreach (User user in users)
            {
                Logger.Logger.Instance.Info("Lock", $"Checking user: {user.Username} (lower: {user.Username.ToLower()})");
                if (user.Username.ToLower() == username.ToLower())
                {
                    foundUser = user;
                    Logger.Logger.Instance.Info("Lock", $"User matched: {foundUser.Username}");
                    break;
                }
            }

            if (foundUser == null)
            {
                Logger.Logger.Instance.Warning("Lock", $"User not found: {username}");
                Shake();
                return;
            }

            Logger.Logger.Instance.Info("Lock", $"User found: {foundUser.Username}, Admin: {foundUser.Admin}");

            string hashedInputPassword = UserSystem.HashPasswordSha256(password);
            Logger.Logger.Instance.Info("Lock", $"Password hash: {hashedInputPassword}");
            Logger.Logger.Instance.Info("Lock", $"Stored password hash: {foundUser.Password}");

            if (foundUser.Password != hashedInputPassword)
            {
                Logger.Logger.Instance.Warning("Lock", $"Authentication failed for user: {foundUser.Username}");
                passwordBox.Text = string.Empty;

                if (foundUser.LockedOut)
                {
                    TimeSpan remaining = foundUser.LockoutEnd - DateTime.Now;
                    if (remaining.Minutes > 0)
                    {
                        ShowError($"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                    }
                    else
                    {
                        ShowError($"Try again in {remaining.Seconds}s.");
                    }
                }

                wm.Update(window);
                soundService.PlaySystemSound(Sound.SystemSound.Alert);
                Shake();
                return;
            }

            Logger.Logger.Instance.Info("Lock", $"Authentication successful for user: {foundUser.Username}");

            TryStop();
            UserSystem.SetCurrentLoggedInUser(foundUser);
            ProcessManager.AddProcess(wm, new ShellComponents.Taskbar()).Start();
            ProcessManager.AddProcess(wm, new ShellComponents.Dock.Dock()).Start();
            soundService.PlaySystemSound(Sound.SystemSound.Login);

            Logger.Logger.Instance.Info("Lock", $"{foundUser.Username} logged on to the GUI.");
        }

        private void LogOnClick(int x, int y)
        {
            LogOn();
        }

        #region Process
        public override void Start()
        {
            base.Start();
            
            var users = UserSystem.GetUsers();
            Logger.Logger.Instance.Info("Lock", $"Lock started. Total users: {users?.Count ?? 0}");
            if (users != null)
            {
                foreach (var user in users)
                {
                    Logger.Logger.Instance.Info("Lock", $"  - User: {user.Username}, Admin: {user.Admin}");
                }
            }
            
            window = new AppWindow(this, (int)(wm.ScreenWidth / 2 - width / 2), (int)(wm.ScreenHeight / 2 - height / 2), width, height);
            window.Title = "CMLeonOS Logon";
            window.Icon = Images.Icon_Key;
            window.CanMove = false;
            window.CanClose = false;
            wm.AddWindow(window);

            RenderBackground();

            int boxesStartY = (int)(padding + Images.Icon_Key.Height + padding);

            usernameBox = new TextBox(window, padding, boxesStartY, 160, 20);
            usernameBox.PlaceholderText = "Username";
            usernameBox.Submitted = LogOn;
            wm.AddWindow(usernameBox);

            passwordBox = new TextBox(window, padding, boxesStartY + padding + 20, 160, 20);
            passwordBox.Shield = true;
            passwordBox.PlaceholderText = "Password";
            passwordBox.Submitted = LogOn;
            wm.AddWindow(passwordBox);

            wm.Update(window);
        }

        public override void Run()
        {
            int oldX = window.X;
            int newX = (int)((wm.ScreenWidth / 2) - (width / 2) + (Math.Sin(shakiness) * 8));
            if (oldX != newX)
            {
                window.Move(newX, window.Y);
            }
            shakiness /= 1.1;
        }

        public override void Stop()
        {
            base.Stop();
            wm.RemoveWindow(window);
        }
        #endregion
    }
}
