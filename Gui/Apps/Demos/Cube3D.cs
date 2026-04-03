using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.OSGraphicDraw;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Demos
{
    internal class Cube3D : Process
    {
        internal Cube3D() : base("Cube3D", ProcessType.Application) { }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private AppWindow window;

        private double angleX = 0d;
        private double angleY = 0d;
        private double angleZ = 0d;

        private static readonly Vec3[] BaseVertices = new Vec3[]
        {
            new Vec3(-1, -1, -1),
            new Vec3( 1, -1, -1),
            new Vec3( 1,  1, -1),
            new Vec3(-1,  1, -1),
            new Vec3(-1, -1,  1),
            new Vec3( 1, -1,  1),
            new Vec3( 1,  1,  1),
            new Vec3(-1,  1,  1),
        };

        private static readonly int[] Edges = new int[]
        {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

        private void RenderCube()
        {
            window.Clear(Color.FromArgb(15, 17, 24));

            Vec3[] transformed = new Vec3[BaseVertices.Length];
            for (int i = 0; i < BaseVertices.Length; i++)
            {
                Vec3 v = BaseVertices[i];
                v = Vec3.RotateX(v, angleX);
                v = Vec3.RotateY(v, angleY);
                v = Vec3.RotateZ(v, angleZ);
                transformed[i] = v;
            }

            double fovScale = 240d;
            double cameraDistance = 4.2d;

            for (int i = 0; i < Edges.Length; i += 2)
            {
                int a = Edges[i];
                int b = Edges[i + 1];
                Renderer3D.DrawLine3D(window, transformed[a], transformed[b], Color.FromArgb(188, 224, 255), fovScale, cameraDistance);
            }

            window.DrawString("OSGD Cube Demo", Color.FromArgb(140, 170, 198), 10, 8);
            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 280, 140, 480, 360);
            window.Title = "Cube3D";
            window.CanResize = true;
            window.Closing = TryStop;
            wm.AddWindow(window);

            RenderCube();
        }

        public override void Run()
        {
            angleX += 0.015d;
            angleY += 0.019d;
            angleZ += 0.011d;
            RenderCube();
        }
    }
}
