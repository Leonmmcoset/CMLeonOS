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
using CMLeonOS;
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using CMLeonOS.Logger;
using System.Drawing;
using System.Text;

namespace CMLeonOS.Gui.Apps
{
    internal class Logs : Process
    {
        internal Logs() : base("Logs", ProcessType.Application) { }

        AppWindow window;

        Table table;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Logs.Info.bmp")]
            private static byte[] _iconBytes_Info;
            internal static Bitmap Icon_Info = new Bitmap(_iconBytes_Info);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Logs.Warning.bmp")]
            private static byte[] _iconBytes_Warning;
            internal static Bitmap Icon_Warning = new Bitmap(_iconBytes_Warning);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Logs.Error.bmp")]
            private static byte[] _iconBytes_Error;
            internal static Bitmap Icon_Error = new Bitmap(_iconBytes_Error);
        }

        private void AddLogToTable(LogEntry log)
        {
            Bitmap icon;
            switch (log.Priority)
            {
                case LogLevel.Info:
                    icon = Icons.Icon_Info;
                    break;
                case LogLevel.Warning:
                    icon = Icons.Icon_Warning;
                    break;
                case LogLevel.Error:
                    icon = Icons.Icon_Error;
                    break;
                default:
                    icon = null;
                    break;
            }
            table.Cells.Add(new TableCell(icon, $"{log.Date.ToString("HH:mm")} - {log.Source}: {log.Message}", log));
        }

        private void Update()
        {
            window.Clear(Color.Gray);

            string text = $"{Log.Logs.Count} messages";
            window.DrawString(text, Color.White, window.Width - (FontData.Width * text.Length) - 12, window.Height - FontData.Height - 12);

            table.Render();
            table.ScrollToBottom();

            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();

            if (UserSystem.CurrentLoggedInUser == null || !UserSystem.CurrentLoggedInUser.Admin)
            {
                MessageBox messageBox = new MessageBox(Parent, Name, "You must be an admin to run this app.");
                messageBox.Show();

                TryStop();
                return;
            }

            window = new AppWindow(this, 256, 256, 512, 352);
            wm.AddWindow(window);
            window.Icon = AppManager.GetAppMetadata("Event Log").Icon;
            window.Title = "Event Log";
            window.Closing = TryStop;

            table = new Table(window, 12, 12, window.Width - 24, window.Height - 24 - 16 - 12);
            table.OnDoubleClick = (int x, int y) =>
            {
                if (table.SelectedCellIndex != -1)
                {
                    LogEntry log = (LogEntry)table.Cells[table.SelectedCellIndex].Tag;

                    string priority = log.Priority switch
                    {
                        LogLevel.Info => "Info",
                        LogLevel.Warning => "Warning",
                        LogLevel.Error => "Error",
                        _ => "Unknown"
                    };

                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"Date: {log.Date.ToLongDateString()} {log.Date.ToLongTimeString()}");
                    builder.AppendLine($"Source: {log.Source}");
                    builder.AppendLine($"Priority: {priority}");
                    builder.AppendLine();
                    builder.Append(log.Message);

                    MessageBox messageBox = new MessageBox(this, $"{log.Source}: Log Event", builder.ToString());
                    messageBox.Show();
                }
            };
            wm.AddWindow(table);

            table.Cells.Clear();
            foreach (LogEntry log in Log.Logs)
            {
                AddLogToTable(log);
            }

            Log.LogEmittedReceivers.Add((LogEntry log) =>
            {
                AddLogToTable(log);
                Update();
            });

            Update();
        }

        public override void Run()
        {

        }
    }
}
