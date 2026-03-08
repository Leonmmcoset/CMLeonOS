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

using Cosmos.HAL;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using CMLeonOS;
using CMLeonOS.Logger;
using System;

namespace CMLeonOS.Gui.Sound
{
    internal class SoundService : Process
    {
        public SoundService() : base("SoundService", ProcessType.Service)
        {
        }

        internal AudioMixer Mixer { get; private set; }
        internal AudioDriver Driver { get; private set; }
        internal AudioManager AudioManager { get; private set; }

        internal bool DriverReady { get; private set; } = false;

        private const int bufferSize = 4096;

        private static class SystemSounds
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Sounds.Login.wav")]
            private static byte[] _soundBytes_Login;
            internal static MemoryAudioStream Sound_Login = MemoryAudioStream.FromWave(_soundBytes_Login);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Sounds.Alert.wav")]
            private static byte[] _soundBytes_Alert;
            internal static MemoryAudioStream Sound_Alert = MemoryAudioStream.FromWave(_soundBytes_Alert);
        }

        internal void PlaySystemSound(SystemSound systemSound)
        {
            if (!DriverReady) return;

            switch (systemSound)
            {
                case SystemSound.None:
                    return;
                case SystemSound.Login:
                    Mixer.Streams.Add(SystemSounds.Sound_Login);
                    break;
                case SystemSound.Alert:
                    Mixer.Streams.Add(SystemSounds.Sound_Alert);
                    break;
                default:
                    throw new Exception("Unknown system sound.");
            }
        }

        #region Process
        public override void Start()
        {
            base.Start();
            Logger.Logger.Instance.Info("SoundService", "Starting SoundService.");

            for (int i = 0; i < Cosmos.HAL.PCI.Count; i++)
            {
                var device = Cosmos.HAL.PCI.Devices[i];

                /* AC97 */
                if (device.ClassCode == (byte)ClassID.MultimediaDevice &&
                    device.Subclass == 0x01
                    && !VMTools.IsVMWare)
                {
                    Logger.Logger.Instance.Info("SoundService", "Initialising AC97 driver.");
                    Driver = AC97.Initialize(bufferSize);
                }
            }

            if (Driver != null)
            {
                DriverReady = true;

                Mixer = new AudioMixer();
                AudioManager = new AudioManager()
                {
                    Stream = Mixer,
                    Output = Driver
                };
                AudioManager.Enable();

                Logger.Logger.Instance.Info("SoundService", "SoundService ready.");
            }
            else
            {
                Logger.Logger.Instance.Warning("SoundService", "No driver available.");
            }
        }

        public override void Run()
        {
        }

        public override void Stop()
        {
            base.Stop();
            if (DriverReady)
            {
                Logger.Logger.Instance.Info("SoundService", "Driver disabling.");
                AudioManager.Disable();
            }
        }
        #endregion Process
    }
}
