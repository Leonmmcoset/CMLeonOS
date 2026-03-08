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

using CMLeonOS.Gui;
using CMLeonOS.Gui.UILib;
using Cosmos.System.Graphics;
using System.Drawing;
using UniLua;
using System;

namespace CMLeonOS.Gui.Apps.CodeStudio
{
    internal class LuaGuiLibrary
    {
        private static WindowManager? wm = null;
        private static Process? process = null;

        public static void Initialize(WindowManager windowManager, Process currentProcess)
        {
            wm = windowManager;
            process = currentProcess;
        }

        public static void Register(ILuaState lua)
        {
            if (wm == null || process == null)
            {
                throw new Exception("GUI library not initialized. Make sure you are running in GUI mode.");
            }

            lua.NewTable();

            lua.PushString("createWindow");
            lua.PushCSharpFunction(CreateWindow);
            lua.SetTable(-3);

            lua.PushString("showMessageBox");
            lua.PushCSharpFunction(ShowMessageBox);
            lua.SetTable(-3);

            lua.PushString("closeWindow");
            lua.PushCSharpFunction(CloseWindow);
            lua.SetTable(-3);

            lua.PushString("setWindowTitle");
            lua.PushCSharpFunction(SetWindowTitle);
            lua.SetTable(-3);

            lua.PushString("setWindowSize");
            lua.PushCSharpFunction(SetWindowSize);
            lua.SetTable(-3);

            lua.PushString("setWindowPosition");
            lua.PushCSharpFunction(SetWindowPosition);
            lua.SetTable(-3);

            lua.PushString("clearWindow");
            lua.PushCSharpFunction(ClearWindow);
            lua.SetTable(-3);

            lua.PushString("drawText");
            lua.PushCSharpFunction(DrawText);
            lua.SetTable(-3);

            lua.PushString("drawRectangle");
            lua.PushCSharpFunction(DrawRectangle);
            lua.SetTable(-3);

            lua.PushString("drawFilledRectangle");
            lua.PushCSharpFunction(DrawFilledRectangle);
            lua.SetTable(-3);

            lua.PushString("drawCircle");
            lua.PushCSharpFunction(DrawCircle);
            lua.SetTable(-3);

            lua.PushString("drawLine");
            lua.PushCSharpFunction(DrawLine);
            lua.SetTable(-3);

            lua.PushString("drawPoint");
            lua.PushCSharpFunction(DrawPoint);
            lua.SetTable(-3);

            lua.PushString("updateWindow");
            lua.PushCSharpFunction(UpdateWindow);
            lua.SetTable(-3);

            lua.SetGlobal("gui");
        }

        private static int CreateWindow(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            string title = L.ToString(1);
            int width = (int)L.ToNumber(2);
            int height = (int)L.ToNumber(3);

            int x = (int)((wm.ScreenWidth - width) / 2);
            int y = (int)((wm.ScreenHeight - height) / 2);

            AppWindow window = new AppWindow(process, x, y, width, height);
            window.Title = title;
            wm.AddWindow(window);

            L.PushLightUserData(window);
            return 1;
        }

        private static int ShowMessageBox(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            string title = L.ToString(1);
            string message = L.ToString(2);

            MessageBox messageBox = new MessageBox(process, title, message);
            messageBox.Show();

            return 0;
        }

        private static int CloseWindow(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            if (window != null)
            {
                wm.RemoveWindow(window);
            }

            return 0;
        }

        private static int SetWindowTitle(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            string title = L.ToString(2);

            if (window != null)
            {
                window.Title = title;
                wm.Update(window);
            }

            return 0;
        }

        private static int SetWindowSize(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int width = (int)L.ToNumber(2);
            int height = (int)L.ToNumber(3);

            if (window != null)
            {
                window.Resize(width, height);
            }

            return 0;
        }

