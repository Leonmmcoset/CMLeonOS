using Cosmos.System;

namespace CMLeonOS.Gui.Apps.Paint.Tools
{
    internal class LineTool : Tool
    {
        public LineTool() : base("Line")
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
                canvas.DrawLine(startX, startY, mouseX, mouseY, paint.SelectedColor);
                started = false;
            }
        }

        internal override void Deselected()
        {
            started = false;
        }
    }
}
