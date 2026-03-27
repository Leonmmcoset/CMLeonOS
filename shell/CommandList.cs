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

namespace CMLeonOS.shell
{
    public static class CommandList
    {
        public static void ProcessCommand(Shell shell, string command, string args)
        {
            switch (command)
            {
                case "echo":
                    shell.ProcessEcho(args);
                    break;
                case "clear":
                case "cls":
                    shell.ProcessClear();
                    break;
                case "restart":
                    shell.ProcessRestart();
                    break;
                case "shutdown":
                    shell.ProcessShutdown();
                    break;
                case "help":
                    shell.ProcessHelp(args);
                    break;
                case "time":
                    shell.ProcessTime();
                    break;
                case "date":
                    shell.ProcessDate();
                    break;
                case "prompt":
                    shell.ChangePrompt(args);
                    break;
                case "calc":
                    shell.Calculate(args);
                    break;
                case "calcgui":
                    shell.ShowCalculatorGUI();
                    break;
                case "history":
                    shell.ShowHistory();
                    break;
                case "background":
                    shell.ChangeBackground(args);
                    break;
                case "cuitest":
                    shell.TestCUI();
                    break;
                case "edit":
                    shell.EditFile(args);
                    break;
                case "nano":
                    shell.NanoFile(args);
                    break;
                case "matrix":
                    shell.ShowMatrix();
                    break;
                case "app":
                    shell.ProcessApp(args);
                    break;
                case "diff":
                    shell.DiffFiles(args);
                    break;
                case "cal":
                    shell.ShowCalendar(args);
                    break;
                case "sleep":
                    shell.SleepCommand(args);
                    break;
                case "com":
                    shell.ExecuteCommandFile(args);
                    break;
                case "ls":
                    shell.ProcessLs(args);
                    break;
                case "cd":
                    shell.ProcessCd(args);
                    break;
                case "pwd":
                    shell.ProcessPwd();
                    break;
                case "mkdir":
                    shell.ProcessMkdir(args);
                    break;
                case "rm":
                    shell.ProcessRm(args);
                    break;
                case "rmdir":
                    shell.ProcessRmdir(args);
                    break;
                case "cat":
                    shell.ProcessCat(args);
                    break;
                case "version":
                    shell.ProcessVersion();
                    break;
                case "settings":
                    shell.ProcessSettings(args);
                    break;
                case "about":
                    shell.ProcessAbout();
                    break;
                case "head":
                    shell.HeadFile(args);
                    break;
                case "tail":
                    shell.TailFile(args);
                    break;
                case "wc":
                    shell.WordCount(args);
                    break;
                case "cp":
                    shell.CopyFile(args);
                    break;
                case "mv":
                    shell.MoveFile(args);
                    break;
                case "rename":
                    shell.RenameFile(args);
                    break;
                case "touch":
                    shell.CreateEmptyFile(args);
                    break;
                case "find":
                    shell.FindFile(args);
                    break;
                case "tree":
                    shell.ShowTree(args);
                    break;
                case "tuitree":
                    shell.ShowTUITree(args);
                    break;
                case "getdisk":
                    shell.GetDiskInfo();
                    break;
                case "user":
                    shell.ProcessUserCommand(args);
                    break;
                case "hostname":
                    shell.ProcessHostnameCommand(args);
                    break;
                case "setdns":
                    shell.SetDnsServer(args);
                    break;
                case "setgateway":
                    shell.SetGateway(args);
                    break;
                case "ipconfig":
                    shell.ShowNetworkConfig();
                    break;
                case "nslookup":
                    shell.NsLookup(args);
                    break;
                case "cpass":
                    shell.ProcessCpass();
                    break;
                case "beep":
                    shell.ProcessBeep();
                    break;
                case "env":
                    shell.ProcessEnvCommand(args);
                    break;
                case "branswe":
                    shell.ProcessBransweCommand(args);
                    break;
                case "uptime":
                    shell.ShowUptime();
                    break;
                case "ps":
                    shell.ShowProcesses();
                    break;
                case "kill":
                    shell.KillProcess(args);
                    break;
                case "grep":
                    shell.GrepFile(args);
                    break;
                case "ftp":
                    shell.CreateFTP();
                    break;
                case "ping":
                    shell.PingIP(args);
                    break;
                case "tcpserver":
                    shell.StartTcpServer(args);
                    break;
                case "tcpclient":
                    shell.ConnectTcpClient(args);
                    break;
                case "wget":
                    shell.DownloadFile(args);
                    break;
                case "whoami":
                    shell.ShowCurrentUsername();
                    break;
                case "base64":
                    shell.ProcessBase64Command(args);
                    break;
                case "lua":
                    shell.ExecuteLuaScript(args);
                    break;
                case "lua2cla":
                    shell.ConvertLuaToCla(args);
                    break;
                case "cla":
                    shell.RunClaFile(args);
                    break;
                case "testgui":
                    shell.ProcessTestGui();
                    break;
                case "testtui":
                    shell.ProcessTestTUI();
                    break;
                case "labyrinth":
                    shell.ProcessLabyrinth();
                    break;
                case "snake":
                    shell.PlaySnake();
                    break;
                case "hex":
                    shell.EditHexFile(args);
                    break;
                case "markit":
                    shell.ProcessMarkit(args);
                    break;
                case "runbin":
                    shell.ProcessRunbin(args);
                    break;
                case "femboy":
                    shell.ProcessFemboy();
                    break;
                case "boom":
                    shell.ProcessBoom();
                    break;
                case "alias":
                    shell.ProcessAlias(args);
                    break;
                case "unalias":
                    shell.ProcessUnalias(args);
                    break;
                case "logs":
                    shell.ProcessLogs(args);
                    break;
                case "exportbackground":
                    shell.ProcessExportBackground(args);
                    break;
                case "exporttestexe":
                    shell.ProcessExportTestExe(args);
                    break;
                default:
                    shell.ShowError($"Unknown command: {command}");
                    break;
            }
        }
    }
}
