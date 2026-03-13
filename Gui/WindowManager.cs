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
using CMLeonOS.Gui.ShellComponents;
using CMLeonOS.Settings;
using CMLeonOS.Driver;
using CMLeonOS.Logger;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui
{
    internal class WindowManager : Process
    {
        internal WindowManager() : base("WindowManager", ProcessType.Service)
        {
            Critical = true;
        }

        private VMWareSVGAII driver;

        internal List<Window> Windows = new List<Window>();

        internal Queue<Window> UpdateQueue = new Queue<Window>();

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Cursor.bmp")]
        private static byte[] cursorBytes;
        private static Bitmap cursorBitmap = new Bitmap(cursorBytes);

        /*[IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.WaitCursor.bmp")]
        private static byte[] waitCursorBytes;
        private static Bitmap waitCursorBitmap = new Bitmap(waitCursorBytes);*/

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Wallpaper_1280_800.bmp")]
        private static byte[] wallpaperBytes;
        private static Bitmap wallpaperBitmap = new Bitmap(wallpaperBytes);

        internal uint ScreenWidth { get; private set; }
        internal uint ScreenHeight { get; private set; }

        internal Window Focus { get; set; }

        private uint bytesPerPixel { get; set; }

        private bool bufferModified = false;

        private DateTime lastClickDate = DateTime.MinValue;
        private TimeSpan doubleClickTimeSpan = TimeSpan.FromSeconds(1);

        private Window fpsCounter;
        private int fps;
        private int framesThisSecond;
        private int lastSecond;

        private bool fpsShown = true;

        private MouseState lastMouseState = MouseState.None;
        private uint lastMouseX = 0;
        private uint lastMouseY = 0;

        private int sweepCounter = 0;

        private Bitmap wallpaperResized;

        internal int Fps
        {
            get
            {
                return fps;
            }
        }

        internal List<Mode> AvailableModes { get; } = new List<Mode>
        {
            /* SD Resolutions */
            /*new Mode(320, 200, ColorDepth.ColorDepth32),
            new Mode(320, 240, ColorDepth.ColorDepth32),
            new Mode(640, 480, ColorDepth.ColorDepth32),
            new Mode(720, 480, ColorDepth.ColorDepth32),*/
            new Mode(800, 600, ColorDepth.ColorDepth32),
            new Mode(1024, 768, ColorDepth.ColorDepth32),
            new Mode(1152, 768, ColorDepth.ColorDepth32),

            /* Old HD-Ready Resolutions */
            new Mode(1280, 720, ColorDepth.ColorDepth32),
            new Mode(1280, 768, ColorDepth.ColorDepth32),
            new Mode(1280, 800, ColorDepth.ColorDepth32),  // WXGA
            new Mode(1280, 1024, ColorDepth.ColorDepth32), // SXGA

            /* Better HD-Ready Resolutions */
            new Mode(1360, 768, ColorDepth.ColorDepth32),
            new Mode(1440, 900, ColorDepth.ColorDepth32),  // WXGA+
            new Mode(1400, 1050, ColorDepth.ColorDepth32), // SXGA+
            new Mode(1600, 1200, ColorDepth.ColorDepth32), // UXGA
            new Mode(1680, 1050, ColorDepth.ColorDepth32), // WXGA++

            /* HDTV Resolutions */
            new Mode(1920, 1080, ColorDepth.ColorDepth32),
            new Mode(1920, 1200, ColorDepth.ColorDepth32), // WUXGA

            /* 2K Resolutions */
            /*new Mode(2048, 1536, ColorDepth.ColorDepth32), // QXGA
            new Mode(2560, 1080, ColorDepth.ColorDepth32), // UW-UXGA
            new Mode(2560, 1600, ColorDepth.ColorDepth32), // WQXGA
            new Mode(2560, 2048, ColorDepth.ColorDepth32), // QXGA+
            new Mode(3200, 2048, ColorDepth.ColorDepth32), // WQXGA+
            new Mode(3200, 2400, ColorDepth.ColorDepth32), // QUXGA
            new Mode(3840, 2400, ColorDepth.ColorDepth32), // WQUXGA*/
        };

        private void RenderWindow(Window window)
        {
            bufferModified = true;

            int screenX = window.ScreenX;
            int screenY = window.ScreenY;
            int height = (int)Math.Min(window.Height, ScreenHeight - screenY);
            int width = (int)Math.Min(window.Width, ScreenWidth - screenX);

            uint index = (uint)((screenY * ScreenWidth) + screenX);
            int byteOffset = (int)((index * bytesPerPixel) + driver.FrameSize);

            for (int y = 0; y < height; y++)
            {
                int sourceIndex = y * window.Width;
                driver.VideoMemory.Copy(aByteOffset: byteOffset, aData: window.Buffer, aIndex: sourceIndex, aCount: width);
                byteOffset += (int)(ScreenWidth * bytesPerPixel);
            }
        }

        private void UpdateAbove(Window window)
        {
            Rectangle aRect = new Rectangle(window.ScreenX, window.ScreenY, window.Width, window.Height);
            int aboveIndex = Windows.IndexOf(window) + 1;
            for (int i = aboveIndex; i < Windows.Count; i++)
            {
                Window bWindow = Windows[i];
                Rectangle bRect = new Rectangle(bWindow.ScreenX, bWindow.ScreenY, bWindow.Width, bWindow.Height);
                if (aRect.IntersectsWith(bRect))
                {
                    RenderWindow(bWindow);
                    aRect = Rectangle.Union(aRect, bRect);
                }
            }
        }

        internal void Update(Window window)
        {
            if (!Windows.Contains(window)) return;
            RenderWindow(window);
            UpdateAbove(window);
        }

        internal void RerenderAll()
        {
            RenderWallpaper();

            foreach (Window window in Windows)
            {
                RenderWindow(window);
            }
        }

        internal void AddWindow(Window window)
        {
            if (Windows.Contains(window)) return;

            Windows.Add(window);

            Logger.Logger.Instance.Debug("WindowManager", $"Added window: {window.GetType().Name}");

            UpdateDock();
        }

        internal void RemoveWindow(Window window, bool rerender = true)
        {
            Windows.Remove(window);

            Logger.Logger.Instance.Debug("WindowManager", $"Removed window: {window.GetType().Name}");

            for (int i = Windows.Count - 1; i >= 0; i--)
            {
                if (i >= Windows.Count) continue;
                if (Windows[i].RelativeTo == window)
                {
                    RemoveWindow(Windows[i], rerender: false);
                }
            }
            if (rerender)
            {
                RerenderAll();
            }

            UpdateDock();
        }

        internal void ClearAllWindows()
        {
            for (int i = Windows.Count - 1; i >= 0; i--)
            {
                if (i >= Windows.Count) continue;
                if (Windows[i].Process != this)
                {
                    if (Windows[i] is UILib.AppWindow appWindow)
                    {
                        appWindow.Closing?.Invoke();
                    }
                    RemoveWindow(Windows[i], rerender: false);
                }
            }
            RerenderAll();
        }

        private void UpdateDock()
        {
            var dock = ShellComponents.Dock.Dock.CurrentDock;

            if (dock != null)
            {
                dock.UpdateWindows();
            }
        }

        private void SetupDriver()
        {
            driver = new VMWareSVGAII();
            driver.SetMode(ScreenWidth, ScreenHeight, depth: bytesPerPixel * 8);
        }

        private void SetupMouse()
        {
            MouseManager.ScreenWidth = ScreenWidth;
            MouseManager.ScreenHeight = ScreenHeight;

            MouseManager.X = ScreenWidth / 2;
            MouseManager.Y = ScreenHeight / 2;

            driver.DefineAlphaCursor(cursorBitmap);
        }

        private Window GetWindowAtPos(uint x, uint y)
        {
            for (int i = Windows.Count - 1; i >= 0; i--)
            {
                Window window = Windows[i];
                if (x >= window.ScreenX
                    && y >= window.ScreenY
                    && x < window.ScreenX + window.Width
                    && y < window.ScreenY + window.Height)
                {
                    return window;
                }
            }
            return null;
        }

        private void DispatchEvents()
        {
            Window window = GetWindowAtPos(MouseManager.X, MouseManager.Y);

            if (window != null)
            {
                int relativeX = (int)(MouseManager.X - window.ScreenX);
                int relativeY = (int)(MouseManager.Y - window.ScreenY);

                if (MouseManager.MouseState == MouseState.Left && lastMouseState == MouseState.None)
                {
                    lastMouseState = MouseManager.MouseState;
                    window.OnDown?.Invoke(relativeX, relativeY);
                }
                else if (MouseManager.MouseState == MouseState.None && lastMouseState == MouseState.Left)
                {
                    lastMouseState = MouseManager.MouseState;

                    if (Focus != null && Focus != window)
                    {
                        Focus.OnUnfocused?.Invoke();
                    }
                    Focus = window;
                    window.OnFocused?.Invoke();

                    window.OnClick?.Invoke(relativeX, relativeY);

                    if (DateTime.Now - lastClickDate < doubleClickTimeSpan)
                    {
                        window.OnDoubleClick?.Invoke(relativeX, relativeY);
                    }
                    lastClickDate = DateTime.Now;
                }
            }

            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                // To-do: Move this out of WindowManager.
                if (key.Key == ConsoleKeyEx.LWin || key.Key == ConsoleKeyEx.RWin)
                {
                    StartMenu.CurrentStartMenu?.ToggleStartMenu(focusSearch: true);
                    return;
                }
                Focus?.OnKeyPressed?.Invoke(key);
            }
        }

        private void UpdateFps()
        {
            framesThisSecond++;
            int second = DateTime.Now.Second;
            if (second != lastSecond)
            {
                fps = framesThisSecond;
                framesThisSecond = 0;

                if (fpsShown)
                {
                    fpsCounter.Clear(Color.Black);
                    fpsCounter.DrawString($"{fps.ToString()} FPS", Color.White, 0, 0);
                    Update(fpsCounter);
                }
            }
            lastSecond = second;
        }

        internal void HideFps()
        {
            fpsShown = false;
            RemoveWindow(fpsCounter);
        }

        internal void ShowFps()
        {
            fpsShown = true;
            AddWindow(fpsCounter);
            Update(fpsCounter);
        }

        private void RenderWallpaper()
        {
            driver.VideoMemory.Copy((int)driver.FrameSize, wallpaperResized.RawData, 0, wallpaperResized.RawData.Length);
        }

        private void SetupWallpaper()
        {
            wallpaperResized = wallpaperBitmap.Resize(ScreenWidth, ScreenHeight);
        }

        private void UpdateCursor()
        {
            uint mouseX = MouseManager.X;
            uint mouseY = MouseManager.Y;
            if (mouseX != lastMouseX || mouseY != lastMouseY)
            {
                driver.SetCursor(true, mouseX, mouseY);
            }
            lastMouseX = mouseX;
            lastMouseY = mouseY;
        }

        private void Sweep()
        {
            if (sweepCounter == 10)
            {
                sweepCounter = 0;
                foreach (Window window in Windows)
                {
                    if (window.Process != null && !window.Process.IsRunning)
                    {
                        RemoveWindow(window);
                    }
                }
            }
            sweepCounter++;
        }

        #region Process
        public override void Start()
        {
            base.Start();

            Logger.Logger.Instance.Info("WindowManager", "Starting WindowManager service");

            SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

            ScreenWidth = (uint)settingsService.Mode.Width;
            ScreenHeight = (uint)settingsService.Mode.Height;
            bytesPerPixel = 4;

            Logger.Logger.Instance.Info("WindowManager", $"Screen resolution: {ScreenWidth}x{ScreenHeight}");

            SetupDriver();
            SetupMouse();
            SetupWallpaper();
            RenderWallpaper();

            Logger.Logger.Instance.Info("WindowManager", "Driver, mouse, and wallpaper initialized");

            fpsCounter = new Window(this, (int)(ScreenWidth) - 64, (int)(ScreenHeight - 16), 64, 16);
            if (SettingsManager.GUI_ShowFps)
            {
                AddWindow(fpsCounter);
            }

            Logger.Logger.Instance.Success("WindowManager", "WindowManager started successfully");
        }

        public override void Run()
        {
            UpdateFps();

            Sweep();

            if (UpdateQueue.Count > 0)
            {
                Window toUpdate = UpdateQueue.Dequeue();

                if (Windows.Contains(toUpdate))
                {
                    if (toUpdate is UILib.Control control)
                    {
                        control.Render();
                    }
                    RenderWindow(toUpdate);
                }
            }

            DispatchEvents();

            if (bufferModified)
            {
                driver.DoubleBufferUpdate();
            }

            UpdateCursor();
        }

        public override void Stop()
        {
            base.Stop();

            driver.Disable();
        }
        #endregion
    }
}
