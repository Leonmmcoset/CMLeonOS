using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Dropdown : Control
    {
        internal Dropdown(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            hostWindow = parent;
            OnClick = (_, _) => ToggleExpanded();
            OnUnfocused = Collapse;
        }

        private readonly Window hostWindow;
        private Window popupWindow;
        private Table popupTable;

        internal Action<int, string> SelectionChanged;

        internal List<string> Items { get; } = new List<string>();

        private string _placeholderText = "Select";
        internal string PlaceholderText
        {
            get
            {
                return _placeholderText;
            }
            set
            {
                _placeholderText = value ?? string.Empty;
                Render();
            }
        }

        private Color _background = UITheme.Surface;
        internal Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Render();
            }
        }

        private Color _foreground = UITheme.TextPrimary;
        internal Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                Render();
            }
        }

        private Color _border = UITheme.SurfaceBorder;
        internal Color Border
        {
            get
            {
                return _border;
            }
            set
            {
                _border = value;
                Render();
            }
        }

        private Color _highlight = UITheme.Accent;
        internal Color Highlight
        {
            get
            {
                return _highlight;
            }
            set
            {
                _highlight = value;
                Render();
            }
        }

        private int _selectedIndex = -1;
        internal int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                int newIndex = value;
                if (newIndex < -1)
                {
                    newIndex = -1;
                }
                if (newIndex >= Items.Count)
                {
                    newIndex = Items.Count - 1;
                }

                if (_selectedIndex != newIndex)
                {
                    _selectedIndex = newIndex;
                    Render();
                    SyncPopupSelection();
                }
            }
        }

        internal string SelectedText
        {
            get
            {
                if (_selectedIndex < 0 || _selectedIndex >= Items.Count)
                {
                    return string.Empty;
                }

                return Items[_selectedIndex];
            }
        }

        internal bool Expanded { get; private set; } = false;

        internal int MaxVisibleItems { get; set; } = 6;

        internal void RefreshItems()
        {
            if (_selectedIndex >= Items.Count)
            {
                _selectedIndex = Items.Count - 1;
            }

            RebuildPopup();
            Render();
        }

        private void ToggleExpanded()
        {
            if (Expanded)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        private void Expand()
        {
            if (Expanded)
            {
                return;
            }

            Expanded = true;
            RebuildPopup();
            Render();
        }

        internal void Collapse()
        {
            if (!Expanded)
            {
                return;
            }

            Expanded = false;

            if (popupTable != null)
            {
                WM.RemoveWindow(popupTable);
                popupTable = null;
            }
            if (popupWindow != null)
            {
                WM.RemoveWindow(popupWindow);
                popupWindow = null;
            }

            Render();
        }

        private void PopupSelected(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return;
            }

            _selectedIndex = index;
            Collapse();
            Render();
            SelectionChanged?.Invoke(_selectedIndex, Items[_selectedIndex]);
        }

        private void SyncPopupSelection()
        {
            if (popupTable != null)
            {
                popupTable.SelectedCellIndex = _selectedIndex;
            }
        }

        private void RebuildPopup()
        {
            if (!Expanded)
            {
                return;
            }

            if (popupTable != null)
            {
                WM.RemoveWindow(popupTable);
                popupTable = null;
            }
            if (popupWindow != null)
            {
                WM.RemoveWindow(popupWindow);
                popupWindow = null;
            }

            int visibleItems = Math.Max(1, Math.Min(MaxVisibleItems, Math.Max(1, Items.Count)));
            int popupHeight = Math.Max(24, visibleItems * 22);

            popupWindow = new Window(Process, hostWindow, X, Y + Height, Width, popupHeight);
            popupWindow.Clear(UITheme.Surface);
            popupWindow.DrawRectangle(0, 0, popupWindow.Width, popupWindow.Height, Border);
            WM.AddWindow(popupWindow);

            popupTable = new Table(popupWindow, 0, 0, popupWindow.Width, popupWindow.Height);
            popupTable.AllowDeselection = false;
            popupTable.CellHeight = 22;
            popupTable.Background = UITheme.Surface;
            popupTable.Foreground = Foreground;
            popupTable.Border = Border;
            popupTable.SelectedBackground = UITheme.AccentLight;
            popupTable.SelectedBorder = Highlight;
            popupTable.SelectedForeground = Foreground;
            popupTable.TableCellSelected = PopupSelected;

            for (int i = 0; i < Items.Count; i++)
            {
                popupTable.Cells.Add(new TableCell(Items[i], i));
            }

            popupTable.SelectedCellIndex = _selectedIndex;
            popupTable.Render();
            WM.AddWindow(popupTable);
        }

        internal override void Render()
        {
            Clear(Background);
            DrawFilledRectangle(0, 0, Width, Height, Background);

            Color frame = Expanded ? Highlight : Border;
            DrawRectangle(0, 0, Width, Height, frame);

            string displayText = SelectedIndex >= 0 ? SelectedText : PlaceholderText;
            Color textColor = SelectedIndex >= 0 ? Foreground : UITheme.TextSecondary;

            int maxChars = Math.Max(1, (Width - 24) / 8);
            if (displayText.Length > maxChars)
            {
                displayText = displayText.Substring(0, Math.Max(1, maxChars - 1)) + "~";
            }

            DrawString(displayText, textColor, 6, (Height / 2) - 8);

            int arrowCenterX = Width - 12;
            int arrowCenterY = Height / 2;
            DrawLine(arrowCenterX - 4, arrowCenterY - 2, arrowCenterX, arrowCenterY + 2, textColor);
            DrawLine(arrowCenterX, arrowCenterY + 2, arrowCenterX + 4, arrowCenterY - 2, textColor);

            if (Expanded && popupWindow != null)
            {
                popupWindow.MoveAndResize(X, Y + Height, Width, popupWindow.Height, sendWMEvent: false);
                if (popupTable != null)
                {
                    popupTable.MoveAndResize(0, 0, popupWindow.Width, popupWindow.Height, sendWMEvent: false);
                    popupTable.Render();
                }
                popupWindow.Clear(UITheme.Surface);
                popupWindow.DrawRectangle(0, 0, popupWindow.Width, popupWindow.Height, Border);
                WM.Update(popupWindow);
            }

            WM.Update(this);
        }
    }
}
