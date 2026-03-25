using Cosmos.System;
using System;

namespace CMLeonOS.Gui.Apps.Paint.Tools
{
    internal class RectangleTool : Tool
    {
        public RectangleTool() : base("Rectangle")
        {
        }

        private bool started;
        private int startX;
        private int startY;

        internal override void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY)
        {
            if (mouseState == MouseState.Left)
            {
                if (!started)
                {
                    started = true;
                    startX = mouseX;
                    startY = mouseY;
                }
            }
            else if (started)
            {
                int x = Math.Min(startX, mouseX);
                int y = Math.Min(startY, mouseY);
                int width = Math.Abs(mouseX - startX) + 1;
                int height = Math.Abs(mouseY - startY) + 1;
                canvas.DrawRectangle(x, y, width, height, paint.SelectedColor);
                started = false;
            }
        }

        internal override void Deselected()
        {
            started = false;
        }
    }
}
