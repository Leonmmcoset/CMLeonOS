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
    public class CUI
    {
        private string title;
        private string status;
        private ConsoleColor backgroundColor;
        private ConsoleColor textColor;

        public CUI(string title = "CMLeonOS")
        {
            this.title = title;
            this.status = "Ready";
            this.backgroundColor = ConsoleColor.Black;
            this.textColor = ConsoleColor.White;
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void SetStatus(string status)
        {
            this.status = status;
        }

        public void SetBackgroundColor(ConsoleColor color)
        {
            this.backgroundColor = color;
        }

        public void SetTextColor(ConsoleColor color)
        {
            this.textColor = color;
        }

        public void Render()
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;
            Console.Clear();

            RenderTopBar();

            // 重置颜色
            Console.ResetColor();
        }

        public void RenderBottomBar()
        {
            // 渲染底栏
            RenderBottomBarImpl();
        }

        private void RenderTopBar()
        {
            // 简化顶栏渲染，确保在Cosmos环境中正确显示
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            
            // 使用固定长度的顶栏，避免宽度计算问题
            string topBar = new string('─', 80); // 假设标准宽度为80
            
            Console.WriteLine(topBar);
            
            // 居中显示标题和状态
            string titleLine = $"{title.PadRight(60)}{status}";
            if (titleLine.Length > 80)
            {
                titleLine = titleLine.Substring(0, 80);
            }
            Console.WriteLine(titleLine);
            
            Console.WriteLine(topBar);
            
            Console.ResetColor();
        }

        private void RenderBottomBarImpl()
        {
            // 简化底栏渲染，确保在Cosmos环境中正确显示
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            
            // 使用固定长度的底栏，避免宽度计算问题
            string bottomBar = new string('─', 80); // 假设标准宽度为80
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            
            Console.WriteLine(bottomBar);
            
            // 显示帮助信息和时间
            string timeText = DateTime.Now.ToShortTimeString();
            string bottomLine = $"Press help for available commands{(new string(' ', 80 - 35 - timeText.Length))}{timeText}";
            if (bottomLine.Length > 80)
            {
                bottomLine = bottomLine.Substring(0, 80);
            }
            Console.WriteLine(bottomLine);
            
            Console.WriteLine(bottomBar);
            
            Console.ResetColor();
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void ShowError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {error}");
            Console.ResetColor();
        }

        public void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Success: {message}");
            Console.ResetColor();
        }

        public string Prompt(string promptText)
        {
            Console.Write($"{promptText}: ");
            return Console.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
            Render();
        }
    }
}