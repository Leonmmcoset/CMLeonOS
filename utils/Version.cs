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

namespace CMLeonOS
{
    public static class Version
    {
        public static string Major = "1";
        public static string Minor = "0";
        public static string Patch = "0";
        public static string VersionType = "Release";
        public static string GitCommit = "unknown";
        
        public static string FullVersion
        {
            get { return $"{Major}.{Minor}.{Patch}-{VersionType}"; }
        }
        
        public static string ShortVersion
        {
            get { return $"{Major}.{Minor}.{Patch}"; }
        }
        
        public static string DisplayVersion
        {
            get { return $"CMLeonOS v{ShortVersion} ({VersionType})"; }
        }
        
        public static string DisplayVersionWithGit
        {
            get { return $"CMLeonOS v{ShortVersion} ({VersionType}) - Git: {GitCommit}"; }
        }
        
        public static string GetVersion()
        {
            return ShortVersion;
        }
    }
}
