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
using CMLeonOS.UILib.Animations;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.ShellComponents
{
    internal class StartMenu : Process
    {
        internal StartMenu() : base("StartMenu", ProcessType.Application)
        {
        }

        private const int menuWidth = 456;
        private const int menuHeight = 320;
        private const int outerPadding = 16;
        private const int searchHeight = 28;
        private const int footerHeight = 72;
        private const int actionButtonWidth = 104;
        private const int actionButtonHeight = 24;

        private static readonly Color MenuBackground = Color.FromArgb(24, 31, 43);
        private static readonly Color PanelBackground = Color.FromArgb(35, 44, 58);
        private static readonly Color PanelBorder = Color.FromArgb(58, 70, 89);
        private static readonly Color HeaderText = Color.White;
        private static readonly Color SubText = Color.FromArgb(178, 190, 210);
        private static readonly Color SearchBackground = Color.FromArgb(245, 247, 250);
        private static readonly Color SearchForeground = Color.FromArgb(28, 34, 45);
        private static readonly Color SearchPlaceholder = Color.FromArgb(126, 136, 149);
        private static readonly Color NeutralButton = Color.FromArgb(70, 83, 104);
        private static readonly Color NeutralButtonBorder = Color.FromArgb(90, 105, 127);
        private static readonly Color SearchResultsBackground = Color.FromArgb(31, 39, 52);
        private static readonly Color SearchResultsBorder = Color.FromArgb(54, 66, 84);

        internal static StartMenu CurrentStartMenu
        {
            get
            {
                StartMenu startMenu = ProcessManager.GetProcess<StartMenu>();
                if (startMenu == null && ProcessManager.GetProcess<Taskbar>() != null)
                {
                    startMenu = (StartMenu)ProcessManager.AddProcess(ProcessManager.GetProcess<WindowManager>(), new StartMenu());
                    startMenu.Start();
                }
                return startMenu;
            }
        }

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.StartMenu.User.bmp")]
            private static byte[] _iconBytes_User;
            internal static Bitmap Icon_User = new Bitmap(_iconBytes_User);
        }

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        private Button shutdownButton;
        private Button rebootButton;
        private Button logOutButton;
        private Button allAppsButton;

        private bool isOpen = false;

        private void StyleActionButton(Button button, string text)
        {
            button.Text = text;
            button.Background = NeutralButton;
            button.Border = NeutralButtonBorder;
            button.Foreground = Color.White;
        }

        internal void ShowStartMenu(bool focusSearch = false)
        {
            isOpen = true;

            bool leftHandStartButton = settingsService.LeftHandStartButton;

            int menuX = leftHandStartButton ? 0 : (int)(wm.ScreenWidth / 2 - menuWidth / 2);
            window = new Window(this, menuX, 24, menuWidth, menuHeight);

            window.Clear(MenuBackground);
            window.DrawRectangle(0, 0, window.Width, window.Height, PanelBorder);

            window.DrawString("Start", HeaderText, outerPadding, outerPadding - 2);
            window.DrawString("Apps, search, and quick actions", SubText, outerPadding, outerPadding + 18);

            int searchY = outerPadding + 40;
            int searchWidth = window.Width - (outerPadding * 2);
            window.DrawFilledRectangle(outerPadding, searchY, searchWidth, searchHeight, SearchBackground);
            window.DrawRectangle(outerPadding, searchY, searchWidth, searchHeight, Color.FromArgb(207, 214, 224));

            int footerY = window.Height - footerHeight - outerPadding;
            window.DrawFilledRectangle(outerPadding, footerY, window.Width - (outerPadding * 2), footerHeight, PanelBackground);
            window.DrawRectangle(outerPadding, footerY, window.Width - (outerPadding * 2), footerHeight, PanelBorder);

            Rectangle userRect = new Rectangle(outerPadding + 10, footerY + 10, 180, footerHeight - 20);
            window.DrawFilledRectangle(userRect.X, userRect.Y, userRect.Width, userRect.Height, Color.FromArgb(44, 55, 73));
            window.DrawRectangle(userRect.X, userRect.Y, userRect.Width, userRect.Height, Color.FromArgb(71, 86, 109));
            window.DrawImageAlpha(Icons.Icon_User, userRect.X + 10, (int)(userRect.Y + (userRect.Height / 2) - (Icons.Icon_User.Height / 2)));
            window.DrawString(UserSystem.CurrentLoggedInUser.Username, Color.White, userRect.X + 38, userRect.Y + 10);
            window.DrawString("Local session", SubText, userRect.X + 38, userRect.Y + 28);

            wm.AddWindow(window);

            int topActionY = footerY + 10;
            int bottomActionY = footerY + footerHeight - actionButtonHeight - 10;
            shutdownButton = new Button(window, window.Width - outerPadding - actionButtonWidth, bottomActionY, actionButtonWidth, actionButtonHeight);
            StyleActionButton(shutdownButton, "Shut down");
            shutdownButton.OnClick = ShutdownClicked;
            wm.AddWindow(shutdownButton);

            rebootButton = new Button(window, window.Width - outerPadding - actionButtonWidth * 2 - 8, bottomActionY, actionButtonWidth, actionButtonHeight);
            StyleActionButton(rebootButton, "Restart");
            rebootButton.OnClick = RebootClicked;
            wm.AddWindow(rebootButton);

            logOutButton = new Button(window, window.Width - outerPadding - actionButtonWidth * 2 - 8, topActionY, actionButtonWidth, actionButtonHeight);
            StyleActionButton(logOutButton, "Log out");
            logOutButton.OnClick = LogOutClicked;
            wm.AddWindow(logOutButton);

            allAppsButton = new Button(window, window.Width - outerPadding - actionButtonWidth, topActionY, actionButtonWidth, actionButtonHeight);
            StyleActionButton(allAppsButton, "All apps");
            allAppsButton.OnClick = AllAppsClicked;
            wm.AddWindow(allAppsButton);

            Table searchResults = null;
            TextBox searchBox = new TextBox(window, outerPadding + 6, searchY + 4, searchWidth - 12, searchHeight - 8);
            if (focusSearch)
            {
                wm.Focus = searchBox;
            }
            searchBox.PlaceholderText = "Search";
            searchBox.Background = SearchBackground;
            searchBox.Foreground = SearchForeground;
            searchBox.PlaceholderForeground = SearchPlaceholder;
            searchBox.Changed = () =>
            {
                if (searchResults == null)
                {
                    searchResults = new Table(searchBox, 0, searchBox.Height + 4, searchBox.Width, 0);

                    searchResults.CellHeight = 24;
                    searchResults.Background = SearchResultsBackground;
                    searchResults.Foreground = Color.White;
                    searchResults.Border = SearchResultsBorder;
                    searchResults.SelectedBackground = Color.FromArgb(62, 84, 120);
                    searchResults.SelectedBorder = Color.FromArgb(88, 117, 164);
                    searchResults.SelectedForeground = Color.White;

                    searchResults.TableCellSelected = (int index) =>
                    {
                        if (index != -1)
                        {
                            ((AppMetadata)searchResults.Cells[index].Tag).Start(this);
                            HideStartMenu();
                        }
                    };
                }

                searchResults.Cells.Clear();

                if (searchBox.Text.Trim().Length > 0)
                {
                    foreach (AppMetadata app in AppManager.AppMetadatas)
                    {
                        if (app.Name.ToLower().Contains(searchBox.Text.ToLower()))
                        {
                            string name = app.Name;
                            if (name.Length > 22)
                            {
                                name = name.Substring(0, 22).Trim() + "...";
                            }
                            searchResults.Cells.Add(new TableCell(app.Icon.Resize(20, 20), name, tag: app));
                        }
                    }
                }

                if (searchResults.Cells.Count > 0)
                {
                    searchResults.Resize(searchResults.Width, searchResults.Cells.Count * searchResults.CellHeight);
                    searchResults.Render();

                    wm.AddWindow(searchResults);

                    wm.Update(searchResults);
                }
                else
                {
                    wm.RemoveWindow(searchResults);
                }
            };
            searchBox.Submitted = () =>
            {
                searchBox.Text = string.Empty;
                wm.Update(searchBox);

                if (searchResults != null && searchResults.Cells.Count > 0)
                {
                    ((AppMetadata)searchResults.Cells[0].Tag).Start(this);
                    HideStartMenu();
                }
            };
            searchBox.OnUnfocused = () =>
            {
                if (searchResults != null)
                {
                    wm.RemoveWindow(searchResults);
                    searchResults = null;

                    searchBox.Text = string.Empty;
                    wm.Update(searchBox);
                }
            };
            wm.AddWindow(searchBox);

            wm.Update(window);
        }

        private void ShutdownClicked(int x, int y)
        {
            Power.Shutdown(reboot: false);
        }

        private void RebootClicked(int x, int y)
        {
            Power.Shutdown(reboot: true);
        }

        private void LogOutClicked(int x, int y)
        {
            HideStartMenu();
            ProcessManager.GetProcess<WindowManager>()?.ClearAllWindows();
            ProcessManager.AddProcess(wm, new Lock()).Start();
        }

        private void AllAppsClicked(int x, int y)
        {
            Table allAppsTable = new Table(window, 0, 0, window.Width, window.Height);

            allAppsTable.CellHeight = 32;

            allAppsTable.Background = MenuBackground;
            allAppsTable.Foreground = Color.White;
            allAppsTable.Border = PanelBorder;
            allAppsTable.SelectedBackground = Color.FromArgb(57, 76, 108);
            allAppsTable.SelectedBorder = Color.FromArgb(86, 109, 146);

            foreach (AppMetadata app in AppManager.AppMetadatas)
            {
                TableCell cell = new TableCell(app.Icon.Resize(20, 20), app.Name);
                allAppsTable.Cells.Add(cell);
            }
            allAppsTable.Render();

            allAppsTable.TableCellSelected = (int index) =>
            {
                if (index != -1)
                {
                    AppManager.AppMetadatas[index].Start(this);
                    HideStartMenu();
                }
            };

            wm.AddWindow(allAppsTable);

            MovementAnimation animation = new MovementAnimation(allAppsTable)
            {
                From = new Rectangle(allAppsTable.X, allAppsTable.Y, allAppsTable.Width, allAppsTable.Height),
                To = new Rectangle(allAppsTable.X, allAppsTable.Y, allAppsTable.Width, allAppsTable.Height + 64),
                Duration = 5
            };
            animation.Start();

            wm.Update(allAppsTable);
        }

        internal void HideStartMenu()
        {
            isOpen = false;
            wm.RemoveWindow(window);
        }

        internal void ToggleStartMenu(bool focusSearch = false)
        {
            if (isOpen)
            {
                HideStartMenu();
            }
            else
            {
                ShowStartMenu(focusSearch);
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Run()
        {
        }
    }
}
