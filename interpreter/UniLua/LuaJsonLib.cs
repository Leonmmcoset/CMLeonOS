using System;
using System.Collections.Generic;
using System.Text;
using StringBuilder = System.Text.StringBuilder;

namespace UniLua
{
    internal static class LuaJsonLib
    {
        public const string LIB_NAME = "json";

        private static readonly string[] ESCAPE_CHARS = new string[]
        {
            "\"", "\\", "/", "\b", "\f", "\n", "\r", "\t"
        };

        public static int OpenLib(ILuaState lua)
        {
            NameFuncPair[] define = new NameFuncPair[]
            {
                new NameFuncPair("encode", Json_Encode),
                new NameFuncPair("decode", Json_Decode),
                new NameFuncPair("null", Json_Null),
                new NameFuncPair("parse", Json_Parse),
                new NameFuncPair("stringify", Json_Stringify)
            };

            lua.L_NewLib(define);
            return 1;
        }

        private static int Json_Encode(ILuaState lua)
        {
            lua.L_CheckAny(1);
            LuaType t = lua.Type(1);

            if (t == LuaType.LUA_TNIL)
            {
                lua.PushString("null");
                return 1;
            }

            if (t == LuaType.LUA_TBOOLEAN)
            {
                lua.PushString(lua.ToBoolean(1) ? "true" : "false");
                return 1;
            }

            if (t == LuaType.LUA_TNUMBER)
            {
                lua.PushString(lua.ToNumber(1).ToString());
                return 1;
            }

            if (t == LuaType.LUA_TSTRING)
            {
                string s = lua.ToString(1);
                lua.PushString(JsonEscapeString(s));
                return 1;
            }

            if (t == LuaType.LUA_TTABLE)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                
                bool first = true;
                lua.PushNil();
                while (lua.Next(1))
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    first = false;

                    string key = lua.ToString(-2);
                    if (lua.Type(-1) == LuaType.LUA_TSTRING)
                    {
                        sb.Append("\"");
                        sb.Append(JsonEscapeString(key));
                        sb.Append("\":");
                    }
                    else
                    {
                        sb.Append(key);
                        sb.Append(":");
                    }

                    lua.PushValue(-1);
                    int result = Json_Encode(lua);
                    lua.Pop(1);

                    if (lua.Type(-1) == LuaType.LUA_TSTRING)
                    {
                        sb.Append("\"");
                        sb.Append(JsonEscapeString(lua.ToString(-1)));
                    }
                    else
                    {
                        sb.Append(lua.ToString(-1));
                    }
                }

                sb.Append("}");
                lua.PushString(sb.ToString());
                return 1;
            }

