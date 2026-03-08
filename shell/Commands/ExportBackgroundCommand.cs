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

using System;
using System.IO;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace CMLeonOS.Commands
{
    public static class ExportBackgroundCommand
    {
        [ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Wallpaper_1280_800.bmp")]
        private static byte[] wallpaperBytes;

        public static void ExportBackground(string outputPath)
        {
            try
            {
                if (wallpaperBytes == null || wallpaperBytes.Length == 0)
                {
                    Console.WriteLine("Error: No wallpaper found in embedded resources.");
                    return;
                }

                string destinationPath = string.IsNullOrEmpty(outputPath) ? @"0:\background.bmp" : outputPath;

                File.WriteAllBytes(destinationPath, wallpaperBytes);
                Console.WriteLine($"Background exported successfully to: {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting background: {ex.Message}");
            }
        }
    }
}