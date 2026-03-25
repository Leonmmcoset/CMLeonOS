using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Tabs : Control
    {
        public Tabs(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = TabsClicked;
        }

        internal Action<int, string> SelectionChanged;

        internal List<string> Items { get; } = new List<string>();

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

        private int _selectedIndex = 0;
        internal int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                int newIndex = Math.Clamp(value, 0, Math.Max(0, Items.Count - 1));
                if (_selectedIndex != newIndex)
                {
                    _selectedIndex = newIndex;
                    Render();
                    if (Items.Count > 0)
                    {
                        SelectionChanged?.Invoke(_selectedIndex, Items[_selectedIndex]);
                    }
                }
            }
        }

        internal string SelectedText
        {
            get
            {
                if (Items.Count == 0 || _selectedIndex < 0 || _selectedIndex >= Items.Count)
                {
                    return string.Empty;
                }

                return Items[_selectedIndex];
            }
        }

        internal void RefreshItems()
        {
            if (_selectedIndex >= Items.Count)
            {
                _selectedIndex = Math.Max(0, Items.Count - 1);
            }
            Render();
        }

        private void TabsClicked(int x, int y)
        {
            if (Items.Count == 0)
            {
                return;
            }

            int tabWidth = Math.Max(1, Width / Items.Count);
            int index = Math.Min(Items.Count - 1, Math.Max(0, x / tabWidth));
            SelectedIndex = index;
        }

        internal override void Render()
        {
            Clear(Background);
            DrawFilledRectangle(0, 0, Width, Height, Background);
            DrawRectangle(0, 0, Width, Height, Border);

            if (Items.Count == 0)
            {
                WM.Update(this);
                return;
            }

            int tabWidth = Math.Max(1, Width / Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                int tabX = i * tabWidth;
                int actualWidth = i == Items.Count - 1 ? Width - tabX : tabWidth;
                bool selected = i == _selectedIndex;

                DrawFilledRectangle(tabX, 0, actualWidth, Height, selected ? UITheme.AccentLight : UITheme.SurfaceMuted);
                DrawRectangle(tabX, 0, actualWidth, Height, selected ? UITheme.Accent : Border);

                string text = Items[i];
                int maxChars = Math.Max(1, (actualWidth - 8) / 8);
                if (text.Length > maxChars)
                {
                    text = text.Substring(0, Math.Max(1, maxChars - 1)) + "~";
                }

                DrawString(text, Foreground, tabX + (actualWidth / 2) - (text.Length * 4), (Height / 2) - 8);
            }

            WM.Update(this);
        }
    }
}
