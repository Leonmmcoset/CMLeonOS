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

using Cosmos.System.Graphics;
using System;

namespace CMLeonOS.Gui
{
    internal static class BitmapExtensions
    {
        internal static Bitmap Resize(this Bitmap bmp, uint width, uint height)
        {
            if (bmp.Width == width && bmp.Height == height)
            {
                return bmp;
            }

            if (bmp.Depth != ColorDepth.ColorDepth32)
            {
                throw new Exception("Resize can only resize images with a colour depth of 32.");
            }

            Bitmap res = new Bitmap(width, height, ColorDepth.ColorDepth32);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double xDouble = (double)x / (double)width;
                    double yDouble = (double)y / (double)height;

                    uint origX = (uint)((double)bmp.Width * xDouble);
                    uint origY = (uint)((double)bmp.Height * yDouble);

                    res.RawData[y * width + x] = bmp.RawData[(origY * bmp.Width) + origX];
                }
            }

            return res;
        }

        internal static Bitmap ResizeWidthKeepRatio(this Bitmap bmp, uint width)
        {
            return Resize(bmp, width, (uint)((double)bmp.Height * ((double)width / (double)bmp.Width)));
        }

        internal static Bitmap ResizeHeightKeepRatio(this Bitmap bmp, uint height)
        {
            return Resize(bmp, (uint)((double)bmp.Width * ((double)height / (double)bmp.Height)), height);
        }
    }
}
