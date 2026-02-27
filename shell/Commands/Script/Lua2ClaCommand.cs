using System;
using System.IO;
using System.Text;

namespace CMLeonOS.Commands.Script
{
    public static class Lua2ClaCommand
    {
        private const string FILE_HEADER = "CMLeonOS_CLA";
        private static readonly byte[] ENCRYPTION_KEY = { 0x55, 0xAA, 0x33, 0xCC, 0x77, 0x88, 0x11, 0x22, 0x44, 0x66, 0x99, 0xBB };

        public static void ConvertLuaToCla(string args, CMLeonOS.FileSystem fileSystem, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError(UsageGenerator.GenerateSimpleUsage("lua2cla", "<lua_file>"));
                showError("Example: lua2cla app.lua");
                return;
            }

            string luaFilePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(luaFilePath))
            {
                showError($"Error: Lua file not found: {args}");
                return;
            }

            if (!luaFilePath.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
            {
                showError("Error: Input file must have .lua extension");
                return;
            }

            try
            {
                string luaContent = File.ReadAllText(luaFilePath);
                byte[] encryptedData = EncryptData(luaContent);
                
                string claFilePath = luaFilePath.Substring(0, luaFilePath.Length - 4) + ".cla";
                
                using (BinaryWriter writer = new BinaryWriter(File.Create(claFilePath)))
                {
                    writer.Write(Encoding.ASCII.GetBytes(FILE_HEADER));
                    writer.Write(encryptedData);
                }

                showSuccess($"Successfully converted: {args} -> {Path.GetFileName(claFilePath)}");
                Console.WriteLine($"Output file: {claFilePath}");
            }
            catch (Exception ex)
            {
                showError($"Error converting file: {ex.Message}");
            }
        }

        private static byte[] EncryptData(string content)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            
            for (int i = 0; i < data.Length; i++)
            {
                int keyIndex = i % ENCRYPTION_KEY.Length;
                data[i] = (byte)(data[i] ^ ENCRYPTION_KEY[keyIndex]);
            }
            
            return data;
        }
    }
}
