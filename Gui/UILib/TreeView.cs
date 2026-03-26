using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class TreeView : Control
    {
        public TreeView(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = TreeViewClicked;
        }

        internal Action<TreeNode> NodeSelected;

        internal List<TreeNode> Nodes { get; } = new List<TreeNode>();

        private readonly List<TreeNode> visibleNodes = new List<TreeNode>();
        private readonly List<int> visibleLevels = new List<int>();

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

        private Color _selectedBackground = UITheme.AccentLight;
        internal Color SelectedBackground
        {
            get { return _selectedBackground; }
            set { _selectedBackground = value; Render(); }
        }

        internal int RowHeight { get; set; } = 22;

        internal TreeNode SelectedNode { get; private set; }

        private void BuildVisibleNodes()
        {
            visibleNodes.Clear();
            visibleLevels.Clear();

            for (int i = 0; i < Nodes.Count; i++)
            {
                AddVisibleNode(Nodes[i], 0);
            }
        }

        private void AddVisibleNode(TreeNode node, int level)
        {
            visibleNodes.Add(node);
            visibleLevels.Add(level);

            if (node.Expanded)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    AddVisibleNode(node.Children[i], level + 1);
                }
            }
        }

        private void TreeViewClicked(int x, int y)
        {
            BuildVisibleNodes();
            int index = y / RowHeight;
            if (index < 0 || index >= visibleNodes.Count)
            {
                return;
            }

            TreeNode node = visibleNodes[index];
            int level = visibleLevels[index];
            int indentX = 8 + (level * 16);

            if (node.Children.Count > 0 && x >= indentX && x <= indentX + 10)
            {
                node.Expanded = !node.Expanded;
                Render();
                return;
            }

            SelectedNode = node;
            NodeSelected?.Invoke(node);
            Render();
        }

        internal override void Render()
        {
            Clear(Background);
            DrawRectangle(0, 0, Width, Height, Border);

            BuildVisibleNodes();

            int maxRows = Math.Min(visibleNodes.Count, Math.Max(0, Height / RowHeight));
            for (int i = 0; i < maxRows; i++)
            {
                TreeNode node = visibleNodes[i];
                int level = visibleLevels[i];
                int rowY = i * RowHeight;

                if (node == SelectedNode)
                {
                    DrawFilledRectangle(1, rowY + 1, Width - 2, RowHeight - 2, SelectedBackground);
                }

                int indentX = 8 + (level * 16);
                if (node.Children.Count > 0)
                {
                    DrawRectangle(indentX, rowY + 6, 10, 10, Border);
                    DrawHorizontalLine(6, indentX + 2, rowY + 11, Foreground);
                    if (!node.Expanded)
                    {
                        DrawVerticalLine(6, indentX + 5, rowY + 8, Foreground);
                    }
                }

                int textX = indentX + (node.Children.Count > 0 ? 16 : 8);
                DrawString(node.Text, Foreground, textX, rowY + 3);
            }

            WM.Update(this);
        }
    }
}
