using CMLeonOS.Gui;
using System.Drawing;

namespace CMLeonOS.OSGraphicDraw
{
    internal static class Renderer3D
    {
        internal static bool Project(Vec3 p, int width, int height, double fovScale, double cameraDistance, out int sx, out int sy)
        {
            double z = p.Z + cameraDistance;
            if (z <= 0.05d)
            {
                sx = 0;
                sy = 0;
                return false;
            }

            double inv = fovScale / z;
            sx = (int)((p.X * inv) + (width / 2.0));
            sy = (int)((-p.Y * inv) + (height / 2.0));
            return true;
        }

        internal static void DrawLine3D(Window target, Vec3 a, Vec3 b, Color color, double fovScale, double cameraDistance)
        {
            if (!Project(a, target.Width, target.Height, fovScale, cameraDistance, out int ax, out int ay))
            {
                return;
            }
            if (!Project(b, target.Width, target.Height, fovScale, cameraDistance, out int bx, out int by))
            {
                return;
            }

            target.DrawLine(ax, ay, bx, by, color);
        }

        private static void DrawScanline(Window target, int x1, int x2, int y, Color color)
        {
            if (y < 0 || y >= target.Height) return;
            if (x1 > x2)
            {
                int t = x1;
                x1 = x2;
                x2 = t;
            }

            if (x2 < 0 || x1 >= target.Width) return;
            if (x1 < 0) x1 = 0;
            if (x2 >= target.Width) x2 = target.Width - 1;
            target.DrawHorizontalLine((x2 - x1) + 1, x1, y, color);
        }

        private static void DrawTriangle2D(Window target, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            if (y1 > y2) { int tx = x1; int ty = y1; x1 = x2; y1 = y2; x2 = tx; y2 = ty; }
            if (y1 > y3) { int tx = x1; int ty = y1; x1 = x3; y1 = y3; x3 = tx; y3 = ty; }
            if (y2 > y3) { int tx = x2; int ty = y2; x2 = x3; y2 = y3; x3 = tx; y3 = ty; }

            if (y1 == y3) return;

            for (int y = y1; y <= y3; y++)
            {
                bool lowerHalf = y > y2 || y2 == y1;
                int segmentHeight = lowerHalf ? (y3 - y2) : (y2 - y1);
                if (segmentHeight == 0) continue;

                double alpha = (double)(y - y1) / (y3 - y1);
                double beta = lowerHalf
                    ? (double)(y - y2) / segmentHeight
                    : (double)(y - y1) / segmentHeight;

                int ax = x1 + (int)((x3 - x1) * alpha);
                int bx = lowerHalf
                    ? x2 + (int)((x3 - x2) * beta)
                    : x1 + (int)((x2 - x1) * beta);

                DrawScanline(target, ax, bx, y, color);
            }
        }

        internal static void DrawTriangle3D(Window target, Vec3 a, Vec3 b, Vec3 c, Color color, double fovScale, double cameraDistance)
        {
            if (!Project(a, target.Width, target.Height, fovScale, cameraDistance, out int ax, out int ay)) return;
            if (!Project(b, target.Width, target.Height, fovScale, cameraDistance, out int bx, out int by)) return;
            if (!Project(c, target.Width, target.Height, fovScale, cameraDistance, out int cx, out int cy)) return;

            DrawTriangle2D(target, ax, ay, bx, by, cx, cy, color);
        }
    }
}
