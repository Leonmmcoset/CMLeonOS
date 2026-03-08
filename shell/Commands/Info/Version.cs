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

using System;
using IL2CPU.API.Attribs;

namespace CMLeonOS.Commands
{
    public static class VersionCommand
    {
        [ManifestResourceStream(ResourceName = "CMLeonOS.BuildTime.txt")]
        private static byte[] buildTimeResource;

        public static void ProcessVersion()
        {
            string buildTime = global::System.Text.Encoding.UTF8.GetString(buildTimeResource);
            Console.WriteLine(Version.DisplayVersionWithGit);
            Console.WriteLine($"Major: {Version.Major}");
            Console.WriteLine($"Minor: {Version.Minor}");
            Console.WriteLine($"Patch: {Version.Patch}");
            Console.WriteLine($"Type: {Version.VersionType}");
            Console.WriteLine($"Full Version: {Version.FullVersion}");
            Console.WriteLine($"Git Commit: {Version.GitCommit}");
            Console.WriteLine($"Build Time: {buildTime}");
        }
    }
}
