using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class ToolTip
    {
        internal ToolTip(Process process, WindowManager wm)
        {
            this.process = process;
            this.wm = wm;
        }

        private readonly Process process;
        private readonly WindowManager wm;
        private Window window;

        internal void Show(Window anchor, string text, int offsetX = 0, int offsetY = 0)
        {
            Hide();

            string message = text ?? string.Empty;
            int width = Math.Max(80, (message.Length * 8) + 12);
            int height = 24;

            int x = anchor.ScreenX + offsetX;
            int y = anchor.ScreenY + anchor.Height + 6 + offsetY;

            if (x + width > wm.ScreenWidth)
            {
                x = (int)wm.ScreenWidth - width - 6;
            }
            if (y + height > wm.ScreenHeight)
            {
                y = anchor.ScreenY - height - 6;
            }

            window = new Window(process, x, y, width, height);
            window.Clear(Color.FromArgb(33, 39, 49));
            window.DrawFilledRectangle(0, 0, width, height, Color.FromArgb(33, 39, 49));
            window.DrawRectangle(0, 0, width, height, UITheme.SurfaceBorder);
            window.DrawString(message, Color.White, 6, 4);
            wm.AddWindow(window);
            wm.Update(window);
        }

        internal void Hide()
        {
            if (window != null)
            {
                wm.RemoveWindow(window);
                window = null;
            }
        }
    }
}
