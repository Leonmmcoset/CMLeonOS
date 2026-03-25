using Cosmos.System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Paint.Tools
{
    internal class Eraser : Tool
    {
        public Eraser() : base("Eraser")
        {
        }

        private bool joinLine;
        private int joinX;
        private int joinY;

        internal override void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY)
        {
            if (mouseState == MouseState.Left)
            {
                if (joinLine)
                {
                    canvas.DrawLine(joinX, joinY, mouseX, mouseY, Color.White);
                }

                for (int y = -4; y <= 4; y++)
                {
                    for (int x = -4; x <= 4; x++)
                    {
                        int drawX = mouseX + x;
                        int drawY = mouseY + y;
                        if (paint.IsInBounds(drawX, drawY))
                        {
                            canvas.DrawPoint(drawX, drawY, Color.White);
                        }
                    }
                }

                joinLine = true;
                joinX = mouseX;
                joinY = mouseY;
            }
            else
            {
                joinLine = false;
            }
        }

        internal override void Deselected()
        {
            joinLine = false;
        }
    }
}
