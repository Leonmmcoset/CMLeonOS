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
    internal class ImageViewer : Process
    {
        internal ImageViewer() : base("Image Viewer", ProcessType.Application) { }

        internal ImageViewer(string path) : base("Image Viewer", ProcessType.Application)
        {
            initialPath = path;
        }

        private AppWindow window;
        private Window canvas;
        private TextBox pathBox;
        private Button openButton;
        private Button loadButton;
        private Button clearButton;

        private WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private FileBrowser fileBrowser;

        private Bitmap currentBitmap;
        private string currentPath = string.Empty;
        private string initialPath = string.Empty;

        private const int toolbarHeight = 30;
        private const int padding = 6;
        private const int buttonWidth = 64;

        private void ShowError(string text)
        {
            MessageBox messageBox = new MessageBox(this, "Image Viewer", text);
            messageBox.Show();
        }

        private void Relayout()
        {
            int y = padding;
            int buttonY = y;

            openButton.MoveAndResize(window.Width - (buttonWidth * 3 + padding * 3), buttonY, buttonWidth, toolbarHeight);
            loadButton.MoveAndResize(window.Width - (buttonWidth * 2 + padding * 2), buttonY, buttonWidth, toolbarHeight);
            clearButton.MoveAndResize(window.Width - (buttonWidth + padding), buttonY, buttonWidth, toolbarHeight);

            int pathWidth = window.Width - (buttonWidth * 3 + padding * 5);
            pathBox.MoveAndResize(padding, y + 3, pathWidth, toolbarHeight - 6);

            int canvasY = toolbarHeight + (padding * 2);
            int canvasHeight = window.Height - canvasY - padding;
            canvas.MoveAndResize(padding, canvasY, window.Width - (padding * 2), canvasHeight);
            RenderCanvas();
        }

        private void DrawBitmapFit(Bitmap bitmap, Window target)
        {
            int srcWidth = (int)bitmap.Width;
            int srcHeight = (int)bitmap.Height;
            if (srcWidth <= 0 || srcHeight <= 0)
            {
                return;
            }

            int dstWidth = target.Width - 2;
            int dstHeight = target.Height - 2;
            if (dstWidth <= 0 || dstHeight <= 0)
            {
                return;
            }

            int scaledWidth = dstWidth;
            int scaledHeight = (srcHeight * dstWidth) / srcWidth;
            if (scaledHeight > dstHeight)
            {
                scaledHeight = dstHeight;
                scaledWidth = (srcWidth * dstHeight) / srcHeight;
            }

            scaledWidth = Math.Max(1, scaledWidth);
            scaledHeight = Math.Max(1, scaledHeight);

            int startX = (target.Width - scaledWidth) / 2;
            int startY = (target.Height - scaledHeight) / 2;

            for (int y = 0; y < scaledHeight; y++)
            {
                int srcY = (y * srcHeight) / scaledHeight;
                int sourceRowOffset = srcY * srcWidth;
                int targetY = startY + y;
                for (int x = 0; x < scaledWidth; x++)
                {
                    int srcX = (x * srcWidth) / scaledWidth;
                    int sourceIndex = sourceRowOffset + srcX;
                    Color color = Color.FromArgb(bitmap.RawData[sourceIndex]);
                    target.DrawPoint(startX + x, targetY, color);
                }
            }
        }

        private void RenderCanvas()
        {
            canvas.Clear(Color.FromArgb(20, 20, 20));
            canvas.DrawRectangle(0, 0, canvas.Width, canvas.Height, Color.FromArgb(70, 70, 70));

            if (currentBitmap == null)
            {
                const string hint = "Open a BMP file to preview.";
                canvas.DrawString(hint, Color.LightGray, Math.Max(2, (canvas.Width / 2) - ((hint.Length * 8) / 2)), canvas.Height / 2 - 8);
                wm.Update(canvas);
                return;
            }

            DrawBitmapFit(currentBitmap, canvas);

            string fileName = Path.GetFileName(currentPath);
            int srcWidth = (int)currentBitmap.Width;
            int srcHeight = (int)currentBitmap.Height;
            string info = $"{fileName}  {srcWidth}x{srcHeight}";
            canvas.DrawFilledRectangle(6, canvas.Height - 22, Math.Min(canvas.Width - 12, (info.Length * 8) + 8), 16, Color.FromArgb(0, 0, 0));
            canvas.DrawString(info, Color.White, 10, canvas.Height - 20);

            wm.Update(canvas);
        }

        private bool LoadImage(string path, bool showPopup = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                if (showPopup)
                {
                    ShowError("Path is empty.");
                }
                return false;
            }

            string sanitizedPath = PathUtil.Sanitize(path.Trim());
            if (!File.Exists(sanitizedPath))
            {
                if (showPopup)
                {
                    ShowError("File not found.");
                }
                return false;
            }

            if (!Path.GetExtension(sanitizedPath).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                if (showPopup)
                {
                    ShowError("Only BMP images are supported.");
                }
                return false;
            }

            try
            {
                byte[] bmpBytes = File.ReadAllBytes(sanitizedPath);
                currentBitmap = new Bitmap(bmpBytes);
                currentPath = sanitizedPath;
                pathBox.Text = sanitizedPath;
                window.Title = $"Image Viewer - {Path.GetFileName(sanitizedPath)}";
                RenderCanvas();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Logger.Instance.Error("ImageViewer", $"Failed to load BMP: {ex.Message}");
                if (showPopup)
                {
                    ShowError("Cannot load this BMP file.");
                }
                return false;
            }
        }

        private void OpenDialog()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    LoadImage(selectedPath);
                }
            }, selectDirectoryOnly: false);
            fileBrowser.Show();
        }

        private void LoadFromPath()
        {
            LoadImage(pathBox.Text);
        }

        private void ClearImage()
        {
            currentBitmap = null;
            currentPath = string.Empty;
            window.Title = "Image Viewer";
            RenderCanvas();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 170, 90, 760, 520);
            window.Title = "Image Viewer";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Relayout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            pathBox = new TextBox(window, 0, 0, 1, 1);
            pathBox.PlaceholderText = @"0:\path\image.bmp";
            pathBox.Submitted = LoadFromPath;
            wm.AddWindow(pathBox);

            openButton = new Button(window, 0, 0, 1, 1);
            openButton.Text = "Open";
            openButton.OnClick = (_, _) => OpenDialog();
            wm.AddWindow(openButton);

            loadButton = new Button(window, 0, 0, 1, 1);
            loadButton.Text = "Load";
            loadButton.OnClick = (_, _) => LoadFromPath();
            wm.AddWindow(loadButton);

            clearButton = new Button(window, 0, 0, 1, 1);
            clearButton.Text = "Clear";
            clearButton.OnClick = (_, _) => ClearImage();
            wm.AddWindow(clearButton);

            canvas = new Window(this, window, 0, 0, 1, 1);
            wm.AddWindow(canvas);

            Relayout();

            if (!string.IsNullOrWhiteSpace(initialPath))
            {
                LoadImage(initialPath, showPopup: false);
            }

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
