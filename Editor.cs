using System;
using System.Collections.Generic;

namespace CMLeonOS
{
    // 简单的代码编辑器类
    // 支持基本的文本编辑功能，如输入、删除、移动光标等
    // 支持文件的加载和保存
    public class Editor
    {
        private string fileName;         // 要编辑的文件名
        private List<string> lines;      // 文件内容的行列表
        private int currentLine;         // 当前光标所在行
        private int currentColumn;       // 当前光标所在列
        private bool isRunning;          // 编辑器是否正在运行
        private FileSystem fileSystem;   // 文件系统实例，用于读写文件

        // 构造函数
        // 参数：fileName - 要编辑的文件名
        // 参数：fs - 文件系统实例
        public Editor(string fileName, FileSystem fs)
        {
            this.fileName = fileName;
            this.fileSystem = fs;
            this.lines = new List<string>();
            this.currentLine = 0;
            this.currentColumn = 0;
            this.isRunning = true;
            
            LoadFile();
        }

        // 运行编辑器
        // 主循环：渲染界面 -> 处理输入 -> 重复
        public void Run()
        {
            while (isRunning)
            {
                Render();
                HandleInput();
            }
        }

        // 加载文件内容
        private void LoadFile()
        {
            try
            {
                // 尝试从文件系统读取文件内容
                string content = fileSystem.ReadFile(fileName);
                if (!string.IsNullOrEmpty(content))
                {
                    // 分割内容为行
                    string[] fileLines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (string line in fileLines)
                    {
                        lines.Add(line);
                    }
                }
                else
                {
                    // 文件不存在或为空，创建新文件
                    lines.Add("");
                }
            }
            catch
            {
                // 文件不存在，创建新文件
                lines.Add("");
            }
        }

