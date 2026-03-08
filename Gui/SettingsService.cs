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

using Cosmos.System;
using Cosmos.System.Graphics;
using CMLeonOS;
using CMLeonOS.Gui.ShellComponents;
using CMLeonOS.Logger;
using CMLeonOS.Utils;
using System.Text;
using System.IO;

namespace CMLeonOS.Gui
{
    internal class SettingsService : Process
    {
        public SettingsService() : base("SettingsService", ProcessType.Service)
        {
        }

        private const string oldSettingsPath = @"0:\settings.ini";
        private const string settingsPath = @"0:\etc\gui.cfg";

        private bool _twelveHourClock = false;
        internal bool TwelveHourClock
        {
            get
            {
                return _twelveHourClock;
            }
            set
            {
                _twelveHourClock = value;
                ProcessManager.GetProcess<Taskbar>()?.UpdateTime();
            }
        }

        private bool _leftHandStartButton = false;
        internal bool LeftHandStartButton
        {
            get
            {
                return _leftHandStartButton;
            }
            set
            {
                _leftHandStartButton = value;
                ProcessManager.GetProcess<Taskbar>()?.SetLeftHandStartButton(LeftHandStartButton);
            }
        }

        private bool _showFps = true;
        internal bool ShowFps
        {
            get
            {
                return _showFps;
            }
            set
            {
                _showFps = value;

                WindowManager wm = ProcessManager.GetProcess<WindowManager>();
                if (value)
                {
                    wm.ShowFps();
                }
                else
                {
                    wm.HideFps();
                }
            }
        }

        private bool _darkNotepad = false;
        internal bool DarkNotepad
        {
            get
            {
                return _darkNotepad;
            }
            set
            {
                _darkNotepad = value;
            }
        }

        private float _mouseSensitivity = 1.0f;
        internal float MouseSensitivity
        {
            get
            {
                return _mouseSensitivity;
            }
            set
            {
                _mouseSensitivity = value;
                MouseManager.MouseSensitivity = value;
            }
        }

        private Mode _mode = new Mode(1280, 800, ColorDepth.ColorDepth32);
        internal Mode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        internal void Flush()
        {
            IniBuilder builder = new IniBuilder();

            /* Appearance */
            builder.BeginSection("Appearance");

            builder.AddKey("LeftHandStartButton", LeftHandStartButton);

            builder.AddKey("ShowFps", ShowFps);

            builder.AddKey("ScreenWidth", Mode.Width);
            builder.AddKey("ScreenHeight", Mode.Height);

            /* Date & Time */
            builder.BeginSection("DateAndTime");
            builder.AddKey("TwelveHourClock", TwelveHourClock);

            /* Mouse */
            builder.BeginSection("Mouse");
            builder.AddKey("Sensitivity", MouseSensitivity);

            /* Apps */
            /* Notepad*/
            builder.BeginSection("Apps.Notepad");
            builder.AddKey("Dark", DarkNotepad);

            File.WriteAllText(settingsPath, builder.ToString());
        }

        #region Process
        public override void Start()
        {
            base.Start();
            if (File.Exists(oldSettingsPath))
            {
                Logger.Logger.Instance.Info("SettingsService", "Upgrading settings.ini...");
                string text = File.ReadAllText(oldSettingsPath);
                File.WriteAllText(settingsPath, text);
                File.Delete(oldSettingsPath);
            }
            if (File.Exists(settingsPath))
            {
                IniReader reader = new IniReader(File.ReadAllText(settingsPath));

                /* Appearance */
                if (reader.TryReadBool("LeftHandStartButton", out bool leftHandStartButton, section: "Appearance"))
                {
                    LeftHandStartButton = leftHandStartButton;
                }
                if (reader.TryReadInt("ScreenWidth", out int width, section: "Appearance"))
                {
                    if (reader.TryReadInt("ScreenHeight", out int height, section: "Appearance"))
                    {
                        _mode = new Mode((uint)width, (uint)height, ColorDepth.ColorDepth32);
                    }
                }
                if (reader.TryReadBool("ShowFps", out bool showFps, section: "ShowFps"))
                {
                    ShowFps = showFps;
                }

                /* Date & Time */
                if (reader.TryReadBool("TwelveHourClock", out bool twelveHourClock, section: "DateAndTime"))
                {
                    TwelveHourClock = twelveHourClock;
                }

                /* Mouse */
                if (reader.TryReadFloat("Sensitivity", out float mouseSensitivity, section: "Mouse"))
                {
                    MouseSensitivity = mouseSensitivity;
                }

                /* Apps */
                /* Notepad */
                if (reader.TryReadBool("Dark", out bool darkNotepad, section: "Apps.Notepad"))
                {
                    DarkNotepad = darkNotepad;
                }
            }
        }

        public override void Run()
        {
        }

        public override void Stop()
        {
            base.Stop();
            Flush();
        }
        #endregion
    }
}
