using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class ProgressRing : Control
    {
        public ProgressRing(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private Color _background = UITheme.Surface;
        internal Color Background
        {
            get { return _background; }
            set { _background = value; Render(); }
        }

        private Color _ringColor = UITheme.Accent;
        internal Color RingColor
        {
            get { return _ringColor; }
            set { _ringColor = value; Render(); }
        }

        private Color _trackColor = UITheme.SurfaceBorder;
        internal Color TrackColor
        {
            get { return _trackColor; }
            set { _trackColor = value; Render(); }
        }

        private bool _active = true;
        internal bool Active
        {
            get { return _active; }
            set { _active = value; Render(); }
        }

        internal int Thickness { get; set; } = 3;

        private int frame = 0;

        internal override void Render()
        {
            Clear(Background);

            int radius = Math.Max(6, (Math.Min(Width, Height) / 2) - 4);
            int centerX = Width / 2;
            int centerY = Height / 2;

            for (int angle = 0; angle < 360; angle += 12)
            {
                DrawArcPoint(centerX, centerY, radius, angle, TrackColor);
            }

            if (Active)
            {
                for (int i = 0; i < 8; i++)
                {
                    int angle = (frame + (i * 18)) % 360;
                    Color color = Color.FromArgb(
                        Math.Max(60, RingColor.R - (i * 12)),
                        Math.Max(60, RingColor.G - (i * 12)),
                        Math.Max(60, RingColor.B - (i * 12)));
                    DrawArcPoint(centerX, centerY, radius, angle, color);
                }

                frame = (frame + 10) % 360;
                WM.UpdateQueue.Enqueue(this);
            }

            WM.Update(this);
        }

        private void DrawArcPoint(int centerX, int centerY, int radius, int angleDegrees, Color color)
        {
            double radians = angleDegrees * (Math.PI / 180d);
            int pointX = centerX + (int)(Math.Cos(radians) * radius);
            int pointY = centerY + (int)(Math.Sin(radians) * radius);

            for (int y = -Thickness / 2; y <= Thickness / 2; y++)
            {
                for (int x = -Thickness / 2; x <= Thickness / 2; x++)
                {
                    DrawPoint(pointX + x, pointY + y, color);
                }
            }
        }
    }
}
