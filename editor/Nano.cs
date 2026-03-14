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
using System.Collections.Generic;
using System.IO;
using UniLua;

namespace CMLeonOS
{
    public class NanoSettings
    {
        public bool EnableSmartIndent { get; set; } = true;
        public bool EnableAutoCompleteBrackets { get; set; } = true;
        public bool EnableAutoCompleteQuotes { get; set; } = true;
        public bool EnableSmartDelete { get; set; } = true;
        public bool EnableSyntaxHighlight { get; set; } = true;

        public void Save(string filePath)
        {
            try
            {
                bool smartIndent = EnableSmartIndent;
                bool autoBrackets = EnableAutoCompleteBrackets;
                bool autoQuotes = EnableAutoCompleteQuotes;
                bool smartDelete = EnableSmartDelete;
                bool syntaxHighlight = EnableSyntaxHighlight;
                
                string content = $"{smartIndent},{autoBrackets},{autoQuotes},{smartDelete},{syntaxHighlight}";
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(content);
                }
            }
            catch
            {
            }
        }

        public void Load(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        string content = sr.ReadToEnd();
                        string[] parts = content.Split(',');
                        if (parts.Length >= 5)
                        {
                            bool smartIndent, autoBrackets, autoQuotes, smartDelete, syntaxHighlight;
                            bool.TryParse(parts[0], out smartIndent);
                            bool.TryParse(parts[1], out autoBrackets);
                            bool.TryParse(parts[2], out autoQuotes);
                            bool.TryParse(parts[3], out smartDelete);
                            bool.TryParse(parts[4], out syntaxHighlight);
                            
                            EnableSmartIndent = smartIndent;
                            EnableAutoCompleteBrackets = autoBrackets;
                            EnableAutoCompleteQuotes = autoQuotes;
                            EnableSmartDelete = smartDelete;
                            EnableSyntaxHighlight = syntaxHighlight;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

    public class Nano
    {
        private string path;
        private bool quit = false;
        private bool modified = false;
        private string clipboard = string.Empty;
        private List<string> lines = new List<string>();
        private int currentLine = 0;
        private int linePos = 0;
        private int scrollX = 0;
        private int scrollY = 0;
        private int? updatedLinesStart;
        private int? updatedLinesEnd;
        private const int TITLEBAR_HEIGHT = 1;
        private const int SHORTCUT_BAR_HEIGHT = 1;
        private FileSystem fileSystem;
        private UserSystem userSystem;
        private Shell shell;
        private NanoSettings settings = new NanoSettings();

        private readonly (string, string)[] SHORTCUTS = new (string, string)[]
        {
            ("Ctrl+X", "Quit"),
            ("Ctrl+S", "Save"),
            ("Ctrl+I", "Info"),
            ("Ctrl+K", "Cut Line"),
            ("Ctrl+V", "Paste"),
            ("Ctrl+R", "Run")
        };

        string? pendingNotification;

        public Nano(string value, bool isPath = true, FileSystem fs = null, UserSystem us = null, Shell sh = null)
        {
            this.fileSystem = fs;
            this.userSystem = us;
            this.shell = sh;

            if (isPath)
            {
                this.path = value;

                if (value != null && fileSystem != null)
                {
                    try
                    {
                        string fullPath = fileSystem.GetFullPath(value);
                        string text = fileSystem.ReadFile(fullPath);
                        
                        if (!string.IsNullOrEmpty(text))
                        {
                            text = text.Replace("\r\n", "\n");
                            lines.AddRange(text.Split('\n'));
                        }
                        else
                        {
                            lines.Add(string.Empty);
                        }
                    }
                    catch
                    {
                        lines.Add(string.Empty);
                    }
                }
                else
                {
                    lines.Add(string.Empty);
                }
            }
            else
            {
                string text = value;
                text = text.Replace("\r\n", "\n");
                lines.AddRange(text.Split('\n'));
            }

            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
        }

        private void Render()
        {
            if (updatedLinesStart != null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                int consoleWidth = 80;
                int consoleHeight = 25;

                for (int i = (int)updatedLinesStart; i <= updatedLinesEnd; i++)
                {
                    int y = i - scrollY + TITLEBAR_HEIGHT;
                    if (y < TITLEBAR_HEIGHT || y >= consoleHeight - SHORTCUT_BAR_HEIGHT) continue;
                    Console.SetCursorPosition(0, y);

                    if (i >= lines.Count || scrollX >= lines[i].Length)
                    {
                        Console.Write(new string(' ', consoleWidth));
                    }
                    else
                    {
                        string line = lines[i].Substring(scrollX, Math.Min(consoleWidth, lines[i].Length - scrollX));
                        
                        if (IsLuaFile() && settings.EnableSyntaxHighlight)
                        {
                            RenderLuaLine(line, consoleWidth);
                        }
                        else if (IsIniFile() && settings.EnableSyntaxHighlight)
                        {
                            RenderIniLine(line, consoleWidth);
                        }
                        else if (IsJsonFile() && settings.EnableSyntaxHighlight)
                        {
                            RenderJsonLine(line, consoleWidth);
                        }
                        else
                        {
                            Console.Write(line + new string(' ', Math.Max(0, consoleWidth - line.Length)));
                        }
                    }
                }

                updatedLinesStart = null;
                updatedLinesEnd = null;
            }

            Console.SetCursorPosition(linePos - scrollX, currentLine + TITLEBAR_HEIGHT - scrollY);
        }

        private bool IsLuaFile()
        {
            if (path != null)
            {
                string extension = System.IO.Path.GetExtension(path)?.ToLower();
                return extension == ".lua" || extension == ".los" || extension == ".losb";
            }
            return false;
        }

        private bool IsIniFile()
        {
            if (path != null)
            {
                string extension = System.IO.Path.GetExtension(path)?.ToLower();
                return extension == ".ini" || extension == ".cfg" || extension == ".conf" || extension == ".config";
            }
            return false;
        }

        private bool IsJsonFile()
        {
            if (path != null)
            {
                string extension = System.IO.Path.GetExtension(path)?.ToLower();
                return extension == ".json";
            }
            return false;
        }

        private void RenderLuaLine(string line, int consoleWidth)
        {
            int pos = 0;
            bool inString = false;
            bool inComment = false;
            bool inLongComment = false;
            char stringDelimiter = '\0';

            while (pos < line.Length && pos < consoleWidth)
            {
                if (inComment)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(line[pos]);
                    pos++;
                }
                else if (inLongComment)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (pos + 1 < line.Length && line[pos] == ']' && line[pos + 1] == ']')
                    {
                        inLongComment = false;
                        Console.Write(line[pos]);
                        pos++;
                        Console.Write(line[pos]);
                        pos++;
                    }
                    else
                    {
                        Console.Write(line[pos]);
                        pos++;
                    }
                }
                else if (pos + 1 < line.Length && line[pos] == '-' && line[pos + 1] == '-')
                {
                    if (pos + 3 < line.Length && line[pos + 2] == '[' && line[pos + 3] == '[')
                    {
                        inLongComment = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(line.Substring(pos));
                        break;
                    }
                    else
                    {
                        inComment = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(line.Substring(pos));
                        break;
                    }
                }
                else if (inString)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (line[pos] == stringDelimiter)
                    {
                        if (pos + 1 < line.Length && line[pos + 1] == stringDelimiter)
                        {
                            Console.Write(line[pos]);
                            pos++;
                            Console.Write(line[pos]);
                            pos++;
                        }
                        else
                        {
                            Console.Write(line[pos]);
                            inString = false;
                            pos++;
                        }
                    }
                    else if (line[pos] == '\\')
                    {
                        Console.Write(line[pos]);
                        pos++;
                        if (pos < line.Length)
                        {
                            Console.Write(line[pos]);
                            pos++;
                        }
                    }
                    else
                    {
                        Console.Write(line[pos]);
                        pos++;
                    }
                }
                else
                {
                    if (line[pos] == '"' || line[pos] == '\'')
                    {
                        inString = true;
                        stringDelimiter = line[pos];
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(line[pos]);
                        pos++;
                    }
                    else if (IsLuaNumber(line, pos))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        while (pos < line.Length && IsLuaNumber(line, pos))
                        {
                            Console.Write(line[pos]);
                            pos++;
                        }
                    }
                    else if (IsLuaOperator(line, pos))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        if (pos + 1 < line.Length && IsLuaOperator(line, pos + 1))
                        {
                            Console.Write(line[pos]);
                            pos++;
                            Console.Write(line[pos]);
                            pos++;
                        }
                        else
                        {
                            Console.Write(line[pos]);
                            pos++;
                        }
                    }
                    else if (IsLuaKeyword(line, pos))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        string keyword = GetLuaKeyword(line, pos);
                        Console.Write(keyword);
                        pos += keyword.Length;
                    }
                    else if (IsLuaBuiltin(line, pos))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        string builtin = GetLuaBuiltin(line, pos);
                        Console.Write(builtin);
                        pos += builtin.Length;
                    }
                    else if (IsLuaFunctionCall(line, pos))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        string funcName = GetLuaFunctionName(line, pos);
                        Console.Write(funcName);
                        pos += funcName.Length;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(line[pos]);
                        pos++;
                    }
                }
            }

            if (pos < consoleWidth)
            {
                Console.Write(new string(' ', consoleWidth - pos));
            }

            Console.ResetColor();
        }

        private void RenderIniLine(string line, int consoleWidth)
        {
            int pos = 0;
            bool inComment = false;
            bool inSection = false;
            bool inKey = false;
            bool inValue = false;
            bool inStringValue = false;
            
            while (pos < line.Length && pos < consoleWidth)
            {
                char c = line[pos];
                
                if (inComment)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else if (pos + 1 < line.Length && line[pos] == ';' && line[pos + 1] == ';')
                {
                    inComment = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(line[pos]);
                    pos++;
                    Console.Write(line[pos]);
                    pos++;
                }
                else if (pos + 1 < line.Length && line[pos] == '/' && line[pos + 1] == '/')
                {
                    inComment = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(line[pos]);
                    pos++;
                    Console.Write(line[pos]);
                    pos++;
                }
                else if (pos + 1 < line.Length && line[pos] == '#' && line[pos + 1] == '#')
                {
                    inComment = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(line[pos]);
                    pos++;
                    Console.Write(line[pos]);
                    pos++;
                }
                else if (inSection)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(c);
                    pos++;
                }
                else if (c == '[')
                {
                    inSection = true;
                    inKey = false;
                    inValue = false;
                    inStringValue = false;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(c);
                    pos++;
                }
                else if (c == ']')
                {
                    inSection = false;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(c);
                    pos++;
                }
                else if (inKey)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(c);
                    pos++;
                }
                else if (c == '=')
                {
                    inKey = false;
                    inValue = true;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c);
                    pos++;
                }
                else if (inStringValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else if (c == '"' || c == '\'')
                {
                    inStringValue = true;
                    inValue = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c);
                    pos++;
                }
            }
            
            if (pos < consoleWidth)
            {
                Console.Write(new string(' ', consoleWidth - pos));
            }
            
            Console.ResetColor();
        }

        private void RenderJsonLine(string line, int consoleWidth)
        {
            int pos = 0;
            bool inString = false;
            bool inNumber = false;
            bool inBoolean = false;
            bool inNull = false;
            bool inKey = false;
            bool inColon = false;
            bool inWhitespace = false;
            
            while (pos < line.Length && pos < consoleWidth)
            {
                char c = line[pos];
                
                if (inString)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else if (c == '"')
                {
                    inString = true;
                    inNumber = false;
                    inBoolean = false;
                    inNull = false;
                    inWhitespace = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else if (inString && (c == '\\' || char.IsControl(c)))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    pos++;
                }
                else if (inNumber)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(c);
                    pos++;
                }
                else if (inBoolean)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(c);
                    pos++;
                }
                else if (inNull)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(c);
                    pos++;
                }
                else if (inKey)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(c);
                    pos++;
                }
                else if (c == ':')
                {
                    inKey = false;
                    inColon = false;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c);
                    pos++;
                }
                else if (c == ',' || c == '{' || c == '}')
                {
                    inWhitespace = false;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c);
                    pos++;
                }
                else if (char.IsWhiteSpace(c))
                {
                    inWhitespace = true;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(c);
                    pos++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(c);
                    pos++;
                }
            }
            
            if (pos < consoleWidth)
            {
                Console.Write(new string(' ', consoleWidth - pos));
            }
            
            Console.ResetColor();
        }

        private bool IsLuaNumber(string line, int pos)
        {
            if (pos >= line.Length) return false;
            
            char c = line[pos];
            if (!char.IsDigit(c) && c != '.') return false;
            
            if (c == '.')
            {
                if (pos + 1 >= line.Length) return false;
                return char.IsDigit(line[pos + 1]);
            }
            
            return true;
        }

        private bool IsLuaOperator(string line, int pos)
        {
            if (pos >= line.Length) return false;
            
            char c = line[pos];
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || 
                   c == '%' || c == '#' || c == '=' || c == '~' || 
                   c == '<' || c == '>' || c == '&' || c == '|' || 
                   c == '(' || c == ')' || c == '[' || c == ']' || 
                   c == '{' || c == '}' || c == ',' || c == ';';
        }

        private bool IsLuaKeyword(string line, int pos)
        {
            string[] keywords = { 
                "function", "end", "if", "then", "else", "elseif", "while", "do", 
                "for", "return", "local", "true", "false", "nil", "and", "or", 
                "not", "break", "repeat", "until", "in", "goto", "self"
            };
            
            foreach (string keyword in keywords)
            {
                if (pos + keyword.Length <= line.Length && line.Substring(pos, keyword.Length) == keyword)
                {
                    char prevChar = pos > 0 ? line[pos - 1] : ' ';
                    char nextChar = pos + keyword.Length < line.Length ? line[pos + keyword.Length] : ' ';
                    
                    if (!char.IsLetterOrDigit(prevChar) && !char.IsLetterOrDigit(nextChar) && prevChar != '_' && nextChar != '_')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetLuaKeyword(string line, int pos)
        {
            string[] keywords = { 
                "function", "end", "if", "then", "else", "elseif", "while", "do", 
                "for", "return", "local", "true", "false", "nil", "and", "or", 
                "not", "break", "repeat", "until", "in", "goto", "self"
            };
            
            foreach (string keyword in keywords)
            {
                if (pos + keyword.Length <= line.Length && line.Substring(pos, keyword.Length) == keyword)
                {
                    char prevChar = pos > 0 ? line[pos - 1] : ' ';
                    char nextChar = pos + keyword.Length < line.Length ? line[pos + keyword.Length] : ' ';
                    
                    if (!char.IsLetterOrDigit(prevChar) && !char.IsLetterOrDigit(nextChar) && prevChar != '_' && nextChar != '_')
                    {
                        return keyword;
                    }
                }
            }
            return "";
        }

        private bool IsLuaBuiltin(string line, int pos)
        {
            string[] builtins = { 
                "print", "type", "tonumber", "tostring", "pairs", "ipairs", 
                "next", "select", "unpack", "assert", "error", "pcall", 
                "xpcall", "load", "loadstring", "loadfile", "dofile", 
                "require", "setmetatable", "getmetatable", "rawget", "rawset", 
                "rawequal", "rawlen", "gcinfo", "collectgarbage", "newproxy"
            };
            
            foreach (string builtin in builtins)
            {
                if (pos + builtin.Length <= line.Length && line.Substring(pos, builtin.Length) == builtin)
                {
                    char prevChar = pos > 0 ? line[pos - 1] : ' ';
                    char nextChar = pos + builtin.Length < line.Length ? line[pos + builtin.Length] : ' ';
                    
                    if (!char.IsLetterOrDigit(prevChar) && !char.IsLetterOrDigit(nextChar) && prevChar != '_' && nextChar != '_')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetLuaBuiltin(string line, int pos)
        {
            string[] builtins = { 
                "print", "type", "tonumber", "tostring", "pairs", "ipairs", 
                "next", "select", "unpack", "assert", "error", "pcall", 
                "xpcall", "load", "loadstring", "loadfile", "dofile", 
                "require", "setmetatable", "getmetatable", "rawget", "rawset", 
                "rawequal", "rawlen", "gcinfo", "collectgarbage", "newproxy"
            };
            
            foreach (string builtin in builtins)
            {
                if (pos + builtin.Length <= line.Length && line.Substring(pos, builtin.Length) == builtin)
                {
                    char prevChar = pos > 0 ? line[pos - 1] : ' ';
                    char nextChar = pos + builtin.Length < line.Length ? line[pos + builtin.Length] : ' ';
                    
                    if (!char.IsLetterOrDigit(prevChar) && !char.IsLetterOrDigit(nextChar) && prevChar != '_' && nextChar != '_')
                    {
                        return builtin;
                    }
                }
            }
            return "";
        }

        private bool IsLuaFunctionCall(string line, int pos)
        {
            if (pos >= line.Length || !char.IsLetter(line[pos]) && line[pos] != '_') return false;
            
            int endPos = pos;
            while (endPos < line.Length && (char.IsLetterOrDigit(line[endPos]) || line[endPos] == '_'))
            {
                endPos++;
            }
            
            if (endPos < line.Length && line[endPos] == '(')
            {
                return true;
            }
            
            return false;
        }

        private string GetLuaFunctionName(string line, int pos)
        {
            int endPos = pos;
            while (endPos < line.Length && (char.IsLetterOrDigit(line[endPos]) || line[endPos] == '_'))
            {
                endPos++;
            }
            return line.Substring(pos, endPos - pos);
        }

        // Insert a new line at the cursor.
        private void InsertLine()
        {
            string line = lines[currentLine];
            
            if (settings.EnableSmartIndent)
            {
                string currentIndent = GetLineIndent(line);
                
                if (linePos == line.Length)
                {
                    lines.Insert(currentLine + 1, currentIndent);
                }
                else
                {
                    lines.Insert(currentLine + 1, currentIndent + line.Substring(linePos, line.Length - linePos));
                    lines[currentLine] = line.Remove(linePos, line.Length - linePos);
                }
                
                if (IsLuaFile())
                {
                    if (line.TrimEnd().EndsWith("then") || line.TrimEnd().EndsWith("do") || line.TrimEnd().EndsWith("else") || line.TrimEnd().EndsWith("elseif") || line.TrimEnd().EndsWith("repeat"))
                    {
                        lines[currentLine + 1] = lines[currentLine + 1] + "    ";
                    }
                }
            }
            else
            {
                if (linePos == line.Length)
                {
                    lines.Insert(currentLine + 1, string.Empty);
                }
                else
                {
                    lines.Insert(currentLine + 1, line.Substring(linePos, line.Length - linePos));
                    lines[currentLine] = line.Remove(linePos, line.Length - linePos);
                }
            }
            
            updatedLinesStart = currentLine;
            updatedLinesEnd = lines.Count - 1;

            currentLine += 1;
            linePos = lines[currentLine].Length;

            modified = true;
        }

        private string GetLineIndent(string line)
        {
            int indent = 0;
            foreach (char c in line)
            {
                if (c == ' ')
                {
                    indent++;
                }
                else if (c == '\t')
                {
                    indent += 4;
                }
                else
                {
                    break;
                }
            }
            return new string(' ', indent);
        }

        // Insert text at the cursor.
        private void Insert(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                
                c = HandleAutoComplete(c);
                
                lines[currentLine] = lines[currentLine].Insert(linePos, c.ToString());
                linePos++;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        private char HandleAutoComplete(char c)
        {
            string line = lines[currentLine];
            
            if (settings.EnableAutoCompleteBrackets)
            {
                if (c == '(')
                {
                    lines[currentLine] = lines[currentLine].Insert(linePos, ")");
                    return '(';
                }
                else if (c == '[')
                {
                    lines[currentLine] = lines[currentLine].Insert(linePos, "]");
                    return '[';
                }
                else if (c == '{')
                {
                    lines[currentLine] = lines[currentLine].Insert(linePos, "}");
                    return '{';
                }
            }
            
            if (settings.EnableAutoCompleteQuotes)
            {
                if (c == '"')
                {
                    if (linePos > 0 && line[linePos - 1] == '\\')
                    {
                        return '"';
                    }
                    lines[currentLine] = lines[currentLine].Insert(linePos, "\"");
                    return '"';
                }
                else if (c == '\'')
                {
                    if (linePos > 0 && line[linePos - 1] == '\\')
                    {
                        return '\'';
                    }
                    lines[currentLine] = lines[currentLine].Insert(linePos, "'");
                    return '\'';
                }
            }
            
            return c;
        }

        private void InsertTab()
        {
            Insert("    ");
        }

        // Backspace at the cursor.
        private void Backspace()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                linePos = lines[currentLine - 1].Length;
                lines[currentLine - 1] += lines[currentLine];

                updatedLinesStart = currentLine - 1;
                updatedLinesEnd = lines.Count - 1;

                lines.RemoveAt(currentLine);
                currentLine -= 1;
            }
            else
            {
                if (settings.EnableSmartDelete)
                {
                    string line = lines[currentLine];
                    char charToDelete = line[linePos - 1];
                    
                    if (linePos < line.Length && IsMatchingPair(charToDelete, line[linePos]))
                    {
                        lines[currentLine] = lines[currentLine].Remove(linePos, 1);
                    }
                }
                
                lines[currentLine] = lines[currentLine].Remove(linePos - 1, 1);
                linePos--;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        private bool IsMatchingPair(char open, char close)
        {
            return (open == '(' && close == ')') ||
                   (open == '[' && close == ']') ||
                   (open == '{' && close == '}') ||
                   (open == '"' && close == '"') ||
                   (open == '\'' && close == '\'');
        }

        // Move the cursor left.
        private void MoveLeft()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine--;
                linePos = lines[currentLine].Length;
            }
            else
            {
                linePos--;
            }
        }

        // Move the cursor right.
        private void MoveRight()
        {
            if (linePos == lines[currentLine].Length)
            {
                if (currentLine + 1 == lines.Count) return;
                currentLine++;
                linePos = 0;
            }
            else
            {
                linePos++;
            }
        }

        // Move the cursor up.
        private void MoveUp()
        {
            if (currentLine == 0) return;
            currentLine--;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Move the cursor down.
        private void MoveDown()
        {
            if (currentLine + 1 == lines.Count) return;
            currentLine++;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Jump back to the previous word.
        private void JumpToPreviousWord()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine -= 1;
                linePos = lines[currentLine].Length;
                JumpToPreviousWord();
            }
            else
            {
                int res = lines[currentLine].Substring(0, linePos - 1).LastIndexOf(' ');
                linePos = res == -1 ? 0 : res + 1;
            }
        }

        // Jump forward to the next word.
        private void JumpToNextWord()
        {
            int res = lines[currentLine].IndexOf(' ', linePos + 1);
            if (res == -1)
            {
                if (currentLine == lines.Count - 1)
                {
                    linePos = lines[currentLine].Length;
                    return;
                }

                linePos = 0;
                currentLine++;
                while (lines[currentLine] == string.Empty)
                {
                    if (currentLine == lines.Count - 1) break;
                    currentLine++;
                }
            }
            else
            {
                linePos = res + 1;
            }
        }

        private void ShowNotification(string text)
        {
            int consoleWidth = 80;
            int consoleHeight = 25;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition((consoleWidth - text.Length) / 2, consoleHeight - SHORTCUT_BAR_HEIGHT - 1);
            Console.Write($" {text} ");
        }

        private void RenderPrompt(string question, (string, string)[] shortcuts)
        {
            RenderShortcuts(shortcuts);

            int consoleHeight = 25;
            int y = consoleHeight - SHORTCUT_BAR_HEIGHT - 1;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            int consoleWidth = 80;
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', consoleWidth));

            Console.SetCursorPosition(1, y);
            Console.Write(question);

            Console.SetCursorPosition(question.Length + 1, y);
        }

        // Get the entire document as a string.
        private string GetAllText()
        {
            string text = string.Empty;
            foreach (string line in lines)
            {
                text += line + "\n";
            }
            // Strip the trailing newline.
            text = text.Remove(text.Length - 1);

            return text;
        }

        private void Save(bool showFeedback)
        {
            if (path == null)
            {
                RenderPrompt("Path to save to: ", new (string, string)[] { ("Enter", "Save"), ("Esc", "Cancel") });

                string input = "";
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        input = null;
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                        Console.SetCursorPosition(1 + input.Length, Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1);
                        Console.Write(" ");
                        Console.SetCursorPosition(1 + input.Length, Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1);
                    }
                    else if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }

                path = input;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                RenderUI();
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
                Render();

                if (path == null)
                {
                    return;
                }
            }
            else
            {
                if (!modified) return;
            }

            modified = false;
            string text = GetAllText();
            try
            {
                fileSystem.CreateFile(path, text);

                if (showFeedback)
                {
                    RenderUI();
                    ShowNotification($"Saved to {path}");
                }
            }
            catch
            {
                ShowNotification("Failed to save");
                return;
            }
        }

        // Quit, and if the file is modified, prompt to save it.
        private void Quit()
        {
            quit = true;
            if (modified)
            {
                RenderPrompt("Save your changes?", new (string, string)[] { ("Y", "Yes"), ("N", "No"), ("Esc", "Cancel") });
                bool choiceMade = false;
                while (!choiceMade)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            Save(false);
                            choiceMade = true;
                            break;
                        case ConsoleKey.N:
                            choiceMade = true;
                            break;
                        case ConsoleKey.Escape:
                            choiceMade = true;
                            quit = false;

                            // Hide the prompt.
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();
                            RenderUI();
                            updatedLinesStart = 0;
                            updatedLinesEnd = lines.Count - 1;
                            Render();
                            break;
                    }
                }
            }
        }

        // Show information about the document.
        private void ShowInfo()
        {
            ShowNotification($"Ln {currentLine + 1}, Col {linePos + 1}");
        }

        // Cut the current line.
        private void CutLine()
        {
            if (lines[currentLine] != string.Empty)
            {
                clipboard = lines[currentLine];
                if (lines.Count == 1)
                {
                    lines[currentLine] = string.Empty;
                    linePos = 0;

                    updatedLinesStart = 0;
                    updatedLinesEnd = 0;
                }
                else
                {
                    lines.RemoveAt(currentLine);

                    if (currentLine >= lines.Count)
                    {
                        currentLine--;
                    }

                    if (linePos >= lines[currentLine].Length)
                    {
                        linePos = lines[currentLine].Length - 1;
                    }

                    updatedLinesStart = currentLine;
                    updatedLinesEnd = lines.Count;
                }
                modified = true;
            }
            else
            {
                ShowNotification("Nothing was cut");
            }
        }

        // Paste from the clipboard.
        private void Paste()
        {
            if (clipboard != string.Empty)
            {
                Insert(clipboard);
            }
            else
            {
                ShowNotification("Nothing to paste");
            }
        }

        private void RunFile()
        {
            if (path == null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Error: No file is currently open. Please save the file first.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                RenderUI();
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
                Render();
                return;
            }

            string extension = System.IO.Path.GetExtension(path)?.ToLower();

            if (extension == ".lua")
            {
                RunLua();
            }
            else if (extension == ".brs")
            {
                RunBranswe();
            }
            else if (extension == ".cm")
            {
                RunCom();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Error: File extension '{extension}' is not supported for running.");
                Console.WriteLine("Supported file types: .lua, .brs, .cm");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                RenderUI();
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
                Render();
            }
        }

        private void RunLua()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            try
            {
                string source = GetAllText();
                
                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();
                
                UniLua.ThreadStatus loadResult = lua.L_LoadString(source);
                
                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                    
                    if (callResult != UniLua.ThreadStatus.LUA_OK)
                    {
                        string errorMsg = lua.ToString(-1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Script execution error: {errorMsg}");
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Script load error: {errorMsg}");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while running script: {e.Message}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
            Render();
        }

        private void RunBranswe()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            try
            {
                string source = GetAllText();
                
                Branswe.Run(source);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while running Branswe: {e.Message}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
            Render();
        }

        private void RunCom()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            try
            {
                string source = GetAllText();
                
                if (shell != null)
                {
                    shell.ExecuteCommand(source);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Shell is not available.");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while running command: {e.Message}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
            Render();
        }

        private bool HandleInput()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Modifiers)
            {
                case ConsoleModifiers.Control:
                    switch (key.Key)
                    {
                        case ConsoleKey.X:
                            Quit();
                            break;
                        case ConsoleKey.S:
                            Save(true);
                            break;
                        case ConsoleKey.I:
                            ShowInfo();
                            break;
                        case ConsoleKey.K:
                            CutLine();
                            break;
                        case ConsoleKey.V:
                            Paste();
                            break;
                        case ConsoleKey.R:
                            RunFile();
                            break;
                        case ConsoleKey.LeftArrow:
                            JumpToPreviousWord();
                            break;
                        case ConsoleKey.RightArrow:
                            JumpToNextWord();
                            break;
                        case ConsoleKey.UpArrow:
                            currentLine = 0;
                            linePos = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentLine = lines.Count - 1;
                            linePos = lines[currentLine].Length;
                            break;
                    }
                    return false;
            }
            switch (key.Key)
            {
                case ConsoleKey.F2:
                    ShowSettingsMenu();
                    return true;
                case ConsoleKey.Tab:
                    InsertTab();
                    break;
                case ConsoleKey.Backspace:
                    Backspace();
                    break;
                case ConsoleKey.Enter:
                    InsertLine();
                    break;
                case ConsoleKey.LeftArrow:
                    MoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight();
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                default:
                    if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        Insert(key.KeyChar.ToString());
                    }
                    break;
            }
            return false;
        }

        private void UpdateScrolling()
        {
            bool scrollChanged = false;
            int consoleWidth = 80;
            int consoleHeight = 25;

            if (currentLine < scrollY)
            {
                scrollY = currentLine;
                scrollChanged = true;
            }
            else if (currentLine >= scrollY + consoleHeight - TITLEBAR_HEIGHT - SHORTCUT_BAR_HEIGHT)
            {
                scrollY = currentLine - consoleHeight + TITLEBAR_HEIGHT + SHORTCUT_BAR_HEIGHT + 1;
                scrollChanged = true;
            }

            if (linePos < scrollX)
            {
                scrollX = linePos;
                scrollChanged = true;
            }
            else if (linePos > scrollX + consoleWidth - 1)
            {
                scrollX = linePos - consoleWidth + 1;
                scrollChanged = true;
            }

            if (scrollChanged)
            {
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
            }
        }

        private void RenderShortcuts((string, string)[] shortcuts)
        {
            int consoleWidth = 80;
            int y = 24;

            Console.SetCursorPosition(0, y);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', consoleWidth - 1));

            Console.SetCursorPosition(0, y);
            foreach (var shortcut in shortcuts)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{shortcut.Item1}");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {shortcut.Item2} ");
            }
        }

        private void RenderUI()
        {
            int consoleWidth = 80;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            string text = "  CMLeonOS Nano Editor";
            Console.WriteLine(text + new string(' ', consoleWidth - text.Length));

            string displayName = path == null ? "New File" : System.IO.Path.GetFileName(path);
            Console.SetCursorPosition((consoleWidth - displayName.Length) / 2, 0);
            Console.Write(displayName);

            RenderShortcuts(SHORTCUTS);
        }

        private void ShowSettingsMenu()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            int consoleWidth = 80;
            int consoleHeight = 25;
            
            string title = " Nano Settings";
            string[] options = new string[]
            {
                "1. Smart Indent (All Files)",
                "2. Auto Complete Brackets (All Files)",
                "3. Auto Complete Quotes (All Files)",
                "4. Smart Delete (All Files)",
                "5. Syntax Highlight (Lua/Ini/Json)",
                "6. Save and Exit"
            };

            int maxOptionLength = 0;
            foreach (string option in options)
            {
                if (option.Length > maxOptionLength)
                {
                    maxOptionLength = option.Length;
                }
            }

            int menuWidth = maxOptionLength + 10;
            int menuHeight = options.Length + 4;
            int menuX = (consoleWidth - menuWidth) / 2;
            int menuY = (consoleHeight - menuHeight) / 2;

            int titleX = menuX + (menuWidth - title.Length) / 2;
            Console.SetCursorPosition(titleX, menuY);
            Console.Write(title);

            int borderLeft = menuX;
            int borderRight = menuX + menuWidth - 1;
            int borderTop = menuY;
            int borderBottom = menuY + menuHeight - 1;

            for (int i = 0; i < menuWidth; i++)
            {
                Console.SetCursorPosition(borderLeft + i, borderTop);
                Console.Write("-");
            }

            for (int i = 0; i < menuHeight; i++)
            {
                Console.SetCursorPosition(borderLeft, borderTop + 1 + i);
                Console.Write("|");
                Console.SetCursorPosition(borderRight, borderTop + 1 + i);
                Console.Write("|");
            }

            for (int i = 0; i < menuWidth; i++)
            {
                Console.SetCursorPosition(borderLeft + i, borderBottom);
                Console.Write("-");
            }

            for (int i = 0; i < options.Length; i++)
            {
                int optionY = borderTop + 2 + i;
                Console.SetCursorPosition(borderLeft + 2, optionY);
                Console.Write(options[i]);
                
                string status = GetSettingStatus(i);
                int statusX = borderRight - status.Length - 2;
                Console.SetCursorPosition(statusX, optionY);
                Console.Write($"[{status}]");
            }

            int promptY = borderBottom - 2;
            string prompt = "Use 1-6 to toggle, ESC to exit";
            int promptX = menuX + (menuWidth - prompt.Length) / 2;
            Console.SetCursorPosition(promptX, promptY);
            Console.Write(prompt);

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
                else if (key.KeyChar >= '1' && key.KeyChar <= '6')
                {
                    int option = key.KeyChar - '1';
                    ToggleSetting(option);
                    ShowSettingsMenu();
                    break;
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
        }

        private string GetSettingStatus(int option)
        {
            switch (option)
            {
                case 0:
                    return settings.EnableSmartIndent ? "ON" : "OFF";
                case 1:
                    return settings.EnableAutoCompleteBrackets ? "ON" : "OFF";
                case 2:
                    return settings.EnableAutoCompleteQuotes ? "ON" : "OFF";
                case 3:
                    return settings.EnableSmartDelete ? "ON" : "OFF";
                case 4:
                    return settings.EnableSyntaxHighlight ? "ON" : "OFF";
                case 5:
                    return "SAVE";
                default:
                    return "";
            }
        }

        private void ToggleSetting(int option)
        {
            switch (option)
            {
                case 0:
                    settings.EnableSmartIndent = !settings.EnableSmartIndent;
                    break;
                case 1:
                    settings.EnableAutoCompleteBrackets = !settings.EnableAutoCompleteBrackets;
                    break;
                case 2:
                    settings.EnableAutoCompleteQuotes = !settings.EnableAutoCompleteQuotes;
                    break;
                case 3:
                    settings.EnableSmartDelete = !settings.EnableSmartDelete;
                    break;
                case 4:
                    settings.EnableSyntaxHighlight = !settings.EnableSyntaxHighlight;
                    break;
                case 5:
                    break;
            }
        }

        public void Start()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            settings.Load(@"0:\system\nano.dat");
            
            RenderUI();
            while (!quit)
            {
                Render();
                if (pendingNotification != null)
                {
                    ShowNotification(pendingNotification);
                    pendingNotification = null;
                }

                HandleInput();
                UpdateScrolling();
            }
            
            settings.Save(@"0:\system\nano.dat");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    }
}