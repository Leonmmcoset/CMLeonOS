using CMLeonOS;
using CMLeonOS.Gui.UILib;

namespace CMLeonOS.Gui.ShellComponents.Dock
{
    internal class AppDockIcon : BaseDockIcon
    {
        internal AppDockIcon(AppWindow appWindow) : base(
            image: appWindow.Icon,
            doAnimation: true)
        {
            AppWindow = appWindow;
        }

        internal AppWindow AppWindow { get; init; }

        internal override void Clicked()
        {
            ProcessManager.GetProcess<WindowManager>().Focus = AppWindow;
        }
    }
}