        // 保存文件内容
        private void SaveFile()
        {
            try
            {
                // 简化实现，避免可能导致栈损坏的操作
                string content = "";
                for (int i = 0; i < lines.Count; i++)
                {
                    content += lines[i];
                    if (i < lines.Count - 1)
                    {
                        content += "\n";
                    }
                }
                
                // 直接使用CreateFile，因为它会覆盖现有文件
                try
                {
                    fileSystem.CreateFile(fileName, content);
                    Console.WriteLine($"File saved: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveFile: {ex.Message}");
            }
        }

        // 渲染编辑器界面
        private void Render()
        {
            Console.Clear();
            
            // 显示标题栏
            Console.WriteLine($"CMLeonOS Editor - {fileName}");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Commands: Esc = Exit");
            Console.WriteLine();
            
            // 显示文件内容（限制显示行数，避免超出控制台高度）
            int maxVisibleLines = 15; // 假设控制台高度为25行，留10行给其他内容
            int startLine = Math.Max(0, currentLine - maxVisibleLines / 2);
            int endLine = Math.Min(lines.Count, startLine + maxVisibleLines);
            
            for (int i = startLine; i < endLine; i++)
            {
                string line = lines[i];
                Console.WriteLine($"{i + 1}: {line}");
            }
            
            // 显示光标位置
            Console.WriteLine();
            Console.WriteLine($"Cursor: Line {currentLine + 1}, Column {currentColumn + 1}");
            Console.WriteLine($"Lines: {lines.Count}");
            
            // 计算光标位置，确保不超出控制台高度
            int consoleHeight = 25; // 假设控制台高度为25行
            int titleLines = 4; // 标题栏占用的行数
            int visibleLines = endLine - startLine;
            int cursorY = titleLines + (currentLine - startLine);
            
            // 确保光标Y位置在有效范围内
            cursorY = Math.Max(titleLines, Math.Min(cursorY, consoleHeight - 5)); // 留5行给状态信息
            
            // 设置光标位置
            try
            {
                Console.SetCursorPosition(currentColumn + 3, cursorY);
            }
            catch
            {
                // 如果设置光标位置失败，忽略错误
            }
        }

        // 处理用户输入
        private void HandleInput()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    // 显示保存确认弹窗
                    ShowSaveConfirmation();
                    break;
                case ConsoleKey.Backspace:
                    // 删除字符
                    if (currentColumn > 0)
                    {
                        string backspaceLineText = lines[currentLine];
                        string backspaceModifiedLine = backspaceLineText.Remove(currentColumn - 1, 1);
                        lines[currentLine] = backspaceModifiedLine;
                        currentColumn--;
                    }
                    else if (currentLine > 0)
                    {
                        // 如果在行首，合并到上一行
                        string previousLine = lines[currentLine - 1];
                        string lineText = lines[currentLine];
                        lines.RemoveAt(currentLine);
                        currentLine--;
                        currentColumn = previousLine.Length;
                        lines[currentLine] = previousLine + lineText;
                    }
                    break;
                case ConsoleKey.Enter:
                    // 插入换行
                    string enterLineText = lines[currentLine];
                    string enterFirstPart = enterLineText.Substring(0, currentColumn);
                    string enterSecondPart = enterLineText.Substring(currentColumn);
                    lines[currentLine] = enterFirstPart;
                    lines.Insert(currentLine + 1, enterSecondPart);
                    currentLine++;
                    currentColumn = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    // 左移光标
                    if (currentColumn > 0)
                    {
                        currentColumn--;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    // 右移光标
                    if (currentColumn < lines[currentLine].Length)
                    {
                        currentColumn++;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    // 上移光标
                    if (currentLine > 0)
                    {
                        currentLine--;
                        currentColumn = Math.Min(currentColumn, lines[currentLine].Length);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    // 下移光标
                    if (currentLine < lines.Count - 1)
                    {
                        currentLine++;
                        currentColumn = Math.Min(currentColumn, lines[currentLine].Length);
                    }
                    break;
                case ConsoleKey.Tab:
                    // Tab键插入四个空格
                    string tabLineText = lines[currentLine];
                    string tabSpaces = "    ";
                    string tabModifiedLine = tabLineText.Insert(currentColumn, tabSpaces);
                    lines[currentLine] = tabModifiedLine;
                    currentColumn += 4;
                    break;
                default:
                    // 输入字符
                    if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar) || char.IsSymbol(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
                    {
                        string defaultLineText = lines[currentLine];
                        string defaultModifiedLine = defaultLineText.Insert(currentColumn, keyInfo.KeyChar.ToString());
                        lines[currentLine] = defaultModifiedLine;
                        currentColumn++;
                    }
                    break;
            }
        }

        // 显示保存确认弹窗
        private void ShowSaveConfirmation()
        {
            // 简化版本，避免使用可能在Cosmos中不支持的功能
            Console.Clear();
            
            // 显示灰色背景的确认弹窗
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            
            // 获取控制台窗口大小
            int consoleWidth = 80; // 假设控制台宽度为80
            int consoleHeight = 25; // 假设控制台高度为25
            
            // 弹窗内容
            string[] popupLines = {
                "+----------------------------+",
                "|          Save File?        |",
                "+----------------------------+",
                "| Do you want to save the    |",
                "| changes?                   |",
                "+----------------------------+",
                "| Y = Yes, N = No            |",
                "+----------------------------+"
            };
            
            // 计算弹窗宽度和高度
            int popupWidth = 30; // 弹窗宽度
            int popupHeight = popupLines.Length; // 弹窗高度
            
            // 计算弹窗在屏幕中的位置（居中）
            int popupX = (consoleWidth - popupWidth) / 2;
            int popupY = (consoleHeight - popupHeight) / 2;
            
            // 显示弹窗
            for (int i = 0; i < popupLines.Length; i++)
            {
                Console.SetCursorPosition(popupX, popupY + i);
                Console.WriteLine(popupLines[i]);
            }
            
            // 等待用户输入
            while (true)
            {
                ConsoleKeyInfo keyInfo;
                try
                {
                    keyInfo = Console.ReadKey(true);
                }
                catch
                {
                    // 如果ReadKey失败，直接退出
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    return;
                }
                
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    // 保存文件
                    try
                    {
                        SaveFile();
                    }
                    catch
                    {
                        Console.WriteLine("Error saving file");
                    }
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    // 不保存文件
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    break;
                }
            }
        }
    }
}
