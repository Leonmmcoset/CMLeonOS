using System;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;

namespace CMLeonOS.Commands
{
    public static class TestGuiCommand
    {
        public static void RunTestGui()
        {
            Canvas canvas;

            Console.WriteLine("Cosmos booted successfully. Let's go in Graphical Mode");

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(640, 480, ColorDepth.ColorDepth32));

            canvas.Clear(global::System.Drawing.Color.FromArgb(0, 0, 255));

            try
            {
                canvas.DrawPoint(global::System.Drawing.Color.FromArgb(255, 0, 0), 69, 69);

                canvas.DrawLine(global::System.Drawing.Color.FromArgb(173, 255, 47), 250, 100, 400, 100);

                canvas.DrawLine(global::System.Drawing.Color.FromArgb(205, 92, 92), 350, 150, 350, 250);

                canvas.DrawLine(global::System.Drawing.Color.FromArgb(245, 245, 220), 250, 150, 400, 250);

                canvas.DrawRectangle(global::System.Drawing.Color.FromArgb(219, 112, 147), 350, 350, 80, 60);

                canvas.DrawRectangle(global::System.Drawing.Color.FromArgb(50, 205, 50), 450, 450, 80, 60);

                canvas.Display();

                Console.WriteLine("Press any key to return to shell...");
                Console.ReadKey(true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred: " + e.Message);
            }
        }
    }
}
