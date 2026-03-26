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
        internal static Color WindowTitleBackground { get; private set; }
        internal static Color WindowTitleTopHighlight { get; private set; }
        internal static Color WindowTitleBottomBorder { get; private set; }
        internal static Color WindowTitleText { get; private set; }
        internal static Color WindowButtonBackground { get; private set; }

        internal static Color Surface { get; private set; }
        internal static Color SurfaceMuted { get; private set; }
        internal static Color SurfaceBorder { get; private set; }
        internal static Color TextPrimary { get; private set; }
        internal static Color TextSecondary { get; private set; }

        internal static Color Accent { get; private set; }
        internal static Color AccentDark { get; private set; }
        internal static Color AccentLight { get; private set; }

        internal static Color Success { get; private set; }
        internal static Color Warning { get; private set; }

        internal static string CurrentThemeName { get; private set; } = "Default";

        internal static string[] GetThemeNames()
        {
            return new string[] { "Default", "Graphite", "Forest" };
        }

        internal static void ApplyTheme(string themeName)
        {
            string name = string.IsNullOrWhiteSpace(themeName) ? "Default" : themeName.Trim();

            switch (name)
            {
                case "Graphite":
                    WindowTitleBackground = Color.FromArgb(31, 34, 40);
                    WindowTitleTopHighlight = Color.FromArgb(62, 69, 81);
                    WindowTitleBottomBorder = Color.FromArgb(18, 20, 24);
                    WindowTitleText = Color.FromArgb(239, 242, 247);
                    WindowButtonBackground = Color.FromArgb(47, 53, 61);

                    Surface = Color.FromArgb(236, 239, 244);
                    SurfaceMuted = Color.FromArgb(214, 220, 228);
                    SurfaceBorder = Color.FromArgb(124, 134, 149);
                    TextPrimary = Color.FromArgb(36, 42, 52);
                    TextSecondary = Color.FromArgb(92, 101, 115);

                    Accent = Color.FromArgb(90, 131, 212);
                    AccentDark = Color.FromArgb(64, 100, 176);
                    AccentLight = Color.FromArgb(205, 220, 247);

                    Success = Color.FromArgb(61, 145, 108);
                    Warning = Color.FromArgb(196, 138, 60);
                    CurrentThemeName = "Graphite";
                    break;

                case "Forest":
                    WindowTitleBackground = Color.FromArgb(24, 48, 36);
                    WindowTitleTopHighlight = Color.FromArgb(60, 98, 77);
                    WindowTitleBottomBorder = Color.FromArgb(13, 28, 20);
                    WindowTitleText = Color.FromArgb(240, 248, 243);
                    WindowButtonBackground = Color.FromArgb(38, 67, 52);

                    Surface = Color.FromArgb(242, 248, 244);
                    SurfaceMuted = Color.FromArgb(220, 232, 224);
                    SurfaceBorder = Color.FromArgb(134, 160, 145);
                    TextPrimary = Color.FromArgb(33, 53, 41);
                    TextSecondary = Color.FromArgb(95, 116, 104);

                    Accent = Color.FromArgb(53, 144, 98);
                    AccentDark = Color.FromArgb(37, 112, 74);
                    AccentLight = Color.FromArgb(202, 236, 217);

                    Success = Color.FromArgb(42, 153, 92);
                    Warning = Color.FromArgb(191, 137, 55);
                    CurrentThemeName = "Forest";
                    break;

                default:
                    WindowTitleBackground = Color.FromArgb(26, 34, 46);
                    WindowTitleTopHighlight = Color.FromArgb(52, 67, 90);
                    WindowTitleBottomBorder = Color.FromArgb(14, 18, 26);
                    WindowTitleText = Color.FromArgb(239, 244, 252);
                    WindowButtonBackground = Color.FromArgb(38, 49, 66);

                    Surface = Color.FromArgb(245, 248, 252);
                    SurfaceMuted = Color.FromArgb(228, 235, 245);
                    SurfaceBorder = Color.FromArgb(149, 164, 183);
                    TextPrimary = Color.FromArgb(33, 43, 56);
                    TextSecondary = Color.FromArgb(96, 109, 128);

                    Accent = Color.FromArgb(45, 117, 222);
                    AccentDark = Color.FromArgb(28, 89, 184);
                    AccentLight = Color.FromArgb(204, 225, 255);

                    Success = Color.FromArgb(45, 152, 99);
                    Warning = Color.FromArgb(210, 139, 54);
                    CurrentThemeName = "Default";
                    break;
            }
        }

        internal static void RefreshOpenWindows(CMLeonOS.Gui.WindowManager wm)
        {
            if (wm == null)
            {
                return;
            }

            for (int i = 0; i < wm.Windows.Count; i++)
            {
                CMLeonOS.Gui.Window window = wm.Windows[i];
                if (window is AppWindow appWindow)
                {
                    appWindow.RefreshTheme();
                }
                else if (window is Control control)
                {
                    control.Render();
                }
                else
                {
                    wm.Update(window);
                }
            }

            wm.RerenderAll();
        }

        static UITheme()
        {
            ApplyTheme("Default");
        }
    }
}
