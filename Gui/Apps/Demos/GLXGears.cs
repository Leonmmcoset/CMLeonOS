using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.OSGraphicDraw;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Demos
{
    internal class GLXGears : Process
    {
        internal GLXGears() : base("GLXGears", ProcessType.Application) { }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private AppWindow window;

        private double redAngle = 0d;
        private double greenAngle = 0d;
        private double blueAngle = 0d;

        private int fps = 0;
        private int frames = 0;
        private int lastSecond = -1;

        private const double ScenePitch = 0.48d;
        private const double SceneYaw = -0.36d;
        private const double FovScale = 260d;
        private const double CameraDistance = 10.5d;

        private Vec3 TransformPoint(Vec3 p)
        {
            p = Vec3.RotateX(p, ScenePitch);
            p = Vec3.RotateY(p, SceneYaw);
            return p;
        }

        private void Draw3DLine(Vec3 a, Vec3 b, Color color)
        {
            a = TransformPoint(a);
            b = TransformPoint(b);
            Renderer3D.DrawLine3D(window, a, b, color, FovScale, CameraDistance);
        }

        private void DrawGear(Vec3 center, double outerRadius, double innerRadius, double depth, int teeth, double angle, Color color)
        {
            int ringCount = teeth * 2;
            Vec3[] frontOuter = new Vec3[ringCount];
            Vec3[] backOuter = new Vec3[ringCount];
            Vec3[] frontInner = new Vec3[ringCount];
            Vec3[] backInner = new Vec3[ringCount];

            double frontZ = center.Z + (depth * 0.5d);
            double backZ = center.Z - (depth * 0.5d);

            for (int i = 0; i < ringCount; i++)
            {
                bool tooth = (i % 2) == 0;
                double radius = tooth ? outerRadius : outerRadius * 0.84d;
                double t = (i / (double)ringCount) * (Math.PI * 2d) + angle;
                double c = Math.Cos(t);
                double s = Math.Sin(t);

                frontOuter[i] = new Vec3(center.X + (radius * c), center.Y + (radius * s), frontZ);
                backOuter[i] = new Vec3(center.X + (radius * c), center.Y + (radius * s), backZ);

                frontInner[i] = new Vec3(center.X + (innerRadius * c), center.Y + (innerRadius * s), frontZ);
                backInner[i] = new Vec3(center.X + (innerRadius * c), center.Y + (innerRadius * s), backZ);
            }

            for (int i = 0; i < ringCount; i++)
            {
                int n = (i + 1) % ringCount;

                Draw3DLine(frontOuter[i], frontOuter[n], color);
                Draw3DLine(backOuter[i], backOuter[n], color);
                Draw3DLine(frontOuter[i], backOuter[i], color);

                Draw3DLine(frontInner[i], frontInner[n], color);
                Draw3DLine(backInner[i], backInner[n], color);
                Draw3DLine(frontInner[i], backInner[i], color);

                if ((i % 2) == 0)
                {
                    Draw3DLine(frontOuter[i], frontInner[i], color);
                    Draw3DLine(backOuter[i], backInner[i], color);
                }
            }
        }

        private void RenderGears()
        {
            window.Clear(Color.FromArgb(14, 16, 23));

            DrawGear(new Vec3(-2.6d, -0.8d, 0.0d), 1.85d, 0.72d, 0.75d, 20, redAngle, Color.FromArgb(238, 95, 95));
            DrawGear(new Vec3(2.35d, -0.95d, 0.0d), 1.65d, 0.66d, 0.75d, 18, greenAngle, Color.FromArgb(108, 214, 132));
            DrawGear(new Vec3(0.25d, 2.0d, 0.0d), 1.45d, 0.58d, 0.75d, 16, blueAngle, Color.FromArgb(113, 170, 255));

            window.DrawString("OSGD GLXGears", Color.FromArgb(182, 194, 214), 10, 8);
            window.DrawString("FPS: " + fps.ToString(), Color.FromArgb(182, 194, 214), 10, 26);
            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 220, 120, 620, 420);
            window.Title = "GLXGears";
            window.CanResize = true;
            window.Closing = TryStop;
            wm.AddWindow(window);

            RenderGears();
        }

        public override void Run()
        {
            redAngle += 0.055d;
            greenAngle -= 0.082d;
            blueAngle += 0.105d;

            DateTime now = DateTime.Now;
            frames++;
            if (lastSecond != now.Second)
            {
                fps = frames;
                frames = 0;
                lastSecond = now.Second;
            }

            RenderGears();
        }
    }
}