        private static int SetWindowPosition(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x = (int)L.ToNumber(2);
            int y = (int)L.ToNumber(3);

            if (window != null)
            {
                window.Move(x, y);
            }

            return 0;
        }

        private static int ClearWindow(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            Color color = ParseColor(L, 2);

            if (window != null)
            {
                window.Clear(color);
            }

            return 0;
        }

        private static int DrawText(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            string text = L.ToString(2);
            Color color = ParseColor(L, 3);
            int x = (int)L.ToNumber(4);
            int y = (int)L.ToNumber(5);

            if (window != null)
            {
                window.DrawString(text, color, x, y);
            }

            return 0;
        }

        private static int DrawRectangle(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x = (int)L.ToNumber(2);
            int y = (int)L.ToNumber(3);
            int width = (int)L.ToNumber(4);
            int height = (int)L.ToNumber(5);
            Color color = ParseColor(L, 6);

            if (window != null)
            {
                window.DrawRectangle(x, y, width, height, color);
            }

            return 0;
        }

        private static int DrawFilledRectangle(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x = (int)L.ToNumber(2);
            int y = (int)L.ToNumber(3);
            int width = (int)L.ToNumber(4);
            int height = (int)L.ToNumber(5);
            Color color = ParseColor(L, 6);

            if (window != null)
            {
                window.DrawFilledRectangle(x, y, width, height, color);
            }

            return 0;
        }

        private static int DrawCircle(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x = (int)L.ToNumber(2);
            int y = (int)L.ToNumber(3);
            int radius = (int)L.ToNumber(4);
            Color color = ParseColor(L, 5);

            if (window != null)
            {
                window.DrawCircle(x, y, radius, color);
            }

            return 0;
        }

        private static int DrawLine(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x1 = (int)L.ToNumber(2);
            int y1 = (int)L.ToNumber(3);
            int x2 = (int)L.ToNumber(4);
            int y2 = (int)L.ToNumber(5);
            Color color = ParseColor(L, 6);

            if (window != null)
            {
                window.DrawLine(x1, y1, x2, y2, color);
            }

            return 0;
        }

        private static int DrawPoint(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;
            int x = (int)L.ToNumber(2);
            int y = (int)L.ToNumber(3);
            Color color = ParseColor(L, 4);

            if (window != null)
            {
                window.DrawPoint(x, y, color);
            }

            return 0;
        }

        private static int UpdateWindow(ILuaState L)
        {
            if (wm == null || process == null)
            {
                L.PushString("GUI library not initialized");
                L.Error();
                return 1;
            }

            AppWindow window = L.ToUserData(1) as AppWindow;

            if (window != null)
            {
                wm.Update(window);
            }

            return 0;
        }

        private static Color ParseColor(ILuaState L, int index)
        {
            if (L.IsTable(index))
            {
                L.GetField(index, "r");
                int r = (int)L.ToNumber(-1);
                L.Pop(1);

                L.GetField(index, "g");
                int g = (int)L.ToNumber(-1);
                L.Pop(1);

                L.GetField(index, "b");
                int b = (int)L.ToNumber(-1);
                L.Pop(1);

                return Color.FromArgb(r, g, b);
            }
            else if (L.IsString(index))
            {
                string colorName = L.ToString(index).ToLower();
                return colorName switch
                {
                    "red" => Color.Red,
                    "green" => Color.Green,
                    "blue" => Color.Blue,
                    "yellow" => Color.Yellow,
                    "orange" => Color.Orange,
                    "purple" => Color.Purple,
                    "pink" => Color.Pink,
                    "black" => Color.Black,
                    "white" => Color.White,
                    "gray" => Color.Gray,
                    "lightgray" => Color.LightGray,
                    "darkgray" => Color.DarkGray,
                    "cyan" => Color.Cyan,
                    "magenta" => Color.Magenta,
                    "lime" => Color.Lime,
                    "brown" => Color.Brown,
                    _ => Color.White
                };
            }
            else
            {
                return Color.White;
            }
        }
    }
}
