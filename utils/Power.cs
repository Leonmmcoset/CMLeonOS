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

using Sys = Cosmos.System;

namespace CMLeonOS
{
    public static class Power
    {
        public static void Reboot()
        {
            Sys.Power.Reboot();
        }

        public static void Shutdown(bool reboot = false)
        {
            if (reboot)
            {
                Sys.Power.Reboot();
            }
            else
            {
                Sys.Power.Shutdown();
            }
        }
    }
}