            lua.PushNil();
            return 1;
        }

        private static int Json_Decode(ILuaState lua)
        {
            lua.L_CheckString(1);
            string json = lua.ToString(1).Trim();
            
            if (json == "null" || json.Length == 0)
            {
                lua.PushNil();
                return 1;
            }

            if (json == "true")
            {
                lua.PushBoolean(true);
                return 1;
            }

            if (json == "false")
            {
                lua.PushBoolean(false);
                return 1;
            }

            if ((json.StartsWith("\"") && json.EndsWith("\"")) ||
                (json.StartsWith("'") && json.EndsWith("'")))
            {
                string unquoted = json.Substring(1, json.Length - 2);
                lua.PushString(unquoted);
                return 1;
            }

            if (json.StartsWith("{") || json.StartsWith("["))
            {
                try
                {
                    int pos = 0;
                    object result = ParseJson(json, ref pos);
                    PushLuaValue(lua, result);
                    return 1;
                }
                catch
                {
                    lua.PushNil();
                    return 1;
                }
            }

            if (char.IsDigit(json[0]))
            {
                double num = double.Parse(json);
                lua.PushNumber(num);
                return 1;
            }

            lua.PushNil();
            return 1;
        }

        private static object ParseJson(string json, ref int pos)
        {
            json = json.Trim();
            
            if (json.StartsWith("{"))
            {
                return ParseJsonObject(json, ref pos);
            }
            else if (json.StartsWith("["))
            {
                return ParseJsonArray(json, ref pos);
            }
            
            return null;
        }

        private static Dictionary<string, object> ParseJsonObject(string json, ref int pos)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string content = json.Substring(1, json.Length - 2);
            int contentPos = 0;

            while (contentPos < content.Length)
            {
                contentPos = SkipWhitespace(content, contentPos);
                
                if (contentPos >= content.Length)
                {
                    break;
                }

                if (content[contentPos] == '}')
                {
                    contentPos++;
                    break;
                }

                string key = ParseJsonString(content, ref contentPos);
                contentPos = SkipWhitespace(content, contentPos);

                if (contentPos >= content.Length || content[contentPos] != ':')
                {
                    break;
                }
                contentPos++;

                object value = ParseJsonValue(content, ref contentPos);
                if (key != null)
                {
                    dict[key] = value;
                }

                contentPos = SkipWhitespace(content, contentPos);
                if (contentPos < content.Length && content[contentPos] == ',')
                {
                    contentPos++;
                }
            }

            pos = contentPos;
            return dict;
        }

        private static List<object> ParseJsonArray(string json, ref int pos)
        {
            List<object> list = new List<object>();
            string content = json.Substring(1, json.Length - 2);
            int contentPos = 0;

            while (contentPos < content.Length)
            {
                contentPos = SkipWhitespace(content, contentPos);
                
                if (contentPos >= content.Length)
                {
                    break;
                }

                if (content[contentPos] == ']')
                {
                    contentPos++;
                    break;
                }

                object value = ParseJsonValue(content, ref contentPos);
                if (value != null)
                {
                    list.Add(value);
                }

                contentPos = SkipWhitespace(content, contentPos);
                if (contentPos < content.Length && content[contentPos] == ',')
                {
                    contentPos++;
                }
            }

            pos = contentPos;
            return list;
        }

        private static object ParseJsonValue(string content, ref int pos)
        {
            pos = SkipWhitespace(content, pos);

            if (pos >= content.Length)
            {
                return null;
            }

            char c = content[pos];

            if (c == '{')
            {
                return ParseJsonObject(content, ref pos);
            }
            else if (c == '[')
            {
                return ParseJsonArray(content, ref pos);
            }
            else if (c == '"')
            {
                return ParseJsonString(content, ref pos);
            }
            else if (c == 't' || c == 'f' || c == 'n')
            {
                string keyword = "";
                while (pos < content.Length && char.IsLetter(content[pos]))
                {
                    keyword += content[pos];
                    pos++;
                }

                if (keyword == "true")
                {
                    return true;
                }
                else if (keyword == "false")
                {
                    return false;
                }
                else if (keyword == "null")
                {
                    return null;
                }

                return null;
            }
            else if (char.IsDigit(c) || c == '-')
            {
                string numStr = "";
                while (pos < content.Length && (char.IsDigit(content[pos]) || content[pos] == '-' || content[pos] == '.'))
                {
                    numStr += content[pos];
                    pos++;
                }

                if (double.TryParse(numStr, out double num))
                {
                    return num;
                }
            }

            return null;
        }

        private static string ParseJsonString(string content, ref int pos)
        {
            if (pos >= content.Length || content[pos] != '"')
            {
                return null;
            }

            pos++;

            StringBuilder sb = new StringBuilder();
            while (pos < content.Length)
            {
                char c = content[pos];
                pos++;

                if (c == '"')
                {
                    break;
                }
                else if (c == '\\')
                {
                    if (pos >= content.Length)
                    {
                        sb.Append('\\');
                        break;
                    }

                    pos++;
                    char escapeChar = content[pos];
                    pos++;

                    switch (escapeChar)
                    {
                        case '"':
                            sb.Append('"');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            if (pos + 3 < content.Length)
                            {
                                string hex = content.Substring(pos, 4);
                                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int code))
                                {
                                    sb.Append((char)code);
                                    pos += 3;
                                }
                            }
                            break;
                        default:
                            sb.Append(escapeChar);
                            break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static int SkipWhitespace(string content, int pos)
        {
            while (pos < content.Length && char.IsWhiteSpace(content[pos]))
            {
                pos++;
            }
            return pos;
        }

        private static void PushLuaValue(ILuaState lua, object value)
        {
            if (value == null)
            {
                lua.PushNil();
            }
            else if (value is bool)
            {
                lua.PushBoolean((bool)value);
            }
            else if (value is double)
            {
                lua.PushNumber((double)value);
            }
            else if (value is string)
            {
                lua.PushString((string)value);
            }
            else if (value is Dictionary<string, object>)
            {
                lua.NewTable();
                foreach (var kvp in (Dictionary<string, object>)value)
                {
                    lua.PushString(kvp.Key);
                    PushLuaValue(lua, kvp.Value);
                    lua.SetField(-3, kvp.Key);
                }
            }
            else if (value is List<object>)
            {
                lua.NewTable();
                int index = 1;
                foreach (var item in (List<object>)value)
                {
                    PushLuaValue(lua, item);
                    lua.RawSetI(-2, index++);
                }
            }
        }

        private static string JsonEscapeString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        private static int Json_Null(ILuaState lua)
        {
            lua.PushNil();
            return 1;
        }

        private static int Json_Parse(ILuaState lua)
        {
            return Json_Decode(lua);
        }

        private static int Json_Stringify(ILuaState lua)
        {
            return Json_Encode(lua);
        }
    }
}