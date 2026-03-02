using CMLeonOS;
using CMLeonOS.Gui.UILib;
using Cosmos.System.Graphics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Gui.UILib
{
    internal class FileBrowser
    {
        private Process process;
        private WindowManager wm;
        private Window window;
        private Table fileTable;
        private TextBox pathBox;
        private TextBox fileNameBox;
        private Button upButton;
        private Button selectButton;
        private Button cancelButton;
        
        private string currentPath = @"0:\";
        private string selectedPath = null;
        private Action<string> onFileSelected;
        private bool selectDirectoryOnly = false;
        
        private const int headerHeight = 32;
        private const int buttonHeight = 28;
        private const int buttonWidth = 80;
        private const int fileNameBoxHeight = 24;
        
        internal FileBrowser(Process process, WindowManager wm, Action<string> onFileSelected, bool selectDirectoryOnly = false)
        {
            this.process = process;
            this.wm = wm;
            this.onFileSelected = onFileSelected;
            this.selectDirectoryOnly = selectDirectoryOnly;
        }
        
        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Directory.bmp")]
            private static byte[] _iconBytes_Directory;
            internal static Bitmap Icon_Directory = new Bitmap(_iconBytes_Directory);
            
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File.bmp")]
            private static byte[] _iconBytes_File;
            internal static Bitmap Icon_File = new Bitmap(_iconBytes_File);
            
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.File_Text.bmp")]
            private static byte[] _iconBytes_File_Text;
            internal static Bitmap Icon_File_Text = new Bitmap(_iconBytes_File_Text);
            
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Files.Up.bmp")]
            private static byte[] _iconBytes_Up;
            internal static Bitmap Icon_Up = new Bitmap(_iconBytes_Up);
        }
        
        private Bitmap GetFileIcon(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            
            return extension switch
            {
                ".txt" or ".md" or ".log" or ".lua" or ".rs" => Icons.Icon_File_Text,
                _ => Icons.Icon_File
            };
        }
        
        private void PopulateFileTable()
        {
            fileTable.Cells.Clear();
            fileTable.SelectedCellIndex = -1;
            
            bool empty = true;
            
            foreach (string path in Directory.GetDirectories(currentPath))
            {
                fileTable.Cells.Add(new TableCell(Icons.Icon_Directory, Path.GetFileName(path), tag: "Directory"));
                empty = false;
            }
            
            foreach (string path in Directory.GetFiles(currentPath))
            {
                fileTable.Cells.Add(new TableCell(GetFileIcon(path), Path.GetFileName(path), tag: "File"));
                empty = false;
            }
            
            if (empty)
            {
                fileTable.Clear(fileTable.Background);
                fileTable.DrawString("This folder is empty.", Color.Black, (fileTable.Width / 2) - 80, 12);
                wm.Update(fileTable);
            }
            else
            {
                fileTable.Render();
            }
        }
        
        private void PathBoxChanged()
        {
            string inputPath = pathBox.Text;
            
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return;
            }
            
            if (!inputPath.Contains(':'))
            {
                inputPath = $@"0:\{inputPath}";
            }
            
            if (Directory.Exists(inputPath))
            {
                currentPath = inputPath;
                PopulateFileTable();
            }
            else if (File.Exists(inputPath))
            {
                currentPath = Path.GetDirectoryName(inputPath);
                PopulateFileTable();
            }
        }
        
        private void PathBoxSubmitted()
        {
            string inputPath = pathBox.Text;
            
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return;
            }
            
            if (!inputPath.Contains(':'))
            {
                inputPath = $@"0:\{inputPath}";
            }
            
            if (File.Exists(inputPath) || (selectDirectoryOnly && Directory.Exists(inputPath)))
            {
                selectedPath = inputPath;
                onFileSelected?.Invoke(selectedPath);
                Close();
            }
            else if (Directory.Exists(inputPath))
            {
                currentPath = inputPath;
                PopulateFileTable();
            }
        }
        
        private void UpClicked(int x, int y)
        {
            if (currentPath.Length > 3)
            {
                string parentPath = Path.GetDirectoryName(currentPath);
                if (!string.IsNullOrEmpty(parentPath))
                {
                    currentPath = @"0:\";
                }
                else
                {
                    currentPath = parentPath;
                }
                PopulateFileTable();
            }
        }
        
        private void SelectClicked(int x, int y)
        {
            if (fileTable.SelectedCellIndex >= 0)
            {
                TableCell selectedCell = fileTable.Cells[fileTable.SelectedCellIndex];
                string selectedName = selectedCell.Text;
                string fullPath = Path.Combine(currentPath, selectedName);
                
                string tag = selectedCell.Tag as string;
                if (tag == "Directory")
                {
                    currentPath = fullPath;
                    PopulateFileTable();
                }
                else if (tag == "File")
                {
                    if (selectDirectoryOnly)
                    {
                        string fileName = fileNameBox.Text;
                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = selectedName;
                        }
                        selectedPath = Path.Combine(currentPath, fileName);
                    }
                    else
                    {
                        selectedPath = fullPath;
                    }
                    onFileSelected?.Invoke(selectedPath);
                    Close();
                }
            }
            else if (selectDirectoryOnly)
            {
                string fileName = fileNameBox.Text;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    selectedPath = Path.Combine(currentPath, fileName);
                    onFileSelected?.Invoke(selectedPath);
                    Close();
                }
            }
        }
        
        private void FileTableSelected(int index)
        {
            SelectClicked(0, 0);
        }
        
        private void CancelClicked(int x, int y)
        {
            selectedPath = null;
            onFileSelected?.Invoke(null);
            Close();
        }
        
        private void FileTableDoubleClicked(int index)
        {
            SelectClicked(0, 0);
        }
        
        internal void Show()
        {
            window = new Window(process, 100, 100, 500, 400);
            wm.AddWindow(window);
            
            window.DrawString(selectDirectoryOnly ? "Save As" : "Open File", Color.DarkBlue, 12, 12);
            
            pathBox = new TextBox(window, 8, 40, window.Width - 16, 24)
            {
                Background = Color.White,
                Foreground = Color.Black,
                Text = currentPath,
                ReadOnly = false
            };
            pathBox.Changed = PathBoxChanged;
            pathBox.Submitted = PathBoxSubmitted;
            wm.AddWindow(pathBox);
            
            if (selectDirectoryOnly)
            {
                window.DrawString("File name:", Color.Black, 8, 72);
                
                fileNameBox = new TextBox(window, 8, 96, window.Width - 16, fileNameBoxHeight)
                {
                    Background = Color.White,
                    Foreground = Color.Black,
                    Text = "",
                    ReadOnly = false
                };
                wm.AddWindow(fileNameBox);
            }
            
            upButton = new Button(window, 8, selectDirectoryOnly ? 128 : 72, 60, buttonHeight)
            {
                Text = "Up",
                Image = Icons.Icon_Up,
                ImageLocation = Button.ButtonImageLocation.Left
            };
            upButton.OnClick = UpClicked;
            wm.AddWindow(upButton);
            
            int tableY = selectDirectoryOnly ? 128 + buttonHeight + 8 : 72 + buttonHeight + 8;
            int tableHeight = window.Height - tableY - buttonHeight - 8 - buttonHeight - 16;
            
            fileTable = new Table(window, 8, tableY, window.Width - 16, tableHeight)
            {
                AllowDeselection = false
            };
            fileTable.TableCellSelected = FileTableSelected;
            PopulateFileTable();
            wm.AddWindow(fileTable);
            
            int buttonY = window.Height - buttonHeight - 8;
            selectButton = new Button(window, window.Width - buttonWidth - 8, buttonY, buttonWidth, buttonHeight)
            {
                Text = selectDirectoryOnly ? "Save" : "Open"
            };
            selectButton.OnClick = SelectClicked;
            wm.AddWindow(selectButton);
            
            cancelButton = new Button(window, window.Width - buttonWidth * 2 - 16, buttonY, buttonWidth, buttonHeight)
            {
                Text = "Cancel"
            };
            cancelButton.OnClick = CancelClicked;
            wm.AddWindow(cancelButton);
            
            wm.Update(window);
        }
        
        internal void Close()
        {
            if (window != null)
            {
                wm.RemoveWindow(window);
                window = null;
            }
        }
    }
}
