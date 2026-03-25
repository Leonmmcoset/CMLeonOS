using Cosmos.System;
using System;

namespace CMLeonOS.Gui.Apps.Paint.Tools
{
    internal class FilledCircleTool : Tool
    {
        public FilledCircleTool() : base("Filled Circle")
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
                int radiusX = Math.Abs(mouseX - startX);
                int radiusY = Math.Abs(mouseY - startY);
                int radius = Math.Max(radiusX, radiusY);

                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        if ((x * x) + (y * y) <= radius * radius)
                        {
                            int drawX = startX + x;
                            int drawY = startY + y;
                            if (paint.IsInBounds(drawX, drawY))
                            {
                                canvas.DrawPoint(drawX, drawY, paint.SelectedColor);
                            }
                        }
                    }
                }

                started = false;
            }
        }

        internal override void Deselected()
        {
            started = false;
        }
    }
}
