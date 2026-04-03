using System;

namespace CMLeonOS.OSGraphicDraw
{
    internal struct Vec3
    {
        internal double X;
        internal double Y;
        internal double Z;

        internal Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal static Vec3 RotateX(Vec3 v, double radians)
        {
            double c = Math.Cos(radians);
            double s = Math.Sin(radians);
            return new Vec3(v.X, (v.Y * c) - (v.Z * s), (v.Y * s) + (v.Z * c));
        }

        internal static Vec3 RotateY(Vec3 v, double radians)
        {
            double c = Math.Cos(radians);
            double s = Math.Sin(radians);
            return new Vec3((v.X * c) + (v.Z * s), v.Y, (-v.X * s) + (v.Z * c));
        }

        internal static Vec3 RotateZ(Vec3 v, double radians)
        {
            double c = Math.Cos(radians);
            double s = Math.Sin(radians);
            return new Vec3((v.X * c) - (v.Y * s), (v.X * s) + (v.Y * c), v.Z);
        }
    }
}
