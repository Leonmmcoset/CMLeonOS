using System.Collections.Generic;

namespace CMLeonOS.Gui.UILib
{
    internal class TreeNode
    {
        internal TreeNode(string text, object tag = null)
        {
            Text = text;
            Tag = tag;
        }

        internal string Text { get; set; }
        internal object Tag { get; set; }
        internal bool Expanded { get; set; } = false;
        internal int AnimatedChildCount { get; set; } = 0;
        internal bool ExpandingAnimation { get; set; } = false;
        internal bool CollapsingAnimation { get; set; } = false;
        internal List<TreeNode> Children { get; } = new List<TreeNode>();
    }
}
