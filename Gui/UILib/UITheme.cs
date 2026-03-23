// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal static class UITheme
    {
        internal static readonly Color WindowTitleBackground = Color.FromArgb(26, 34, 46);
        internal static readonly Color WindowTitleTopHighlight = Color.FromArgb(52, 67, 90);
        internal static readonly Color WindowTitleBottomBorder = Color.FromArgb(14, 18, 26);
        internal static readonly Color WindowTitleText = Color.FromArgb(239, 244, 252);
        internal static readonly Color WindowButtonBackground = Color.FromArgb(38, 49, 66);

        internal static readonly Color Surface = Color.FromArgb(245, 248, 252);
        internal static readonly Color SurfaceMuted = Color.FromArgb(228, 235, 245);
        internal static readonly Color SurfaceBorder = Color.FromArgb(149, 164, 183);
        internal static readonly Color TextPrimary = Color.FromArgb(33, 43, 56);
        internal static readonly Color TextSecondary = Color.FromArgb(96, 109, 128);

        internal static readonly Color Accent = Color.FromArgb(45, 117, 222);
        internal static readonly Color AccentDark = Color.FromArgb(28, 89, 184);
        internal static readonly Color AccentLight = Color.FromArgb(204, 225, 255);

        internal static readonly Color Success = Color.FromArgb(45, 152, 99);
        internal static readonly Color Warning = Color.FromArgb(210, 139, 54);
    }
}
