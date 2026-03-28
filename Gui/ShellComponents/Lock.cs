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
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.ShellComponents
{
    internal class Lock : Process
    {
        internal Lock() : base("Lock", ProcessType.Application) { }

        AppWindow window;

        Table userTable;
        
        TextBox passwordBox;
        Button logOnButton;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        Sound.SoundService soundService = ProcessManager.GetProcess<Sound.SoundService>();

        private static class Images
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Lock.Key.bmp")]
            private static byte[] _iconBytes_Key;
            internal static Bitmap Icon_Key = new Bitmap(_iconBytes_Key);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Lock.User.bmp")]
            private static byte[] _iconBytes_User;
            internal static Bitmap Icon_User = new Bitmap(_iconBytes_User);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Lock.UserArrow.bmp")]
            private static byte[] _iconBytes_UserArrow;
            internal static Bitmap Icon_UserArrow = new Bitmap(_iconBytes_UserArrow);
        }

        private const int width = 700;
        private const int height = 420;
        private const int leftPanelWidth = 240;
        private const int rightContentPadding = 28;
        private const int rightContentBottomPadding = 30;
        private const int accountsLabelY = 102;
        private const int tableY = 124;
        private const int tableHeight = 140;
        private const int passwordLabelY = tableY + tableHeight + 18;
        private const int passwordY = passwordLabelY + 22;
        private const int passwordHeight = 30;
        private const int logOnButtonY = passwordY + 44;
        private const int logOnButtonHeight = 38;

        private Color windowBackground;
        private Color outerBorder;
        private Color leftPanel;
        private Color leftPanelAccent;
        private Color leftPanelSoft;
        private Color rightPanel;
        private Color titleColor;
        private Color bodyColor;
        private Color hintColor;
        private Color inputBackground;
        private Color inputForeground;
        private Color selectionBackground;
        private Color selectionBorder;
        private Color primaryButton;
        private Color primaryButtonBorder;
        private Color primaryButtonText;

        private double shakiness = 0;

        private List<User> users = new List<User>();

        private static Color Blend(Color a, Color b, float t)
        {
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }

        private void SyncThemeColors()
        {
            windowBackground = UITheme.Surface;
            outerBorder = UITheme.SurfaceBorder;
            rightPanel = UITheme.Surface;
            titleColor = UITheme.TextPrimary;
            bodyColor = UITheme.TextSecondary;
            hintColor = UITheme.TextSecondary;
            inputBackground = UITheme.Surface;
            inputForeground = UITheme.TextPrimary;
            selectionBackground = UITheme.AccentLight;
            selectionBorder = UITheme.Accent;
            primaryButton = UITheme.Accent;
            primaryButtonBorder = UITheme.AccentDark;
            primaryButtonText = UITheme.WindowTitleText;

            leftPanel = Blend(UITheme.WindowTitleBackground, Color.Black, 0.28f);
            leftPanelAccent = UITheme.Accent;
            leftPanelSoft = Blend(leftPanel, UITheme.Accent, 0.32f);
        }

        private void ApplyThemeToControls()
        {
            if (userTable != null)
            {
                userTable.Background = inputBackground;
                userTable.Foreground = inputForeground;
                userTable.Border = outerBorder;
                userTable.SelectedBackground = selectionBackground;
                userTable.SelectedForeground = inputForeground;
                userTable.SelectedBorder = selectionBorder;
            }

            if (passwordBox != null)
            {
                passwordBox.Background = inputBackground;
                passwordBox.Foreground = inputForeground;
                passwordBox.PlaceholderForeground = hintColor;
            }

            if (logOnButton != null)
            {
                logOnButton.Background = primaryButton;
                logOnButton.Border = primaryButtonBorder;
                logOnButton.Foreground = primaryButtonText;
            }
        }

        private void RenderBackground()
        {
            SyncThemeColors();

            window.Clear(windowBackground);
            window.DrawRectangle(0, 0, width, height, outerBorder);

            int leftWidth = leftPanelWidth;
            int rightX = leftWidth + 1;
            int rightWidth = width - rightX;

            window.DrawFilledRectangle(0, 0, leftWidth, height, leftPanel);
            window.DrawFilledRectangle(leftWidth - 6, 0, 6, height, leftPanelAccent);
            window.DrawFilledRectangle(rightX, 0, rightWidth, height, rightPanel);

            window.DrawFilledRectangle(20, 22, 56, 56, leftPanelSoft);
            window.DrawRectangle(20, 22, 56, 56, Blend(leftPanelAccent, Color.White, 0.25f));
            window.DrawImageAlpha(Images.Icon_Key, 32, 34);

            window.DrawString("Welcome Back", UITheme.WindowTitleText, 20, 96);
            window.DrawString("CMLeonOS Desktop", Blend(UITheme.WindowTitleText, leftPanel, 0.45f), 20, 120);

            string selectedUserName = GetSelectedUser()?.Username ?? "Guest";
            window.DrawString("Selected account", Blend(UITheme.WindowTitleText, leftPanel, 0.55f), 20, 168);
            window.DrawString(selectedUserName, UITheme.WindowTitleText, 20, 190);
            window.DrawString(GetSelectedUserSubtitle(), Blend(UITheme.WindowTitleText, leftPanel, 0.5f), 20, 214);

            window.DrawFilledRectangle(20, height - 72, leftWidth - 40, 44, leftPanelSoft);
            window.DrawRectangle(20, height - 72, leftWidth - 40, 44, Blend(leftPanelAccent, Color.White, 0.2f));
            window.DrawString("Tip: type password.", UITheme.WindowTitleText, 32, height - 62);
            window.DrawString("Press Enter to sign in.", Blend(UITheme.WindowTitleText, leftPanelSoft, 0.4f), 32, height - 44);

            int rightContentX = leftWidth + rightContentPadding;
            window.DrawString("Sign in", titleColor, rightContentX, 28);
            window.DrawString("Choose an account, then enter your password.", bodyColor, rightContentX, 54);
            window.DrawString("Continue to the desktop when ready.", bodyColor, rightContentX, 72);

            window.DrawString("Accounts", hintColor, rightContentX, accountsLabelY);
            window.DrawString("Password", hintColor, rightContentX, passwordLabelY);
            window.DrawString("Secure local session", hintColor, rightContentX, height - rightContentBottomPadding);

        }

        private User GetSelectedUser()
        {
            if (userTable == null || users == null || users.Count == 0)
            {
                return null;
            }

            if (userTable.SelectedCellIndex < 0 || userTable.SelectedCellIndex >= users.Count)
            {
                return null;
            }

            return users[userTable.SelectedCellIndex];
        }

        private string GetSelectedUserSubtitle()
        {
            User selectedUser = GetSelectedUser();
            if (selectedUser == null)
            {
                return "No local users available";
            }

            return selectedUser.Admin ? "Administrator account" : "Standard local account";
        }

        private void RefreshLayout()
        {
            RenderBackground();
            ApplyThemeToControls();
            wm.Update(window);
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

            if (userTable.SelectedCellIndex >= users.Count)
            {
                return;
            }

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
            
            users = UserSystem.GetUsers() ?? new List<User>();
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
            SyncThemeColors();

            int contentX = leftPanelWidth + rightContentPadding;
            int contentWidth = width - contentX - rightContentPadding;
            
            userTable = new Table(window, contentX, tableY, contentWidth, tableHeight);
            userTable.CellHeight = 40;
            userTable.Background = inputBackground;
            userTable.Foreground = inputForeground;
            userTable.Border = outerBorder;
            userTable.SelectedBackground = selectionBackground;
            userTable.SelectedForeground = inputForeground;
            userTable.SelectedBorder = selectionBorder;
            userTable.AllowSelection = true;
            userTable.AllowDeselection = false;
            userTable.ScrollbarThickness = 18;
            userTable.TableCellSelected = _ => RefreshLayout();
            
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

            passwordBox = new TextBox(window, contentX, passwordY, contentWidth, passwordHeight);
            passwordBox.Shield = true;
            passwordBox.PlaceholderText = "Enter password";
            passwordBox.Background = inputBackground;
            passwordBox.Foreground = inputForeground;
            passwordBox.PlaceholderForeground = hintColor;
            passwordBox.Changed = RefreshLayout;
            passwordBox.Submitted = LogOn;
            wm.AddWindow(passwordBox);

            logOnButton = new Button(window, contentX, logOnButtonY, contentWidth, logOnButtonHeight);
            logOnButton.Text = "Log On";
            logOnButton.Background = primaryButton;
            logOnButton.Border = primaryButtonBorder;
            logOnButton.Foreground = primaryButtonText;
            logOnButton.Image = Images.Icon_UserArrow;
            logOnButton.ImageLocation = Button.ButtonImageLocation.Left;
            logOnButton.OnClick = LogOnClick;
            wm.AddWindow(logOnButton);

            RefreshLayout();
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
