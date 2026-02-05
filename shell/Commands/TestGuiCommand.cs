using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.Drawing;
using Cosmos.System.Graphics;
using Microsoft.VisualBasic;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System;
using Cosmos.HAL;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.IO;

namespace CMLeonOS.Commands
{
    public static class TestGuiCommand
    {
        public static Canvas Screen;
        public static Bitmap Wallpaper;
        public static Mode display;

        public static void DrawCursor()
        {
            if ((int)MouseManager.X >= 0 && (int)MouseManager.Y >= 0 && (int)MouseManager.X < Screen.Mode.Width && (int)MouseManager.Y < Screen.Mode.Height)
            {
                MouseManager.ScreenWidth = Screen.Mode.Width;
                MouseManager.ScreenHeight = Screen.Mode.Height;
                Screen.DrawFilledCircle(Color.FromArgb(75, 255, 255, 255), (int)MouseManager.X, (int)MouseManager.Y, 10);
            }
        }

        public static void DrawBackground()
        {
            Screen.Clear(Color.Indigo);
        }

        public static void RunTestGui()
        {
            try
            {
                display = new Mode(1024, 768, ColorDepth.ColorDepth24);
                Screen = FullScreenCanvas.GetFullScreenCanvas(display);

                try
                {
                    if (File.Exists(@"0:\system\wallpaper.bmp"))
                    {
                        Wallpaper = new Bitmap(File.ReadAllBytes(@"0:\system\wallpaper.bmp"));
                    }
                    else
                    {
                        Wallpaper = null;
                    }
                }
                catch
                {
                    Wallpaper = null;
                }

                global::System.Console.WriteLine("Starting graphical mode...");
                global::System.Console.WriteLine("Press ESC to return to shell.");

                while (true)
                {
                    Screen.Clear();
                    DrawBackground();
                    Screen.DrawString(DateTime.Now.ToString("H:mm") + ", Screen:" + Screen.Mode.Width + "x" + Screen.Mode.Height, PCScreenFont.Default, Color.White, 15, 10);
                    DrawCursor();
                    Screen.Display();

                    var key = global::System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                global::System.Console.WriteLine("Exception occurred: " + e.Message);
            }
        }
    }
}
