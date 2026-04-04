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
using CMLeonOS.Gui.ShellComponents.Dock;
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.UILib.Animations;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class AppWindow : Window
    {
        internal AppWindow(Process process, int x, int y, int width, int height) : base(process, x, y, width, height)
        {
            wm = ProcessManager.GetProcess<WindowManager>();

            decorationWindow = new Window(process, 0, -titlebarHeight, width, titlebarHeight);
            wm.AddWindow(decorationWindow);

            decorationWindow.RelativeTo = this;

            decorationWindow.OnClick = DecorationClicked;
            decorationWindow.OnDown = DecorationDown;

            Icon = defaultAppIconBitmap;

            RenderDecoration();
            StartOpenAnimation();
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Close.bmp")]
        private static byte[] closeBytes;
        private static Bitmap closeBitmap = new Bitmap(closeBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Maximise.bmp")]
        private static byte[] maximiseBytes;
        private static Bitmap maximiseBitmap = new Bitmap(maximiseBytes);

        /*[IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Minimise.bmp")]
        private static byte[] minimiseBytes;
        private static Bitmap minimiseBitmap = new Bitmap(minimiseBytes);*/

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Restore.bmp")]
        private static byte[] restoreBytes;
        private static Bitmap restoreBitmap = new Bitmap(restoreBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.AppIcons.Default.bmp")]
        private static byte[] defaultAppIconBytes;
        private static Bitmap defaultAppIconBitmap = new Bitmap(defaultAppIconBytes);

        internal Action Closing;

        private Bitmap _icon;
        private Bitmap _smallIcon;
        internal Bitmap Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                _smallIcon = _icon.Resize(20, 20);

                RenderDecoration();

                ProcessManager.GetProcess<Dock>()?.UpdateWindows();
            }
        }

        private string _title = "Window";
        internal string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RenderDecoration();
            }
        }

        private bool _canResize = false;
        internal bool CanResize
        {
            get
            {
                return _canResize;
            }
            set
            {
                _canResize = value;
                RenderDecoration();
            }
        }

        private bool _canClose = true;
        internal bool CanClose
        {
            get
            {
                return _canClose;
            }
            set
            {
                _canClose = value;
                RenderDecoration();
            }
        }

        private bool _canMove = true;
        internal bool CanMove
        {
            get
            {
                return _canMove;
            }
            set
            {
                _canMove = value;
                RenderDecoration();
            }
        }

        private const int titlebarHeight = 24;

        private Window decorationWindow;

        private WindowManager wm;

        private bool maximised = false;
        private bool closeAnimationRunning = false;
        private bool resizeAnimationRunning = false;
        private int originalX;
        private int originalY;
        private int originalWidth;
        private int originalHeight;

        private void RefreshWindowTree()
        {
            wm.Update(this);
            RenderDecoration();

            foreach (Window child in wm.Windows)
            {
                if (child.RelativeTo == this)
                {
                    if (child is Control control)
                    {
                        control.Render();
                    }
                    else
                    {
                        wm.Update(child);
                    }
                }
            }
        }

        private void StartOpenAnimation()
        {
            int targetY = Y;
            int offsetY = Math.Min(18, Math.Max(8, Height / 12));

            Move(X, targetY + offsetY, sendWMEvent: false);

            MovementAnimation animation = new MovementAnimation(this)
            {
                From = new Rectangle(X, Y, Width, Height),
                To = new Rectangle(X, targetY, Width, Height),
                Duration = 12,
                EasingType = EasingType.Sine,
                EasingDirection = EasingDirection.Out
            };
            animation.Completed = RefreshWindowTree;
            animation.Start();
        }

        private void StartCloseAnimation()
        {
            if (closeAnimationRunning || resizeAnimationRunning)
            {
                return;
            }

            closeAnimationRunning = true;

            int offsetY = Math.Min(18, Math.Max(8, Height / 12));
            int startX = X;
            int startY = Y;
            int startWidth = Width;
            int startHeight = Height;

            MovementAnimation animation = new MovementAnimation(this)
            {
                From = new Rectangle(startX, startY, startWidth, startHeight),
                To = new Rectangle(startX, startY + offsetY, startWidth, startHeight),
                Duration = 10,
                EasingType = EasingType.Sine,
                EasingDirection = EasingDirection.In
            };
            animation.Completed = () =>
            {
                Closing?.Invoke();
                wm.RemoveWindow(this);
                closeAnimationRunning = false;
            };
            animation.Start();
        }

        private Rectangle GetMaximizedBounds()
        {
            var taskbar = ProcessManager.GetProcess<ShellComponents.Taskbar>();
            int taskbarHeight = taskbar.GetTaskbarHeight();

            var dock = ProcessManager.GetProcess<ShellComponents.Dock.Dock>();
            int dockHeight = dock.GetDockHeight();

            return new Rectangle(
                0,
                taskbarHeight + titlebarHeight,
                (int)wm.ScreenWidth,
                (int)wm.ScreenHeight - titlebarHeight - taskbarHeight - dockHeight
            );
        }

        private void StartResizeAnimation(Rectangle targetBounds)
        {
            if (resizeAnimationRunning)
            {
                return;
            }

            resizeAnimationRunning = true;

            Rectangle startWindow = new Rectangle(X, Y, Width, Height);
            Rectangle startDecoration = new Rectangle(decorationWindow.X, decorationWindow.Y, decorationWindow.Width, decorationWindow.Height);
            Rectangle targetDecoration = new Rectangle(0, -titlebarHeight, targetBounds.Width, titlebarHeight);

            int completedCount = 0;
            Action finish = () =>
            {
                completedCount++;
                if (completedCount < 2)
                {
                    return;
                }

                MoveAndResize(targetBounds.X, targetBounds.Y, targetBounds.Width, targetBounds.Height, sendWMEvent: false);
                decorationWindow.MoveAndResize(targetDecoration.X, targetDecoration.Y, targetDecoration.Width, targetDecoration.Height, sendWMEvent: false);

                resizeAnimationRunning = false;
                UserResized?.Invoke();
                RefreshWindowTree();
                ProcessManager.GetProcess<WindowManager>().RerenderAll();
            };

            MovementAnimation windowAnimation = new MovementAnimation(this)
            {
                From = startWindow,
                To = targetBounds,
                Duration = 14,
                EasingType = EasingType.Sine,
                EasingDirection = EasingDirection.Out,
                Completed = finish
            };
            windowAnimation.Advanced = () =>
            {
                UserResized?.Invoke();
                RefreshWindowTree();
            };

            MovementAnimation decorationAnimation = new MovementAnimation(decorationWindow)
            {
                From = startDecoration,
                To = targetDecoration,
                Duration = 14,
                EasingType = EasingType.Sine,
                EasingDirection = EasingDirection.Out,
                Completed = finish
            };

            windowAnimation.Start();
            decorationAnimation.Start();
        }

        private void DecorationClicked(int x, int y)
        {
            if (closeAnimationRunning || resizeAnimationRunning)
            {
                return;
            }

            if (x >= Width - titlebarHeight && _canClose)
            {
                // Close.
                StartCloseAnimation();
            }
            else if (x >= Width - (titlebarHeight * (_canClose ? 2 : 1)) && _canResize)
            {
                // Maximise / restore.
                if (maximised)
                {
                    maximised = false;
                    StartResizeAnimation(new Rectangle(originalX, originalY, originalWidth, originalHeight));
                }
                else
                {
                    maximised = true;

                    originalX = X;
                    originalY = Y;
                    originalWidth = Width;
                    originalHeight = Height;
                    StartResizeAnimation(GetMaximizedBounds());
                }
                RenderDecoration();
            }
        }

        private void DecorationDown(int x, int y)
        {
            int buttonSpace = 0;
            if (_canClose)
            {
                buttonSpace += titlebarHeight;
            }
            if (_canResize)
            {
                buttonSpace += titlebarHeight;
            }
            if (x >= Width - buttonSpace || maximised || !_canMove) return;
            if (closeAnimationRunning || resizeAnimationRunning) return;

            uint startMouseX = MouseManager.X;
            uint startMouseY = MouseManager.Y;

            int startWindowX = X;
            int startWindowY = Y;

            while (MouseManager.MouseState == MouseState.Left)
            {
                X = (int)(startWindowX + (MouseManager.X - startMouseX));
                Y = (int)(startWindowY + (MouseManager.Y - startMouseY));

                ProcessManager.Yield();
            }
        }

        private void RenderDecoration()
        {
            decorationWindow.Clear(UITheme.WindowTitleBackground);
            decorationWindow.DrawHorizontalLine(Width, 0, 0, UITheme.WindowTitleTopHighlight);
            decorationWindow.DrawHorizontalLine(Width, 0, titlebarHeight - 1, UITheme.WindowTitleBottomBorder);

            int buttonSpace = 0;
            if (_canClose)
            {
                buttonSpace += titlebarHeight;
            }
            if (_canResize)
            {
                buttonSpace += titlebarHeight;
            }
            if (buttonSpace > 0)
            {
                decorationWindow.DrawFilledRectangle(Width - buttonSpace, 1, buttonSpace, titlebarHeight - 2, UITheme.WindowButtonBackground);
            }

            if (_smallIcon != null)
            {
                decorationWindow.DrawImageAlpha(_smallIcon, 3, 2);
            }

            int maxTitleChars = Math.Max(4, (Width - buttonSpace - 42) / FontData.Width);
            string displayTitle = Title;
            if (displayTitle.Length > maxTitleChars)
            {
                displayTitle = displayTitle.Substring(0, Math.Max(1, maxTitleChars - 1)) + "~";
            }
            decorationWindow.DrawString(displayTitle, UITheme.WindowTitleText, 28, 4);

            if (_canClose)
            {
                decorationWindow.DrawImageAlpha(closeBitmap, Width - titlebarHeight, 0);
            }
            if (_canResize)
            {
                decorationWindow.DrawImageAlpha(maximised ? restoreBitmap : maximiseBitmap, Width - (titlebarHeight * (_canClose ? 2 : 1)), 0);
            }
            wm.Update(decorationWindow);
        }

        internal void RefreshTheme()
        {
            wm.Update(this);
            RenderDecoration();
        }
    }
}
