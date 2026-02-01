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
{
    private static string varlib = "";
    private static string geted = "";
    private static string runlib = "";
    private static string methods = "";
    public static void Run(string Code)
    {
        //��ʼ��
        string[] codelines = Code.Split("\n");
        int Codelength = codelines.Length;



        for (int coderun = 0; coderun < Codelength; coderun++)
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
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: method command is not compatible with CMLeonOS");
                    break;



                case "diskfile":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: diskfile command is not compatible with CMLeonOS");
                    break;



                case "rstr": //rstr=Read String To Run
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: rstr command is not compatible with CMLeonOS");
                    break;
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
                                        int wc0 = int.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        int wc1 = int.Parse(geted);
                                        int calced = wc0 + wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=-":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        int wc0 = int.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        int wc1 = int.Parse(geted);
                                        int calced = wc0 - wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=*":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        int wc0 = int.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        int wc1 = int.Parse(geted);
                                        int calced = wc0 * wc1;
                                        Branswe.Run("var(text) " + parts[1] + " = " + calced);
                                        break;
                                    }
                                case "=/":
                                    {
                                        Branswe.Run("ref getvar " + parts[1]);
                                        int wc0 = int.Parse(geted);
                                        Branswe.Run("ref getvar " + parts[3]);
                                        int wc1 = int.Parse(geted);
                                        int calced = wc0 / wc1;
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
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: mousex command is not compatible with CMLeonOS");
                    break;
                case "mousey":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: mousey command is not compatible with CMLeonOS");
                    break;
                case "screenx":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: screenx command is not compatible with CMLeonOS");
                    break;
                case "screeny":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: screeny command is not compatible with CMLeonOS");
                    break;
                case "concolour-b":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: concolour-b command is not compatible with CMLeonOS");
                    break;
                case "concolour-f":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: concolour-f command is not compatible with CMLeonOS");
                    break;
                            case "getalldisks":
                                {

                                    Branswe.Run("var(text) [] = " + VFSManager.GetDisks().ToArray());
                                    break;
                                }
                            case "to":
                                {
                                    switch (parts[2])
                                    {
                                        case "raw":
                                            {
                                                Branswe.Run("ref getvar " + parts[3]);
                                                Branswe.Run("var(text) [] = " + byte.Parse(geted).ToString());
                                                break;
                                            }
                                            /*case "int":
                                                {
                                                    int.Parse(parts[3]);
                                                    break;
                                                }
                                            case "long":
                                                {
                                                    long.Parse(parts[3]);
                                                    break;
                                                }
                                            case "decimal":
                                                {
                                                    decimal.Parse(parts[3]);
                                                    break;
                                                }*/
                                    }
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
                        Thread.Sleep(TimeSpan.FromMilliseconds(long.Parse(geted)));//����C# Threading��Sleep
                        break;
                    }
                /*case "if":
                    {
                        if ()
                            break;
                    }*/
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
                case "canvas":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: canvas command is not compatible with CMLeonOS");
                    break;
                case "end":
                    {
                        throw new Exception("\uE001");
                    }
                /*case "graphics":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: graphics command is not compatible with CMLeonOS");
                    break;*/
                case "power":
                    // 与CMLeonOS不兼容，删除此功能
                    Console.WriteLine("Error: power command is not compatible with CMLeonOS");
                    break;
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
                /*case "conchange":
                    {
                        switch (line.Split(" ")[1])
                        {
                            case "bgcolor":
                                {
                                    Console.BackgroundColor = Color.FromArgb(int.Parse(line.Split(" ")[2]));
                                    break;
                                }
                        }
                    }*/
                default:
                    {
                        var methodEntries = methods.Split('\uE001', StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < methodEntries.Length; i++)
                        {
                            var parts = methodEntries[i].Split('\uE002');
                            if (parts.Length < 2) continue;

                            string varPart = parts[0].Trim();      // "a c b"
                            string codeTemplate = parts[1].Trim(); // "d"

                            // 1. �ƶϷ��������� varPart �ĵ�һ��������
                            var firstToken = varPart.Split(' ')[0];
                            Branswe.Run("ref getvar " + firstToken);
                            string methodName = geted.TrimEnd('(').Trim(); // "print(" �� "print"

                            // 2. ����Ƿ�ƥ��
                            if (line.StartsWith(methodName))
                            {
                                // 3. ��ȡ����
                                string args = line.Substring(methodName.Length).Trim();
                                if (args.StartsWith("(") && args.EndsWith(")"))
                                {
                                    args = args.Substring(1, args.Length - 2);
                                }

                                // 4. �������մ���
                                string finalCode = codeTemplate;
                                var varTokens = varPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                // �ȴ��������滻
                                for (int j = 1; j < varTokens.Length; j++) // ������һ������������
                                {
                                    var token = varTokens[j];
                                    if (token == "[]")
                                    {
                                        // varPart �е� [] �Ѿ��� args ʹ����
                                        // codeTemplate �п���Ҳ�� []����Ҫ�滻
                                        finalCode = finalCode.Replace("[]", args);
                                    }
                                    else
                                    {
                                        Branswe.Run("ref getvar " + token);
                                        finalCode = finalCode.Replace(token, geted);
                                    }
                                }

                                // 5. ִ��
                                Branswe.Run(finalCode);
                                return;
                            }
                        }

                        // û��ƥ��ķ���������ֱ��ִ��
                        Branswe.Run(line);
                        break;
                    }
                case "cat":
                    {
                        // 与CMLeonOS兼容，支持cat命令
                        var parts = line.Split(" ", 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Error: Please specify file name");
                            Console.WriteLine("Usage: cat <filename>");
                            break;
                        }
                        
                        string filePath = parts[1];
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"Error: File not found: {filePath}");
                            break;
                        }
                        
                        try
                        {
                            string content = File.ReadAllText(filePath);
                            Console.WriteLine(content);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading file: {ex.Message}");
                        }
                        break;
                    }
                case "echo":
                    {
                        // 与CMLeonOS兼容，支持echo命令
                        var parts = line.Split(" ", 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Error: Please specify text");
                            Console.WriteLine("Usage: echo <text>");
                            break;
                        }
                        
                        try
                        {
                            File.WriteAllText(parts[1], parts[0]);
                            Console.WriteLine("Text written successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error writing file: {ex.Message}");
                        }
                        break;
                    }
                case "ls":
                    {
                        // 与CMLeonOS兼容，支持ls命令
                        var parts = line.Split(" ", 2);
                        string dirPath = parts.Length >= 2 ? parts[1] : "";
                        
                        try
                        {
                            if (Directory.Exists(dirPath))
                            {
                                var files = Directory.GetFiles(dirPath);
                                var dirs = Directory.GetDirectories(dirPath);
                                
                                foreach (var file in files)
                                {
                                    Console.WriteLine($"  {file}");
                                }
                                foreach (var dir in dirs)
                                {
                                    Console.WriteLine($"  [{dir}]/");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Error: Directory not found: {dirPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error listing directory: {ex.Message}");
                        }
                        break;
                    }
                case "pwd":
                    {
                        // 与CMLeonOS兼容，支持pwd命令
                        Console.WriteLine("0:\\");
                        break;
                    }
                case "mkdir":
                    {
                        // 与CMLeonOS兼容，支持mkdir命令
                        var parts = line.Split(" ", 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Error: Please specify directory name");
                            Console.WriteLine("Usage: mkdir <dirname>");
                            break;
                        }
                        
                        try
                        {
                            Directory.CreateDirectory(parts[1]);
                            Console.WriteLine($"Directory created: {parts[1]}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating directory: {ex.Message}");
                        }
                        break;
                    }
                case "rm":
                    {
                        // 与CMLeonOS兼容，支持rm命令
                        var parts = line.Split(" ", 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Error: Please specify file name");
                            Console.WriteLine("Usage: rm <filename>");
                            break;
                        }
                        
                        string filePath = parts[1];
                        try
                        {
                            File.Delete(filePath);
                            Console.WriteLine($"File deleted: {filePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file: {ex.Message}");
                        }
                        break;
                    }
                case "rmdir":
                    {
                        // 与CMLeonOS兼容，支持rmdir命令
                        var parts = line.Split(" ", 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Error: Please specify directory name");
                            Console.WriteLine("Usage: rmdir <dirname>");
                            break;
                        }
                        
                        try
                        {
                            Directory.Delete(parts[1]);
                            Console.WriteLine($"Directory deleted: {parts[1]}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting directory: {ex.Message}");
                        }
                        break;
                    }
                // default:
                //     {
                //         // û��ƥ��ķ���������ֱ��ִ��
                //         Branswe.Run(line);
                //         break;
                //     }
            }
        }
    }
}