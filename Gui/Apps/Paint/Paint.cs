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

using Cosmos.System;
using Cosmos.System.Graphics;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.Utils;
using System;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps.Paint
{
    internal class Paint : Process
    {
        internal Paint() : base("Paint", ProcessType.Application)
        {
        }

        AppWindow window;

        Window canvas;
        Button openButton;
        Button saveButton;
        Button saveAsButton;

        ToolBox toolBox;

        ColourPicker colourPicker;
        FileBrowser fileBrowser;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private bool down = false;
        private const int sidePanelWidth = 128;
        private const int topBarHeight = 40;
        private const int topButtonWidth = 84;
        private const int topButtonHeight = 24;
        private const int topButtonGap = 8;
        private string currentFilePath = string.Empty;

        internal Color SelectedColor { get; set; } = Color.Black;

        internal bool IsInBounds(int x, int y)
        {
            if (x >= canvas.Width || y >= canvas.Height) return false;
            if (x < 0 || y < 0) return false;

            return true;
        }

        private void CanvasDown(int x, int y)
        {
            down = true;
        }

        private void ShowMessage(string title, string text)
        {
            MessageBox messageBox = new MessageBox(this, title, text);
            messageBox.Show();
        }

        private void Relayout()
        {
            toolBox.MoveAndResize(0, 0, sidePanelWidth, window.Height);
            colourPicker.MoveAndResize(window.Width - sidePanelWidth, 0, sidePanelWidth, window.Height);

            int buttonsX = sidePanelWidth + 12;
            openButton.MoveAndResize(buttonsX, 8, topButtonWidth, topButtonHeight);
            saveButton.MoveAndResize(buttonsX + topButtonWidth + topButtonGap, 8, topButtonWidth, topButtonHeight);
            saveAsButton.MoveAndResize(buttonsX + (topButtonWidth + topButtonGap) * 2, 8, topButtonWidth, topButtonHeight);
            openButton.Render();
            saveButton.Render();
            saveAsButton.Render();

            int canvasX = sidePanelWidth + 12;
            int canvasY = topBarHeight + 8;
            int canvasWidth = window.Width - sidePanelWidth * 2 - 24;
            int canvasHeight = window.Height - canvasY - 12;
            canvas.MoveAndResize(canvasX, canvasY, canvasWidth, canvasHeight);

            if (canvasWidth > 0 && canvasHeight > 0)
            {
                canvas.DrawRectangle(0, 0, canvas.Width, canvas.Height, Color.Black);
                wm.Update(canvas);
            }
        }

        private void UpdateWindowTitle()
        {
            if (string.IsNullOrWhiteSpace(currentFilePath))
            {
                window.Title = "Paint";
                return;
            }

            window.Title = $"Paint - {Path.GetFileName(currentFilePath)}";
        }

        private byte[] BuildBmpBytes()
        {
            int width = canvas.Width;
            int height = canvas.Height;
            int bytesPerPixel = 4;
            int pixelDataSize = width * height * bytesPerPixel;
            int fileSize = 54 + pixelDataSize;
            byte[] bytes = new byte[fileSize];

            bytes[0] = (byte)'B';
            bytes[1] = (byte)'M';
            BitConverter.GetBytes(fileSize).CopyTo(bytes, 2);
            BitConverter.GetBytes(54).CopyTo(bytes, 10);
            BitConverter.GetBytes(40).CopyTo(bytes, 14);
            BitConverter.GetBytes(width).CopyTo(bytes, 18);
            BitConverter.GetBytes(height).CopyTo(bytes, 22);
            BitConverter.GetBytes((short)1).CopyTo(bytes, 26);
            BitConverter.GetBytes((short)32).CopyTo(bytes, 28);
            BitConverter.GetBytes(pixelDataSize).CopyTo(bytes, 34);

            int offset = 54;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = canvas.GetPixel(x, y);
                    bytes[offset++] = color.B;
                    bytes[offset++] = color.G;
                    bytes[offset++] = color.R;
                    bytes[offset++] = color.A;
                }
            }

            return bytes;
        }

        private void SaveCanvas(string path)
        {
            try
            {
                string sanitizedPath = PathUtil.Sanitize(path.Trim());
                if (!sanitizedPath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    sanitizedPath += ".bmp";
                }

                string directory = Path.GetDirectoryName(sanitizedPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(sanitizedPath, BuildBmpBytes());
                currentFilePath = sanitizedPath;
                UpdateWindowTitle();
                ShowMessage("Paint", "Saved BMP successfully.");
            }
            catch (Exception ex)
            {
                ShowMessage("Paint", $"Save failed: {ex.Message}");
            }
        }

        private void OpenImage(string path)
        {
            try
            {
                string sanitizedPath = PathUtil.Sanitize(path.Trim());
                if (!File.Exists(sanitizedPath))
                {
                    ShowMessage("Paint", "File not found.");
                    return;
                }

                if (!sanitizedPath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("Paint", "Only BMP files are supported.");
                    return;
                }

                Bitmap bitmap = new Bitmap(File.ReadAllBytes(sanitizedPath));
                canvas.Clear(Color.White);

                int drawWidth = Math.Min(canvas.Width, (int)bitmap.Width);
                int drawHeight = Math.Min(canvas.Height, (int)bitmap.Height);
                for (int y = 0; y < drawHeight; y++)
                {
                    int rowOffset = y * (int)bitmap.Width;
                    for (int x = 0; x < drawWidth; x++)
                    {
                        canvas.DrawPoint(x, y, Color.FromArgb(bitmap.RawData[rowOffset + x]));
                    }
                }

                canvas.DrawRectangle(0, 0, canvas.Width, canvas.Height, Color.Black);
                wm.Update(canvas);

                currentFilePath = sanitizedPath;
                UpdateWindowTitle();
            }
            catch (Exception ex)
            {
                ShowMessage("Paint", $"Open failed: {ex.Message}");
            }
        }

        private void OpenClicked(int x, int y)
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    OpenImage(selectedPath);
                }
            }, selectDirectoryOnly: false);
            fileBrowser.Show();
        }

        private void SaveAsClicked(int x, int y)
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    SaveCanvas(selectedPath);
                }
            }, selectDirectoryOnly: true);
            fileBrowser.Show();
        }

        private void SaveClicked(int x, int y)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath))
            {
                SaveAsClicked(x, y);
                return;
            }

            SaveCanvas(currentFilePath);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 768, 448);
            window.Title = "Paint";
            window.Icon = AppManager.GetAppMetadata("Paint").Icon;
            window.Closing = TryStop;
            window.UserResized = Relayout;
            window.Clear(Color.FromArgb(73, 73, 73));
            wm.AddWindow(window);

            canvas = new Window(this, 0, 0, 1, 1);
            canvas.RelativeTo = window;
            canvas.OnDown = CanvasDown;
            canvas.Clear(Color.White);
            wm.AddWindow(canvas);

            openButton = new Button(window, 0, 0, 1, 1);
            openButton.Text = "Open";
            openButton.OnClick = OpenClicked;
            wm.AddWindow(openButton);

            saveButton = new Button(window, 0, 0, 1, 1);
            saveButton.Text = "Save";
            saveButton.OnClick = SaveClicked;
            wm.AddWindow(saveButton);

            saveAsButton = new Button(window, 0, 0, 1, 1);
            saveAsButton.Text = "Save As";
            saveAsButton.OnClick = SaveAsClicked;
            wm.AddWindow(saveAsButton);

            toolBox = new ToolBox(this, 0, 0, sidePanelWidth, window.Height);
            toolBox.RelativeTo = window;
            colourPicker = new ColourPicker(this, window.Width - sidePanelWidth, 0, sidePanelWidth, window.Height);
            colourPicker.RelativeTo = window;

            UpdateWindowTitle();
            Relayout();

            wm.Update(window);
        }

        public override void Run()
        {
            if (down)
            {
                if (MouseManager.MouseState == MouseState.None)
                {
                    down = false;
                }

                toolBox.SelectedTool.Run(
                   this,
                   canvas,
                   MouseManager.MouseState,
                   (int)(MouseManager.X - canvas.ScreenX),
                   (int)(MouseManager.Y - canvas.ScreenY)
                );

                wm.Update(canvas);
            }
        }
    }
}
