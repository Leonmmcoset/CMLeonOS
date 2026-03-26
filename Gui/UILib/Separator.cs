using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Separator : Control
    {
        internal enum SeparatorOrientation
        {
            Horizontal,
            Vertical
        }

        public Separator(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private Color _background = UITheme.Surface;
        internal Color Background
        {
            get { return _background; }
            set { _background = value; Render(); }
        }

        private Color _foreground = UITheme.SurfaceBorder;
        internal Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; Render(); }
        }

        internal SeparatorOrientation Orientation { get; set; } = SeparatorOrientation.Horizontal;

        internal override void Render()
        {
            Clear(Background);

            if (Orientation == SeparatorOrientation.Horizontal)
            {
                int y = Height / 2;
                DrawHorizontalLine(Width, 0, y, Foreground);
            }
            else
            {
                int x = Width / 2;
                DrawVerticalLine(Height, x, 0, Foreground);
            }

            WM.Update(this);
        }
    }
}
