using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.CodeStudio
{
    internal static class LuaSyntaxHighlighter
    {
        private static readonly List<string> Keywords = new List<string>
        {
            "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
        };

        private static readonly List<string> Builtins = new List<string>
        {
            "print", "type", "tostring", "tonumber", "ipairs", "pairs", "table", "string", "math", "io", "os", "coroutine", "debug", "package", "utf8", "bit32"
        };

        private static readonly List<string> Operators = new List<string>
        {
            "+", "-", "*", "/", "%", "^", "#", "==", "~=", "<=", ">=", "<", ">", "=", "(", ")", "[", "]", "{", "}", ";", ":", ",", "..", "...", "."
        };

        internal static class Colors
        {
            internal static Color Keyword = Color.FromArgb(255, 0, 128);
            internal static Color Builtin = Color.FromArgb(0, 128, 0);
            internal static Color String = Color.FromArgb(163, 21, 21);
            internal static Color Number = Color.FromArgb(0, 0, 255);
            internal static Color Comment = Color.FromArgb(128, 128, 128);
            internal static Color Operator = Color.FromArgb(0, 0, 0);
            internal static Color Default = Color.White;
        }

        internal class HighlightedToken
        {
            internal string Text;
            internal Color Color;

            internal HighlightedToken(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        internal static List<HighlightedToken> HighlightLine(string line)
        {
            var tokens = new List<HighlightedToken>();
            int i = 0;

            while (i < line.Length)
            {
                char c = line[i];

                if (char.IsWhiteSpace(c))
                {
                    int start = i;
                    while (i < line.Length && char.IsWhiteSpace(line[i]))
                    {
                        i++;
                    }
                    tokens.Add(new HighlightedToken(line.Substring(start, i - start), Colors.Default));
                    continue;
                }

                if (c == '-' && i + 1 < line.Length && line[i + 1] == '-')
                {
                    int start = i;
                    i += 2;
                    while (i < line.Length && line[i] != '\n')
                    {
                        i++;
                    }
                    tokens.Add(new HighlightedToken(line.Substring(start, i - start), Colors.Comment));
                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    int start = i;
                    i++;
                    while (i < line.Length && line[i] != c)
                    {
                        if (line[i] == '\\' && i + 1 < line.Length)
                        {
                            i += 2;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (i < line.Length)
                    {
                        i++;
                    }
                    tokens.Add(new HighlightedToken(line.Substring(start, i - start), Colors.String));
                    continue;
                }

                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < line.Length && (char.IsDigit(line[i]) || line[i] == '.'))
                    {
                        i++;
                    }
                    tokens.Add(new HighlightedToken(line.Substring(start, i - start), Colors.Number));
                    continue;
                }

                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '_'))
                    {
                        i++;
                    }
                    string word = line.Substring(start, i - start);

                    if (Keywords.Contains(word))
                    {
                        tokens.Add(new HighlightedToken(word, Colors.Keyword));
                    }
                    else if (Builtins.Contains(word))
                    {
                        tokens.Add(new HighlightedToken(word, Colors.Builtin));
                    }
                    else
                    {
                        tokens.Add(new HighlightedToken(word, Colors.Default));
                    }
                    continue;
                }

                if (Operators.Contains(c.ToString()))
                {
                    tokens.Add(new HighlightedToken(c.ToString(), Colors.Operator));
                    i++;
                    continue;
                }

                tokens.Add(new HighlightedToken(c.ToString(), Colors.Default));
                i++;
            }

            return tokens;
        }
    }
}
