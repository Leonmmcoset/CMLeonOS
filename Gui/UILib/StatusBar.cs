using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class StatusBar : Control
    {
        public StatusBar(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private string _text = "Ready";
        internal string Text
        {
            get { return _text; }
            set { _text = value ?? string.Empty; Render(); }
        }

        private string _detailText = string.Empty;
        internal string DetailText
        {
            get { return _detailText; }
            set { _detailText = value ?? string.Empty; Render(); }
        }

        private Color _background = UITheme.SurfaceMuted;
        internal Color Background
        {
            get { return _background; }
            set { _background = value; Render(); }
        }

        private Color _foreground = UITheme.TextPrimary;
        internal Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; Render(); }
        }

        private Color _border = UITheme.SurfaceBorder;
        internal Color Border
        {
            get { return _border; }
            set { _border = value; Render(); }
        }

        internal override void Render()
        {
            Clear(Background);
            DrawFilledRectangle(0, 0, Width, Height, Background);
            DrawHorizontalLine(Width, 0, 0, Border);

            DrawString(Text, Foreground, 6, (Height / 2) - 8);

            if (!string.IsNullOrWhiteSpace(DetailText))
            {
                DrawString(DetailText, Foreground, Width - (DetailText.Length * 8) - 6, (Height / 2) - 8);
            }

            WM.Update(this);
        }
    }
}
