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
    }
}
