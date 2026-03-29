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
using CMLeonOS.Utils;
using System;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class Files : Process
    {
        internal Files() : base("Files", ProcessType.Application) { }

        AppWindow window;

        Table entryTable;
        Table shortcutsTable;

        ImageBlock up;
        Button newFileButton;
        Button newFolderButton;

        TextBox pathBox;

        Window header;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Directory.bmp")]
            private static byte[] _iconBytes_Directory;
            internal static Bitmap Icon_Directory = new Bitmap(_iconBytes_Directory);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File.bmp")]
            private static byte[] _iconBytes_File;
            internal static Bitmap Icon_File = new Bitmap(_iconBytes_File);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File_Config.bmp")]
            private static byte[] _iconBytes_File_Config;
            internal static Bitmap Icon_File_Config = new Bitmap(_iconBytes_File_Config);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File_Rs.bmp")]
            private static byte[] _iconBytes_File_Rs;
            internal static Bitmap Icon_File_Rs = new Bitmap(_iconBytes_File_Rs);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File_Text.bmp")]
            private static byte[] _iconBytes_File_Text;
            internal static Bitmap Icon_File_Text = new Bitmap(_iconBytes_File_Text);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Drive.bmp")]
            private static byte[] _iconBytes_Drive;
            internal static Bitmap Icon_Drive = new Bitmap(_iconBytes_Drive);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Home.bmp")]
            private static byte[] _iconBytes_Home;
            internal static Bitmap Icon_Home = new Bitmap(_iconBytes_Home);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Up.bmp")]
            private static byte[] _iconBytes_Up;
            internal static Bitmap Icon_Up = new Bitmap(_iconBytes_Up);
        }

        private string currentDir = @"0:\";

        private const int pathBoxMargin = 4;
        private const int pathBoxHeight = 24;
        private const int shortcutsWidth = 128;
        private const int headerHeight = 56;
        private const int toolbarButtonWidth = 80;
        private const int toolbarButtonGap = 4;

        private const string dirEmptyMessage = "This folder is empty.";

        private readonly (string Name, string Path)[] shortcuts = new (string, string)[]
        {
            ("CMLeonOS (0:)", @"0:\"),
            ("ISO (1:)", @"1:\"),
            ("My Home", @$"0:\user\{UserSystem.CurrentLoggedInUser.Username}"),
            ("Users", @"0:\user"),
        };

        private Bitmap GetFileIcon(string path)
        {
            string extension = Path.GetExtension(path).ToLower();

            return extension switch
            {
                ".txt" or ".md" or ".log" or ".mi" => Icons.Icon_File_Text,
                ".rs" => Icons.Icon_File_Rs,
                ".ini" or ".cfg" => Icons.Icon_File_Config,
                _ => Icons.Icon_File
            };
        }

        private Bitmap GetDirectoryIcon(string path)
        {
            if (Path.TrimEndingDirectorySeparator(path).StartsWith(@"0:\user\"))
            {
                return Icons.Icon_Home;
            }

            switch (path)
            {
                case @"0:\":
                    return Icons.Icon_Drive;
                case @"1:\":
                    return Icons.Icon_Drive;
                default:
                    return Icons.Icon_Directory;
            }
        }

        private void PopulateEntryTable()
        {
            entryTable.Cells.Clear();
            entryTable.SelectedCellIndex = -1;

            bool empty = true;
            foreach (string path in Directory.GetDirectories(currentDir))
            {
                entryTable.Cells.Add(new TableCell(GetDirectoryIcon(path), Path.GetFileName(path), tag: "Directory"));
                empty = false;
            }
            foreach (string path in Directory.GetFiles(currentDir))
            {
                entryTable.Cells.Add(new TableCell(GetFileIcon(path), Path.GetFileName(path), tag: "File"));
                empty = false;
            }

            if (empty)
            {
                entryTable.Clear(entryTable.Background);
                entryTable.DrawString(dirEmptyMessage, Color.Black, (entryTable.Width / 2) - ((dirEmptyMessage.Length * 8) / 2), 12);
                wm.Update(entryTable);
            }
            else
            {
                entryTable.Render();
            }
        }

        private void RefreshCurrentDirectory()
        {
            PopulateEntryTable();
            RenderHeader();
            wm.Update(window);
        }

        private void PopulateShortcutTable()
        {
            shortcutsTable.Cells.Clear();
            foreach ((string Name, string Path) item in shortcuts)
            {
                shortcutsTable.Cells.Add(new TableCell(GetDirectoryIcon(item.Path), item.Name, tag: item.Path));
            }
            shortcutsTable.Render();
        }

        private bool NavigateTo(string path)
        {
            string sanitised = PathUtil.Sanitize(path);

            if (!Directory.Exists(sanitised))
            {
                MessageBox messageBox = new MessageBox(this, "Files", $"CMLeonOS can't find that folder.\nCheck the spelling and try again.");
                messageBox.Show();

                return false;
            }

            currentDir = sanitised;
            pathBox.Text = sanitised;

            PopulateEntryTable();
            UpdateSelectedShortcut();
            RenderHeader();

            if (sanitised == @"0:\")
            {
                window.Title = "Files";
            }
            else
            {
                window.Title = Path.GetDirectoryName(sanitised);
            }

            return true;
        }

        private void UpdateSelectedShortcut()
        {
            for (int i = 0; i < shortcuts.Length; i++)
            {
                if (shortcuts[i].Path == currentDir)
                {
                    shortcutsTable.SelectedCellIndex = i;
                    return;
                }
            }
            shortcutsTable.SelectedCellIndex = -1;
        }

        private void EntryTableDoubleClicked(int x, int y)
        {
            if (entryTable.SelectedCellIndex != -1)
            {
                TableCell cell = entryTable.Cells[entryTable.SelectedCellIndex];
                string path = PathUtil.Combine(currentDir, cell.Text);
                if ((string)cell.Tag == "Directory")
                {
                    NavigateTo(path);
                }
                else if ((string)cell.Tag == "File")
                {
                    try
                    {
                        string extension = Path.GetExtension(path).ToLower();
                        if (extension == ".txt" || extension == ".md")
                        {
                            ProcessManager.AddProcess(this, new Notepad(path)).Start();
                        }
                        else if (extension == ".lua")
                        {
                            ProcessManager.AddProcess(this, new CodeStudio.CodeStudio(path)).Start();
                        }
                        else if (extension == ".bmp")
                        {
                            ProcessManager.AddProcess(this, new ImageViewer(path)).Start();
                        }
                        else if (extension == ".mi")
                        {
                            ProcessManager.AddProcess(this, new MarkItViewer(path)).Start();
                        }
                        else if (extension == ".mus")
                        {
                            ProcessManager.AddProcess(this, new MusicEditor(path)).Start();
                        }
                        else
                        {
                            ShowOpenWithPrompt(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Logger.Instance.Error("Files", $"Error opening file: {ex.Message}");
                    }
                }
            }
        }

        private void OpenWithApp(string path, string appName)
        {
            switch (appName)
            {
                case "Notepad":
                    ProcessManager.AddProcess(this, new Notepad(path)).Start();
                    break;
                case "CodeStudio":
                    ProcessManager.AddProcess(this, new CodeStudio.CodeStudio(path)).Start();
                    break;
                case "Image Viewer":
                    ProcessManager.AddProcess(this, new ImageViewer(path)).Start();
                    break;
                case "MarkIt Viewer":
                    ProcessManager.AddProcess(this, new MarkItViewer(path)).Start();
                    break;
                case "Music Editor":
                    ProcessManager.AddProcess(this, new MusicEditor(path)).Start();
                    break;
                default:
                    Logger.Logger.Instance.Warning("Files", $"Unsupported open-with app: {appName}");
                    break;
            }
        }

        private void ShowOpenWithPrompt(string path)
        {
            AppWindow openWithWindow = new AppWindow(this, 320, 240, 300, 206);
            openWithWindow.Title = "Open With";
            openWithWindow.Icon = AppManager.DefaultAppIcon;
            wm.AddWindow(openWithWindow);

            openWithWindow.Clear(UITheme.Surface);
            openWithWindow.DrawRectangle(0, 0, openWithWindow.Width, openWithWindow.Height, UITheme.SurfaceBorder);
            openWithWindow.DrawString("Open this file with:", UITheme.TextPrimary, 12, 12);
            openWithWindow.DrawString(Path.GetFileName(path), UITheme.TextSecondary, 12, 32);

            Button notepadButton = new Button(openWithWindow, 12, 68, 84, 24);
            notepadButton.Text = "Notepad";
            notepadButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
                OpenWithApp(path, "Notepad");
            };
            wm.AddWindow(notepadButton);

            Button codeStudioButton = new Button(openWithWindow, 108, 68, 84, 24);
            codeStudioButton.Text = "CodeStudio";
            codeStudioButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
                OpenWithApp(path, "CodeStudio");
            };
            wm.AddWindow(codeStudioButton);

            Button imageViewerButton = new Button(openWithWindow, 204, 68, 84, 24);
            imageViewerButton.Text = "Image";
            imageViewerButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
                OpenWithApp(path, "Image Viewer");
            };
            wm.AddWindow(imageViewerButton);

            Button markItButton = new Button(openWithWindow, 12, 98, 84, 24);
            markItButton.Text = "MarkIt";
            markItButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
                OpenWithApp(path, "MarkIt Viewer");
            };
            wm.AddWindow(markItButton);

            Button musicButton = new Button(openWithWindow, 108, 98, 84, 24);
            musicButton.Text = "Music";
            musicButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
                OpenWithApp(path, "Music Editor");
            };
            wm.AddWindow(musicButton);

            Button cancelButton = new Button(openWithWindow, openWithWindow.Width - 80 - 12, openWithWindow.Height - 20 - 12, 80, 20);
            cancelButton.Text = "Cancel";
            cancelButton.OnClick = (_, _) =>
            {
                wm.RemoveWindow(openWithWindow);
            };
            wm.AddWindow(cancelButton);

            wm.Update(openWithWindow);
        }

        private void ShortcutsTableCellSelected(int index)
        {
            if (index != -1)
            {
                bool success = NavigateTo(shortcuts[index].Path);
                if (!success)
                {
                    UpdateSelectedShortcut();
                }
            }
        }

        private void PathBoxSubmitted()
        {
            bool success = NavigateTo(pathBox.Text);
            if (!success)
            {
                pathBox.Text = currentDir;
            }
        }

        private void UpClicked()
        {
            DirectoryInfo parent = Directory.GetParent(currentDir);
            if (parent != null)
            {
                NavigateTo(parent.FullName);
            }
        }

        private void PromptCreateFile()
        {
            PromptBox promptBox = new PromptBox(this, "New File", "Create an empty file in the current folder.", "NewFile.txt", (string name) =>
            {
                try
                {
                    string sanitizedName = Path.GetFileName(PathUtil.Sanitize((name ?? string.Empty).Trim()));
                    if (string.IsNullOrWhiteSpace(sanitizedName))
                    {
                        return;
                    }

                    string path = Path.Combine(currentDir, sanitizedName);
                    if (File.Exists(path) || Directory.Exists(path))
                    {
                        new MessageBox(this, "Files", "A file or folder with that name already exists.").Show();
                        return;
                    }

                    File.WriteAllText(path, string.Empty);
                    RefreshCurrentDirectory();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Instance.Error("Files", $"Error creating file: {ex.Message}");
                    new MessageBox(this, "Files", "Unable to create the file.").Show();
                }
            });
            promptBox.Show();
        }

        private void PromptCreateFolder()
        {
            PromptBox promptBox = new PromptBox(this, "New Folder", "Create a folder in the current directory.", "NewFolder", (string name) =>
            {
                try
                {
                    string sanitizedName = Path.GetFileName(PathUtil.Sanitize((name ?? string.Empty).Trim()));
                    if (string.IsNullOrWhiteSpace(sanitizedName))
                    {
                        return;
                    }

                    string path = Path.Combine(currentDir, sanitizedName);
                    if (File.Exists(path) || Directory.Exists(path))
                    {
                        new MessageBox(this, "Files", "A file or folder with that name already exists.").Show();
                        return;
                    }

                    Directory.CreateDirectory(path);
                    RefreshCurrentDirectory();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Instance.Error("Files", $"Error creating folder: {ex.Message}");
                    new MessageBox(this, "Files", "Unable to create the folder.").Show();
                }
            });
            promptBox.Show();
        }

        private void RenderHeader(bool updateWindow = true)
        {
            header.Clear(Color.DarkBlue);

            header.DrawImageAlpha(GetDirectoryIcon(currentDir), 8, 8);

            DirectoryInfo info = new DirectoryInfo(currentDir);

            string currentDirFriendlyName = Path.GetFileName(currentDir);
            if (currentDir == $@"0:\user\{UserSystem.CurrentLoggedInUser.Username}")
            {
                currentDirFriendlyName = "My Home";
            }
            header.DrawString(info.Parent == null ? currentDir : currentDirFriendlyName, Color.White, 32, 8);

            if (updateWindow)
            {
                wm.Update(header);
            }
        }

        private void WindowClicked(int x, int y)
        {
            if (x < pathBoxHeight && y < pathBoxHeight)
            {
                UpClicked();
            }
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 288, 240, 512, 304);
            wm.AddWindow(window);
            window.Title = "Files";
            window.Icon = AppManager.GetAppMetadata("Files").Icon;
            window.OnClick = WindowClicked;
            window.Closing = TryStop;

            window.Clear(Color.DarkGray);

            window.DrawImageAlpha(Icons.Icon_Up, 0, 0);

            pathBox = new TextBox(window, (pathBoxMargin / 2) + pathBoxHeight, pathBoxMargin / 2, window.Width - pathBoxMargin - pathBoxHeight, 24 - pathBoxMargin);
            pathBox.Text = currentDir;
            pathBox.Submitted = PathBoxSubmitted;
            wm.AddWindow(pathBox);

            int buttonY = pathBoxHeight + 28;
            int buttonX = shortcutsWidth + 8;

            entryTable = new Table(window, shortcutsWidth, pathBoxHeight + headerHeight, window.Width - shortcutsWidth, window.Height - pathBoxHeight - headerHeight);
            entryTable.OnDoubleClick = EntryTableDoubleClicked;
            PopulateEntryTable();
            wm.AddWindow(entryTable);

            header = new Window(this, window, shortcutsWidth, pathBoxHeight, window.Width - shortcutsWidth, headerHeight);
            wm.AddWindow(header);
            RenderHeader(updateWindow: false);

            newFileButton = new Button(window, buttonX, buttonY, toolbarButtonWidth, 20);
            newFileButton.Text = "New File";
            newFileButton.OnClick = (_, _) => PromptCreateFile();
            wm.AddWindow(newFileButton);

            newFolderButton = new Button(window, buttonX + toolbarButtonWidth + toolbarButtonGap, buttonY, toolbarButtonWidth, 20);
            newFolderButton.Text = "New Folder";
            newFolderButton.OnClick = (_, _) => PromptCreateFolder();
            wm.AddWindow(newFolderButton);

            shortcutsTable = new Table(window, 0, pathBoxHeight, shortcutsWidth, window.Height - pathBoxHeight);
            shortcutsTable.AllowDeselection = false;
            shortcutsTable.Background = Color.DarkGray;
            shortcutsTable.Foreground = Color.White;
            PopulateShortcutTable();
            shortcutsTable.SelectedCellIndex = 0;
            shortcutsTable.TableCellSelected = ShortcutsTableCellSelected;
            wm.AddWindow(shortcutsTable);

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
