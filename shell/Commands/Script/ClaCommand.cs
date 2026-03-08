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
using System.IO;
using System.Text;
using UniLua;

namespace CMLeonOS.Commands.Script
{
    public static class ClaCommand
    {
        private const string FILE_HEADER = "CMLeonOS_CLA";
        private static readonly byte[] ENCRYPTION_KEY = { 0x55, 0xAA, 0x33, 0xCC, 0x77, 0x88, 0x11, 0x22, 0x44, 0x66, 0x99, 0xBB };

        public static void RunClaFile(string args, CMLeonOS.FileSystem fileSystem, Shell shell, Action<string> showError, Action<string> showWarning)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError(UsageGenerator.GenerateSimpleUsage("cla", "<cla_file>"));
                showError("Example: cla app.cla");
                return;
            }

            string claFilePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(claFilePath))
            {
                showError($"Error: CLA file not found: {args}");
                return;
            }

            if (!claFilePath.EndsWith(".cla", StringComparison.OrdinalIgnoreCase))
            {
                showError("Error: Input file must have .cla extension");
                return;
            }

            try
            {
                string luaContent = DecryptClaFile(claFilePath);
                
                if (string.IsNullOrEmpty(luaContent))
                {
                    showError("Error: Failed to decrypt CLA file or file is corrupted");
                    return;
                }

                LuaCommand.ExecuteLuaCode(luaContent, fileSystem, shell, showError, showWarning);
            }
            catch (Exception ex)
            {
                showError($"Error running CLA file: {ex.Message}");
            }
        }

        private static string DecryptClaFile(string claFilePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(claFilePath)))
            {
                byte[] headerBytes = reader.ReadBytes(FILE_HEADER.Length);
                string header = Encoding.ASCII.GetString(headerBytes);
                
                if (header != FILE_HEADER)
                {
                    throw new Exception("Invalid CLA file format: Missing or incorrect header");
                }

                byte[] encryptedData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                return DecryptData(encryptedData);
            }
        }

        private static string DecryptData(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int keyIndex = i % ENCRYPTION_KEY.Length;
                data[i] = (byte)(data[i] ^ ENCRYPTION_KEY[keyIndex]);
            }
            
            return Encoding.UTF8.GetString(data);
        }
    }
}
