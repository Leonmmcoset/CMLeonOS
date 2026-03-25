using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class NumericUpDown : Control
    {
        public NumericUpDown(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = NumericUpDownClicked;
        }

        internal Action<int> Changed;

        private Color _background = UITheme.Surface;
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

        private int _minimum = 0;
        internal int Minimum
        {
            get { return _minimum; }
            set { _minimum = value; if (_value < _minimum) { _value = _minimum; } Render(); }
        }

        private int _maximum = 100;
        internal int Maximum
        {
            get { return _maximum; }
            set { _maximum = value; if (_value > _maximum) { _value = _maximum; } Render(); }
        }

        private int _step = 1;
        internal int Step
        {
            get { return _step; }
            set { _step = Math.Max(1, value); Render(); }
        }

        private int _value = 0;
        internal int Value
        {
            get { return _value; }
            set
            {
                int newValue = Math.Clamp(value, _minimum, _maximum);
                if (_value != newValue)
                {
                    _value = newValue;
                    Render();
                    Changed?.Invoke(_value);
                }
            }
        }

        private void NumericUpDownClicked(int x, int y)
        {
            int buttonWidth = 22;
            if (x >= Width - buttonWidth)
            {
                if (y < Height / 2)
                {
                    Value += _step;
                }
                else
                {
                    Value -= _step;
                }
            }
        }

        internal override void Render()
        {
            int buttonWidth = 22;

            Clear(Background);
            DrawFilledRectangle(0, 0, Width, Height, Background);
            DrawRectangle(0, 0, Width, Height, Border);

            DrawFilledRectangle(Width - buttonWidth, 0, buttonWidth, Height, UITheme.SurfaceMuted);
            DrawRectangle(Width - buttonWidth, 0, buttonWidth, Height, Border);
            DrawHorizontalLine(buttonWidth, Width - buttonWidth, Height / 2, Border);

            string text = _value.ToString();
            DrawString(text, Foreground, 6, (Height / 2) - 8);

            DrawString("+", Foreground, Width - buttonWidth + 7, 2);
            DrawString("-", Foreground, Width - buttonWidth + 7, (Height / 2) - 2);

            WM.Update(this);
        }
    }
}
