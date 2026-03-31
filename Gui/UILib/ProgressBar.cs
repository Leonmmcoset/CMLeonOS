using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class ProgressBar : Control
    {
        public ProgressBar(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private float _minimum = 0f;
        internal float Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                if (_maximum < _minimum) _maximum = _minimum;
                if (_value < _minimum) _value = _minimum;
                Render();
            }
        }

        private float _maximum = 100f;
        internal float Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = Math.Max(value, _minimum);
                if (_value > _maximum) _value = _maximum;
                Render();
            }
        }

        private float _value = 0f;
        internal float Value
        {
            get { return _value; }
            set
            {
                float clamped = Math.Max(_minimum, Math.Min(_maximum, value));
                if (Math.Abs(clamped - _value) < 0.0001f) return;
                _value = clamped;
                Render();
            }
        }

        private Color _background = UITheme.SurfaceMuted;
        internal Color Background
        {
            get { return _background; }
            set { _background = value; Render(); }
        }

        private Color _foreground = UITheme.Accent;
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

        internal bool ShowPercentageText { get; set; } = true;

        internal float GetPercent()
        {
            float range = _maximum - _minimum;
            if (range <= 0.0001f) return 1f;
            return (_value - _minimum) / range;
        }

        internal override void Render()
        {
            Clear(UITheme.Surface);

            int innerW = Math.Max(0, Width - 2);
            int innerH = Math.Max(0, Height - 2);
            DrawFilledRectangle(1, 1, innerW, innerH, Background);

            float percent = Math.Max(0f, Math.Min(1f, GetPercent()));
            int fillWidth = (int)(innerW * percent);
            if (fillWidth > 0)
            {
                DrawFilledRectangle(1, 1, fillWidth, innerH, Foreground);
                DrawHorizontalLine(Math.Max(0, fillWidth - 1), 1, 1, Color.FromArgb(
                    Math.Min(255, Foreground.R + 20),
                    Math.Min(255, Foreground.G + 20),
                    Math.Min(255, Foreground.B + 20)));
            }

            DrawRectangle(0, 0, Width, Height, Border);

            if (ShowPercentageText)
            {
                string text = ((int)(percent * 100f)).ToString() + "%";
                int textX = (Width / 2) - (text.Length * 4);
                int textY = (Height / 2) - 8;
                Color textColor = percent > 0.45f ? Color.White : UITheme.TextPrimary;
                DrawString(text, textColor, textX, textY);
            }

            WM.Update(this);
        }
    }
}
