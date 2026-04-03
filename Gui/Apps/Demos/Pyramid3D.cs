using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.OSGraphicDraw;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Demos
{
    internal class Pyramid3D : Process
    {
        internal Pyramid3D() : base("Pyramid3D", ProcessType.Application) { }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private AppWindow window;

        private double angleX = 0d;
        private double angleY = 0d;
        private double angleZ = 0d;

        // Base square + apex
        private static readonly Vec3[] BaseVertices = new Vec3[]
        {
            new Vec3(-1, -1, -1),
            new Vec3( 1, -1, -1),
            new Vec3( 1, -1,  1),
            new Vec3(-1, -1,  1),
            new Vec3( 0,  1,  0), // apex
        };

        // edges in pairs
        private static readonly int[] Edges = new int[]
        {
            0,1, 1,2, 2,3, 3,0,
            0,4, 1,4, 2,4, 3,4
        };

        private void RenderPyramid()
        {
            window.Clear(Color.FromArgb(14, 18, 22));

            Vec3[] transformed = new Vec3[BaseVertices.Length];
            for (int i = 0; i < BaseVertices.Length; i++)
            {
                Vec3 v = BaseVertices[i];
                v = Vec3.RotateX(v, angleX);
                v = Vec3.RotateY(v, angleY);
                v = Vec3.RotateZ(v, angleZ);
                transformed[i] = v;
            }

            double fovScale = 230d;
            double cameraDistance = 4.0d;
            Color wireColor = Color.FromArgb(255, 208, 136);

            for (int i = 0; i < Edges.Length; i += 2)
            {
                int a = Edges[i];
                int b = Edges[i + 1];
                Renderer3D.DrawLine3D(window, transformed[a], transformed[b], wireColor, fovScale, cameraDistance);
            }

            window.DrawString("OSGD Pyramid Demo", Color.FromArgb(176, 184, 196), 10, 8);
            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 300, 160, 460, 340);
            window.Title = "Pyramid3D";
            window.CanResize = true;
            window.Closing = TryStop;
            wm.AddWindow(window);

            RenderPyramid();
        }

        public override void Run()
        {
            angleX += 0.014d;
            angleY += 0.017d;
            angleZ += 0.010d;
            RenderPyramid();
        }
    }
}
