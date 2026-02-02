using System;
using System.Text;

namespace CMLeonOS
{
    /// <summary>
    /// Cosmos裸机专属Base64编码/解码工具类
    /// 无.NET原生Convert依赖，纯手写实现，适配Cosmos System2
    /// </summary>
    public static class Base64Helper
    {
        // Base64标准编码表（0-63对应），裸机环境直接硬编码，无需动态生成
        private const string Base64Table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        /// <summary>
        /// Base64编码（加密）：将字符串转为Base64编码串（默认UTF8编码）
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string Encode(string input)
        {
            // 空值校验，裸机环境需严格判空
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // 将字符串转为UTF8字节数组（Cosmos支持基础UTF8/ASCII）
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            // 调用字节数组的编码核心方法
            return EncodeBytes(inputBytes);
        }

        /// <summary>
        /// Base64编码（核心）：将二进制字节数组转为Base64编码串
        /// 适配任意二进制数据（文件、网络流、字符串字节）
        /// </summary>
        /// <param name="inputBytes">原始二进制字节数组</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string EncodeBytes(byte[] inputBytes)
        {
            if (inputBytes == null || inputBytes.Length == 0)
                return string.Empty;

            // 构建结果字符串，裸机用StringBuilder更高效
            StringBuilder result = new StringBuilder();
            int inputLength = inputBytes.Length;
            // 按3字节为一组遍历，处理所有完整组
            for (int i = 0; i < inputLength; i += 3)
            {
                // 取3个字节，不足的补0（位运算用，不修改原数组）
                byte b1 = i < inputLength ? inputBytes[i] : (byte)0;
                byte b2 = i + 1 < inputLength ? inputBytes[i + 1] : (byte)0;
                byte b3 = i + 2 < inputLength ? inputBytes[i + 2] : (byte)0;

                // 核心位运算：3字节(24位)拆分为4个6位
                // 第一个6位：b1的高6位（右移2位，与0x3F过滤低2位）
                int idx1 = (b1 >> 2) & 0x3F;
                // 第二个6位：b1的低2位 + b2的高4位（左移4位 + b2右移4位，与0x3F过滤）
                int idx2 = ((b1 & 0x03) << 4) | ((b2 >> 4) & 0x0F);
                // 第三个6位：b2的低4位 + b3的高2位（左移2位 + b3右移6位，与0x3F过滤）
                int idx3 = ((b2 & 0x0F) << 2) | ((b3 >> 6) & 0x03);
                // 第四个6位：b3的低6位（与0x3F过滤高2位）
                int idx4 = b3 & 0x3F;

                // 根据索引取Base64字符，添加到结果
                result.Append(Base64Table[idx1]);
                result.Append(Base64Table[idx2]);

                // 补位处理：不足3字节时，用=替代
                result.Append(i + 1 < inputLength ? Base64Table[idx3] : '=');
                result.Append(i + 2 < inputLength ? Base64Table[idx4] : '=');
            }

            return result.ToString();
        }

        /// <summary>
        /// Base64解码（解密）：将Base64编码串转回原始字符串（默认UTF8编码）
        /// </summary>
        /// <param name="input">Base64编码串</param>
        /// <returns>解码后的原始字符串</returns>
        /// <exception cref="ArgumentException">Base64格式错误时抛出</exception>
        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // 解码为字节数组，再转为UTF8字符串
            byte[] outputBytes = DecodeToBytes(input);
            return Encoding.UTF8.GetString(outputBytes);
        }

        /// <summary>
        /// Base64解码（核心）：将Base64编码串转回原始二进制字节数组
        /// 适配任意Base64编码的二进制数据
        /// </summary>
        /// <param name="input">Base64编码串</param>
        /// <returns>解码后的二进制字节数组</returns>
        /// <exception cref="ArgumentException">Base64格式错误时抛出</exception>
        public static byte[] DecodeToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<byte>();

            // 预处理：过滤所有非Base64有效字符（仅保留编码表字符和=）
            StringBuilder cleanInput = new StringBuilder();
            foreach (char c in input)
            {
                if (Base64Table.Contains(c) || c == '=')
                    cleanInput.Append(c);
            }
            string base64 = cleanInput.ToString();
            int inputLength = base64.Length;

            // 基础格式校验：Base64长度必须是4的倍数
            if (inputLength % 4 != 0)
                throw new ArgumentException("Invalid Base64 string: Length is not a multiple of 4");

            // 计算补位符数量（=的个数，只能是0/1/2）
            int padCount = 0;
            if (base64[inputLength - 1] == '=') padCount++;
            if (base64[inputLength - 2] == '=') padCount++;

            // 计算解码后的字节数：(4*分组数 - 补位符数) / 3
            int outputLength = (inputLength * 6) / 8 - padCount;
            byte[] outputBytes = new byte[outputLength];
            int outputIndex = 0;

            // 按4个字符为一组遍历，处理所有组
            for (int i = 0; i < inputLength; i += 4)
            {
                // 取4个Base64字符，转换为对应的6位索引（0-63）
                int idx1 = Base64Table.IndexOf(base64[i]);
                int idx2 = Base64Table.IndexOf(base64[i + 1]);
                // 补位的=索引为-1，转为0处理
                int idx3 = i + 2 < inputLength ? Base64Table.IndexOf(base64[i + 2]) : 0;
                int idx4 = i + 3 < inputLength ? Base64Table.IndexOf(base64[i + 3]) : 0;

                // 基础校验：无效字符（索引为-1且非=）
                if (idx1 == -1 || idx2 == -1 || (idx3 == -1 && base64[i+2] != '=') || (idx4 == -1 && base64[i+3] != '='))
                    throw new ArgumentException("Invalid Base64 string: Contains invalid characters");

                // 核心位运算：4个6位拼接为24位，拆分为3个字节
                uint combined = (uint)((idx1 << 18) | (idx2 << 12) | (idx3 << 6) | idx4);
                // 第一个字节：24位的高8位
                byte b1 = (byte)((combined >> 16) & 0xFF);
                // 第二个字节：24位的中间8位
                byte b2 = (byte)((combined >> 8) & 0xFF);
                // 第三个字节：24位的低8位
                byte b3 = (byte)(combined & 0xFF);

                // 将字节写入结果数组，根据补位符数量跳过多余字节
                if (outputIndex < outputLength)
                    outputBytes[outputIndex++] = b1;
                if (outputIndex < outputLength)
                    outputBytes[outputIndex++] = b2;
                if (outputIndex < outputLength)
                    outputBytes[outputIndex++] = b3;
            }

            return outputBytes;
        }
    }
}
