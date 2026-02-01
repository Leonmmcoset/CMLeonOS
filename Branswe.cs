using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using Console = System.Console;
using Sys = Cosmos.System;

public static class Branswe
//Branswe��ѭMIT����֤��
{
    private static string varlib = "";
    private static string geted = "";
    private static string methods = "";
    public static void Run(string Code)
    {
        //��ʼ��
        string[] codelines = Code.Split("\n");
        long Codelength = codelines.Length;



        for (long coderun = 0; coderun < Codelength; coderun++)
        {
            string line = codelines[coderun];

            // �������к��� # ��ͷ��ע��
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
            {
                continue;
            }
            //��ȡ���ʶ�����ո�ָ���һ����

            switch (line.Split(" ")[0])
            {
                case "method":
                    {
                        string rest = line.Substring(6).Trim();
                        int arrowIndex = rest.IndexOf("<<");

                        string varPart = rest.Substring(0, arrowIndex).Trim();
                        string codePart = rest.Substring(arrowIndex + 2).Trim();

                        // �� ��������������
                        // ��� varPart �Ǳ���������ȡ��ֵ
                        if (!varPart.Contains(" "))
                        {
                            Branswe.Run("ref getvar " + varPart);
                            if (!string.IsNullOrEmpty(geted)) varPart = geted;
                        }

                        // ��� codePart �Ǳ���������ȡ��ֵ
                        if (!codePart.Contains(" "))
                        {
                            Branswe.Run("ref getvar " + codePart);
                            if (!string.IsNullOrEmpty(geted)) codePart = geted;
                        }

                        methods += "\uE001" + varPart + "\uE002" + codePart;
                        break;
                    }



                case "diskfile":
                    {
                        switch (line.Split(" ")[1])
                        {
                            /*case "read":
                                {
                                    Branswe.Run("ref getvar " + line.Split(" ", 5)[2]);
                                    var get0 = geted;
                                    switch (line.Split(" ")[3])
                                    {
                                        case "to":
                                            {
                                                Branswe.Run("ref getvar " + line.Split(" ", 5)[4]);
                                                var get1 = geted;
                                                Branswe.Run("var(text) " + get1 + " = " + VFSManager.GetFile(get0));
                                                break;
                                            }
                                    }
                                    break;
                                }*/
                            case "reg":
                                {
                                    VFSBase vfs = new CosmosVFS();
                                    VFSManager.RegisterVFS(vfs);
                                    break;
                                }
                            case "create":
                                {
                                    switch (line.Split(" ")[2])
                                    {
                                        case "file":
                                            {
                                                Branswe.Run("ref getvar " + line.Split(" ", 4)[3]);
                                                var get = geted;
                                                VFSManager.CreateFile(get);
                                                break;
                                            }
                                        case "dir":
                                            {
                                                Branswe.Run("ref getvar " + line.Split(" ", 4)[3]);
                                                var get = geted;
                                                VFSManager.CreateDirectory(get);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case "write":
                                {
                                    Branswe.Run("ref getvar " + line.Split(" ", 5)[2]);
                                    var get0 = geted;
                                    switch (line.Split(" ", 5)[3])
                                    {
                                        case "to":
                                            {
                                                Branswe.Run("ref getvar " + line.Split(" ", 5)[4]);
                                                var get1 = geted;
                                                File.WriteAllText(get0, get1);
                                                break;
                                            }
                                    }
                                }
                                break;
                        }
                    }
                    break;



                case "rstr": //rstr=Read String To Run
                    {
                        var parts = line.Split(" ", 2);
                        Branswe.Run("ref getvar " + parts[1]);
                        var code = geted;
                        for (long i = 0; i < code.Split("\\n").Length; i++)
                        {
                            Branswe.Run(code.Split("\\n")[i]);
                        }
                        break;
                    }
                case "var()":
                    {
                        var parts = line.Split(" ", 4);

                        if (parts.Length == 3 && parts[2] == "rm")
                        {
                            string varName = parts[1];
                            string[] lines = varlib.Split('\n');
                            string newVarlib = "";

                            foreach (string varlibLine in lines)
                            {
                                if (!string.IsNullOrEmpty(varlibLine))
                                {
                                    if (!varlibLine.StartsWith(varName + "\uE001"))
                                    {
                                        newVarlib = newVarlib + "\n" + varlibLine;
                                    }
                                }
                            }

                            varlib = newVarlib;
                        }
                        else
                        {
                            switch (parts[2])
                            {
                                case "=+":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        long wc0 = long.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        long wc1 = long.Parse(geted);
                                        long calced = wc0 + wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=-":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        long wc0 = long.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        long wc1 = long.Parse(geted);
                                        long calced = wc0 - wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=*":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        long wc0 = long.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        long wc1 = long.Parse(geted);
                                        long calced = wc0 * wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=/":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        long wc0 = long.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        long wc1 = long.Parse(geted);
                                        long calced = wc0 / wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=":
                                    {
                                        Branswe.Run("ref getvar " + parts[3]);
                                        Branswe.Run("var(text) " + parts[1] + " = " + geted);
                                        break;
                                    }
                            }
                        }
                        break;
                    }

                case "var(text)":
                    {
                        var parts = line.Split(" ", 4);
                        switch (parts[2])
                        {
                            case "=":
                                {
                                    Branswe.Run("var() " + parts[1] + " rm");
                                    varlib = varlib + "\n" + parts[1] + "\uE001" + parts[3];
                                    break;
                                }
                        }

                        break;
                    }

                case "conshowl":
                    {
                        Branswe.Run("ref getvar " + line.Split(" ")[1]);
                        Console.WriteLine(geted);
                        break;
                    }
                case "ref":
                    {
                        string[] parts = line.Split(" ");
                        if (parts.Length < 2) break;

                        switch (parts[1])
                        {
                            case "mousex":
                                Branswe.Run("var(text) [] = " + MouseManager.X);
                                break;

                            case "mousey":
                                Branswe.Run("var(text) [] = " + MouseManager.Y);
                                break;

                            case "screenx":
                                Branswe.Run("var(text) [] = " + Console.WindowWidth);
                                break;

                            case "screeny":
                                Branswe.Run("var(text) [] = " + Console.WindowHeight);
                                break;

                            case "concolour-b":
                                Branswe.Run("var(text) [] = " + Console.BackgroundColor);
                                break;

                            case "concolour-f":
                                Branswe.Run("var(text) [] = " + Console.ForegroundColor);
                                break;
                            case "getalldisks":
                                {

                                    Branswe.Run("var(text) [] = " + VFSManager.GetDisks().ToArray());
                                    break;
                                }


                            case "getvar":
                                {
                                    string varName = parts[2];
                                    string value = "";
                                    string[] lines = varlib.Split('\n');
                                    foreach (string lineInLib in lines)
                                    {
                                        if (!string.IsNullOrEmpty(lineInLib) &&
                                            lineInLib.StartsWith(varName + "\uE001"))
                                        {
                                            value = lineInLib.Substring(varName.Length + 1);

                                        }
                                    }
                                    //CSharp:
                                    geted = value;
                                    //Branswe:
                                    Branswe.Run("var(text) [] = " + value);
                                    break;
                                }


                        }
                        break;
                    }



                case "conshow":
                    {
                        Branswe.Run("ref getvar " + line.Split(" ")[1]);
                        Console.Write(geted);
                        break;
                    }
                case "coninput":
                    {
                        Branswe.Run("var(text) " + line.Split(" ")[1] + " = " + Console.ReadLine());
                        break;
                    }


                case "conbeep":
                    {
                        Console.Beep(); //����C#��beep
                        break;
                    }
                case "sleep":
                    {
                        Branswe.Run("ref getvar " + line.Split(" ")[1]);
                        Thread.Sleep(TimeSpan.FromMilliseconds(long.Parse(geted))); //����C# Threading��Sleep
                        break;
                    }
                case "loop":
                    {
                        var parts = line.Split(" ");
                        if (parts[1] == "<<")
                        {
                            while (true)
                            {
                                Branswe.Run("rstr " + parts[2]);
                            }
                        }
                        break;
                    }
                case "end":
                    {
                        throw new Exception("\uE001");
                    }
                case "power":
                    {
                        switch (line.Split(" ")[1])
                        {
                            case "off":
                                {
                                    ACPI.Shutdown();
                                    CPU.Halt();
                                    break;
                                }
                            case "reboot":
                                {
                                    ACPI.Reboot();
                                    CPU.Reboot();
                                    break;
                                }

                        }

                        break;
                    }
                case "decide":
                    {
                        var parts = line.Split(" ");
                        bool IsRight;
                        Branswe.Run("ref getvar " + parts[1]);
                        var left = geted;
                        Branswe.Run("ref getvar " + parts[3]);
                        var right = geted;
                        switch (parts[2])
                        {
                            case "==":
                                {
                                    if (left == right)
                                    {
                                        IsRight = true;
                                    }
                                    else
                                    {
                                        IsRight = false;
                                    }
                                    break;
                                }
                            case "!=":
                                {
                                    if (left == right)
                                    {
                                        IsRight = false;
                                    }
                                    else
                                    {
                                        IsRight = true;
                                    }
                                    break;
                                }
                            case ">":
                                {
                                    if (long.Parse(left) > long.Parse(right))
                                    {
                                        IsRight = true;
                                    }
                                    else
                                    {
                                        IsRight = false;
                                    }
                                    break;
                                }
                            case "<":
                                {
                                    if (long.Parse(left) < long.Parse(right))
                                    {
                                        IsRight = true;
                                    }
                                    else
                                    {
                                        IsRight = false;
                                    }
                                    break;
                                }
                            case ">=":
                                {
                                    if (long.Parse(left) >= long.Parse(right))
                                    {
                                        IsRight = true;
                                    }
                                    else
                                    {
                                        IsRight = false;
                                    }
                                    break;
                                }
                            case "<=":
                                {
                                    if (long.Parse(left) <= long.Parse(right))
                                    {
                                        IsRight = true;
                                    }
                                    else
                                    {
                                        IsRight = false;
                                    }
                                    break;
                                }
                            default:
                                {
                                    IsRight = false;
                                    break;
                                }

                        }
                        if (IsRight)
                        {
                            Branswe.Run("var(text) [] = \uE003");
                        }
                        else
                        {
                            Branswe.Run("var() [] rm");
                        }


                        break;
                    }

                case "if":
                    {
                        //if <bool> then <true_code> else <false_code>
                        //Split: 0    1     2       3        4        5     
                        //Length:6
                        var parts = line.Split(" ");
                        Branswe.Run("ref getvar " + parts[1]); //bool
                        var ifask = geted;
                        if (ifask == "\uE003")
                        {
                            if (parts[2] == "then")
                            {
                                Branswe.Run("rstr " + parts[3]); //then
                            }
                        }
                        else if (parts.Length >= 6)
                        {
                            if (parts[4] == "else")
                            {
                                Branswe.Run("rstr " + parts[5]); //else
                            }
                        }

                        break;
                    }
                case "concls":
                    {
                        Console.Clear();
                        break;
                    }
                default:
                    {
                        var methodEntries = methods.Split('\uE001', StringSplitOptions.RemoveEmptyEntries);

                        for (long i = 0; i < methodEntries.Length; i++)
                        {
                            var parts = methodEntries[i].Split('\uE002');
                            if (parts.Length < 2) continue;

                            string varPart = parts[0];      //keep spaces as is
                            string codeTemplate = parts[1]; //keep spaces

                            var firstToken = varPart.Split(' ')[0];
                            Branswe.Run("ref getvar " + firstToken);
                            string methodName = geted; //no .TrimEnd('(').Trim() 

                            if (line.StartsWith(methodName))
                            {
                                string args = line.Substring(methodName.Length); //keep spaces
                                Branswe.Run("ref getvar " + args);
                                string argValue = geted; //value may contain spaces

                                string finalCode = codeTemplate;
                                var varTokens = varPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                for (long j = 1; j < varTokens.Length; j++)
                                {
                                    var token = varTokens[j];
                                    if (token == "[]")
                                    {
                                        finalCode = finalCode.Replace("[]", argValue);
                                    }
                                    else
                                    {
                                        Branswe.Run("ref getvar " + token);
                                        finalCode = finalCode.Replace(token, geted);
                                    }
                                }

                                Branswe.Run(finalCode);
                                break;
                            }
                        }
                        break;
                    }
            }
        }
    }
}