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

        Table userTable;
        
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
        private const int height = 200;
        private const int padding = 12;

        private double shakiness = 0;

        private void RenderBackground()
        {
            window.Clear(Color.LightGray);

            window.DrawImageAlpha(Images.Icon_Key, padding, padding);

            window.DrawString("Select a user and enter password,\nthen press Enter to log on", Color.Black, (int)(padding + Images.Icon_Key.Width + padding), padding);
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
            if (userTable.SelectedCellIndex < 0 || passwordBox.Text.Trim() == string.Empty)
            {
                return;
            }

            if (userTable.SelectedCellIndex >= UserSystem.GetUsers().Count)
            {
                return;
            }

            var users = UserSystem.GetUsers();
            User selectedUser = users[userTable.SelectedCellIndex];
            string password = passwordBox.Text.Trim();

            Logger.Logger.Instance.Info("Lock", $"Attempting login for user: {selectedUser.Username}");
            Logger.Logger.Instance.Info("Lock", $"UserSystem.GetUsers() returned: {(users == null ? "null" : users.Count.ToString())}");

            if (users == null || users.Count == 0)
            {
                Logger.Logger.Instance.Error("Lock", "User list is null or empty!");
                ShowError("User system error. Please restart.");
                Shake();
                return;
            }

            Logger.Logger.Instance.Info("Lock", $"User found: {selectedUser.Username}, Admin: {selectedUser.Admin}");
            string hashedInputPassword = UserSystem.HashPasswordSha256(password);

            if (selectedUser.Password != hashedInputPassword)
            {
                Logger.Logger.Instance.Warning("Lock", $"Authentication failed for user: {selectedUser.Username}");
                passwordBox.Text = string.Empty;

                if (selectedUser.LockedOut)
                {
                    TimeSpan remaining = selectedUser.LockoutEnd - DateTime.Now;
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

            Logger.Logger.Instance.Info("Lock", $"Authentication successful for user: {selectedUser.Username}");
            TryStop();
            UserSystem.SetCurrentLoggedInUser(selectedUser);
            ProcessManager.AddProcess(wm, new ShellComponents.Taskbar()).Start();
            ProcessManager.AddProcess(wm, new ShellComponents.Dock.Dock()).Start();
            soundService.PlaySystemSound(Sound.SystemSound.Login);

            Logger.Logger.Instance.Info("Lock", $"{selectedUser.Username} logged on to GUI.");
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
            window.Title = "CMLeonOS Login";
            window.Icon = Images.Icon_Key;
            window.CanMove = false;
            window.CanClose = false;
            wm.AddWindow(window);

            RenderBackground();

            int contentHeight = 110;
            int contentWidth = 160;
            int startX = (width - contentWidth) / 2;
            int startY = (height - contentHeight) / 2;
            
            userTable = new Table(window, startX, startY, contentWidth, 80);
            userTable.CellHeight = 25;
            userTable.Background = Color.White;
            userTable.Foreground = Color.Black;
            userTable.Border = Color.Gray;
            userTable.SelectedBackground = Color.FromArgb(221, 246, 255);
            userTable.SelectedForeground = Color.Black;
            userTable.SelectedBorder = Color.FromArgb(126, 205, 234);
            userTable.AllowSelection = true;
            userTable.AllowDeselection = false;
            
            foreach (var user in users)
            {
                var cell = new TableCell(user.Username);
                userTable.Cells.Add(cell);
            }
            
            if (userTable.Cells.Count > 0)
            {
                userTable.SelectedCellIndex = 0;
            }
            
            wm.AddWindow(userTable);

            passwordBox = new TextBox(window, startX, startY + 90, contentWidth, 20);
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
