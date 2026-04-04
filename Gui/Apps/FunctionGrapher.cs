using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class FunctionGrapher : Process
    {
        internal FunctionGrapher() : base("Function Grapher", ProcessType.Application) { }

        private sealed class FunctionSeries
        {
            internal string Expression = "x";
            internal string ColorName = "Blue";
            internal Color Color = Color.FromArgb(44, 102, 217);
        }

        private sealed class GraphPoint
        {
            internal double X;
            internal double Y;
            internal string ColorName = "Red";
            internal Color Color = Color.FromArgb(220, 74, 74);
        }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly List<FunctionSeries> functions = new List<FunctionSeries>();
        private readonly List<GraphPoint> points = new List<GraphPoint>();

        private AppWindow window;
        private Window canvas;

        private TextBox expressionBox;
        private Dropdown functionColorDropdown;
        private Button addFunctionButton;
        private Button plotButton;
        private Button removeFunctionButton;
        private Table functionTable;

        private TextBox pointXBox;
        private TextBox pointYBox;
        private Dropdown pointColorDropdown;
        private Button addPointButton;
        private Button clearPointsButton;
        private Table pointTable;

        private NumericUpDown scaleInput;
        private TextBox statusBox;

        private readonly Dictionary<string, Color> palette = new Dictionary<string, Color>
        {
            { "Blue", Color.FromArgb(44, 102, 217) },
            { "Red", Color.FromArgb(220, 74, 74) },
            { "Green", Color.FromArgb(58, 166, 108) },
            { "Orange", Color.FromArgb(236, 145, 53) },
            { "Purple", Color.FromArgb(145, 97, 204) },
            { "Teal", Color.FromArgb(46, 164, 176) },
            { "Black", Color.FromArgb(44, 44, 52) },
            { "Pink", Color.FromArgb(223, 88, 168) }
        };

        private readonly Color canvasBg = Color.FromArgb(250, 252, 255);
        private readonly Color axisColor = Color.FromArgb(156, 170, 188);
        private readonly Color gridColor = Color.FromArgb(226, 233, 244);

        private void Layout()
        {
            int pad = 8;
            int top = 34;
            int inputH = 24;
            int leftPanelWidth = 322;
            int panelX = pad;
            int panelY = top;

            expressionBox.MoveAndResize(panelX, panelY, 138, inputH);
            functionColorDropdown.MoveAndResize(panelX + 142, panelY, 82, inputH);
            addFunctionButton.MoveAndResize(panelX + 228, panelY, 42, inputH);
            removeFunctionButton.MoveAndResize(panelX + 274, panelY, 40, inputH);

            functionTable.MoveAndResize(panelX, panelY + inputH + 6, leftPanelWidth - pad, 150);

            int pointTop = panelY + inputH + 6 + 150 + 8;
            pointXBox.MoveAndResize(panelX, pointTop, 66, inputH);
            pointYBox.MoveAndResize(panelX + 70, pointTop, 66, inputH);
            pointColorDropdown.MoveAndResize(panelX + 140, pointTop, 84, inputH);
            addPointButton.MoveAndResize(panelX + 228, pointTop, 42, inputH);
            clearPointsButton.MoveAndResize(panelX + 274, pointTop, 40, inputH);

            int pointTableY = pointTop + inputH + 6;
            int pointTableHeight = window.Height - pointTableY - 62;
            pointTable.MoveAndResize(panelX, pointTableY, leftPanelWidth - pad, Math.Max(90, pointTableHeight));

            int rightX = panelX + leftPanelWidth + 4;
            int rightWidth = window.Width - rightX - pad;

            scaleInput.MoveAndResize(rightX, top, 74, inputH);
            plotButton.MoveAndResize(rightX + 80, top, 64, inputH);

            int canvasY = top + inputH + 8;
            int statusH = 24;
            int canvasHeight = window.Height - canvasY - statusH - 10;
            canvas.MoveAndResize(rightX, canvasY, rightWidth, canvasHeight);
            statusBox.MoveAndResize(rightX, window.Height - statusH - 6, rightWidth, statusH);
        }

        private void SetStatus(string text)
        {
            statusBox.Text = text ?? string.Empty;
            statusBox.Render();
            wm.Update(statusBox);
        }

        private string[] ColorNames()
        {
            string[] keys = new string[palette.Count];
            int i = 0;
            foreach (var kv in palette)
            {
                keys[i++] = kv.Key;
            }
            return keys;
        }

        private Color ResolveColor(string name)
        {
            if (name != null && palette.TryGetValue(name, out Color c))
            {
                return c;
            }
            return palette["Blue"];
        }

        private void RefreshFunctionTable()
        {
            functionTable.Cells.Clear();
            for (int i = 0; i < functions.Count; i++)
            {
                FunctionSeries f = functions[i];
                string line = $"{f.ColorName}: y={f.Expression}";
                if (line.Length > 34)
                {
                    line = line.Substring(0, 34) + "...";
                }
                functionTable.Cells.Add(new TableCell(line, i));
            }
            functionTable.Render();
        }

        private void RefreshPointTable()
        {
            pointTable.Cells.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                GraphPoint p = points[i];
                string line = $"{p.ColorName}: ({p.X}, {p.Y})";
                if (line.Length > 34)
                {
                    line = line.Substring(0, 34) + "...";
                }
                pointTable.Cells.Add(new TableCell(line, i));
            }
            pointTable.Render();
        }

        private void DrawGridAndAxes(int centerX, int centerY, int pixelPerUnit)
        {
            int step = Math.Max(4, pixelPerUnit);

            for (int x = centerX; x < canvas.Width; x += step)
            {
                canvas.DrawLine(x, 0, x, canvas.Height - 1, gridColor);
            }
            for (int x = centerX; x >= 0; x -= step)
            {
                canvas.DrawLine(x, 0, x, canvas.Height - 1, gridColor);
            }
            for (int y = centerY; y < canvas.Height; y += step)
            {
                canvas.DrawLine(0, y, canvas.Width - 1, y, gridColor);
            }
            for (int y = centerY; y >= 0; y -= step)
            {
                canvas.DrawLine(0, y, canvas.Width - 1, y, gridColor);
            }

            canvas.DrawLine(0, centerY, canvas.Width - 1, centerY, axisColor);
            canvas.DrawLine(centerX, 0, centerX, canvas.Height - 1, axisColor);
        }

        private void DrawFunctions(int centerX, int centerY, int pixelPerUnit)
        {
            for (int fi = 0; fi < functions.Count; fi++)
            {
                FunctionSeries series = functions[fi];
                ExpressionParser parser = new ExpressionParser(series.Expression);
                if (!parser.Parse())
                {
                    SetStatus("Parse error in: " + series.Expression + " (" + parser.ErrorMessage + ")");
                    continue;
                }

                bool hasLast = false;
                int lastX = 0;
                int lastY = 0;

                for (int sx = 0; sx < canvas.Width; sx++)
                {
                    double x = (sx - centerX) / (double)pixelPerUnit;
                    double y;
                    if (!parser.TryEvaluate(x, out y))
                    {
                        hasLast = false;
                        continue;
                    }

                    if (double.IsNaN(y) || double.IsInfinity(y))
                    {
                        hasLast = false;
                        continue;
                    }

                    int sy = centerY - (int)(y * pixelPerUnit);
                    if (sy < -10000 || sy > 10000)
                    {
                        hasLast = false;
                        continue;
                    }

                    if (hasLast && Math.Abs(sy - lastY) < canvas.Height)
                    {
                        canvas.DrawLine(lastX, lastY, sx, sy, series.Color);
                    }

                    hasLast = true;
                    lastX = sx;
                    lastY = sy;
                }
            }
        }

        private void DrawPoints(int centerX, int centerY, int pixelPerUnit)
        {
            for (int i = 0; i < points.Count; i++)
            {
                GraphPoint p = points[i];
                int sx = centerX + (int)(p.X * pixelPerUnit);
                int sy = centerY - (int)(p.Y * pixelPerUnit);
                if (sx < 1 || sx >= canvas.Width - 1 || sy < 1 || sy >= canvas.Height - 1)
                {
                    continue;
                }

                canvas.DrawFilledRectangle(sx - 2, sy - 2, 5, 5, p.Color);
                canvas.DrawRectangle(sx - 2, sy - 2, 5, 5, Color.Black);
            }
        }

        private void RenderGraph()
        {
            if (canvas.Width < 8 || canvas.Height < 8)
            {
                return;
            }

            canvas.Clear(canvasBg);
            int centerX = canvas.Width / 2;
            int centerY = canvas.Height / 2;
            int pixelPerUnit = Math.Max(8, scaleInput.Value);

            DrawGridAndAxes(centerX, centerY, pixelPerUnit);
            DrawFunctions(centerX, centerY, pixelPerUnit);
            DrawPoints(centerX, centerY, pixelPerUnit);

            wm.Update(canvas);
            SetStatus($"Functions: {functions.Count}, Points: {points.Count}");
        }

        private void AddFunction()
        {
            string expr = (expressionBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(expr))
            {
                SetStatus("Expression cannot be empty.");
                return;
            }

            ExpressionParser parser = new ExpressionParser(expr);
            if (!parser.Parse())
            {
                SetStatus("Parse error: " + parser.ErrorMessage);
                return;
            }

            string colorName = functionColorDropdown.SelectedIndex >= 0 ? functionColorDropdown.SelectedText : "Blue";
            FunctionSeries item = new FunctionSeries
            {
                Expression = expr,
                ColorName = colorName,
                Color = ResolveColor(colorName)
            };
            functions.Add(item);

            RefreshFunctionTable();
            RenderGraph();
        }

        private void RemoveSelectedFunction()
        {
            int idx = functionTable.SelectedCellIndex;
            if (idx < 0 || idx >= functions.Count)
            {
                SetStatus("Select a function first.");
                return;
            }
            functions.RemoveAt(idx);
            RefreshFunctionTable();
            RenderGraph();
        }

        private void AddPoint()
        {
            if (!double.TryParse((pointXBox.Text ?? string.Empty).Trim(), out double x))
            {
                SetStatus("Invalid X.");
                return;
            }
            if (!double.TryParse((pointYBox.Text ?? string.Empty).Trim(), out double y))
            {
                SetStatus("Invalid Y.");
                return;
            }

            string colorName = pointColorDropdown.SelectedIndex >= 0 ? pointColorDropdown.SelectedText : "Red";
            GraphPoint p = new GraphPoint
            {
                X = x,
                Y = y,
                ColorName = colorName,
                Color = ResolveColor(colorName)
            };
            points.Add(p);
            RefreshPointTable();
            RenderGraph();
        }

        private void ClearPoints()
        {
            points.Clear();
            RefreshPointTable();
            RenderGraph();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 120, 70, 980, 600);
            window.Title = "Function Grapher";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = () =>
            {
                Layout();
                RenderGraph();
            };
            window.Closing = TryStop;
            wm.AddWindow(window);

            expressionBox = new TextBox(window, 0, 0, 1, 1);
            expressionBox.Text = "sin(x)";
            expressionBox.PlaceholderText = "f(x)";
            expressionBox.Submitted = AddFunction;
            wm.AddWindow(expressionBox);

            functionColorDropdown = new Dropdown(window, 0, 0, 1, 1);
            string[] colors = ColorNames();
            for (int i = 0; i < colors.Length; i++)
            {
                functionColorDropdown.Items.Add(colors[i]);
            }
            functionColorDropdown.RefreshItems();
            functionColorDropdown.SelectedIndex = 0;
            wm.AddWindow(functionColorDropdown);

            addFunctionButton = new Button(window, 0, 0, 1, 1);
            addFunctionButton.Text = "Add";
            addFunctionButton.OnClick = (_, _) => AddFunction();
            wm.AddWindow(addFunctionButton);

            removeFunctionButton = new Button(window, 0, 0, 1, 1);
            removeFunctionButton.Text = "Del";
            removeFunctionButton.OnClick = (_, _) => RemoveSelectedFunction();
            wm.AddWindow(removeFunctionButton);

            functionTable = new Table(window, 0, 0, 1, 1);
            functionTable.CellHeight = 24;
            functionTable.AllowDeselection = true;
            wm.AddWindow(functionTable);

            pointXBox = new TextBox(window, 0, 0, 1, 1);
            pointXBox.Text = "0";
            pointXBox.PlaceholderText = "x";
            wm.AddWindow(pointXBox);

            pointYBox = new TextBox(window, 0, 0, 1, 1);
            pointYBox.Text = "0";
            pointYBox.PlaceholderText = "y";
            wm.AddWindow(pointYBox);

            pointColorDropdown = new Dropdown(window, 0, 0, 1, 1);
            for (int i = 0; i < colors.Length; i++)
            {
                pointColorDropdown.Items.Add(colors[i]);
            }
            pointColorDropdown.RefreshItems();
            pointColorDropdown.SelectedIndex = 1;
            wm.AddWindow(pointColorDropdown);

            addPointButton = new Button(window, 0, 0, 1, 1);
            addPointButton.Text = "Add";
            addPointButton.OnClick = (_, _) => AddPoint();
            wm.AddWindow(addPointButton);

            clearPointsButton = new Button(window, 0, 0, 1, 1);
            clearPointsButton.Text = "Clr";
            clearPointsButton.OnClick = (_, _) => ClearPoints();
            wm.AddWindow(clearPointsButton);

            pointTable = new Table(window, 0, 0, 1, 1);
            pointTable.CellHeight = 24;
            pointTable.AllowDeselection = true;
            wm.AddWindow(pointTable);

            scaleInput = new NumericUpDown(window, 0, 0, 1, 1);
            scaleInput.Minimum = 8;
            scaleInput.Maximum = 100;
            scaleInput.Value = 24;
            scaleInput.Step = 2;
            scaleInput.Changed = _ => RenderGraph();
            wm.AddWindow(scaleInput);

            plotButton = new Button(window, 0, 0, 1, 1);
            plotButton.Text = "Plot All";
            plotButton.OnClick = (_, _) => RenderGraph();
            wm.AddWindow(plotButton);

            canvas = new Window(this, window, 0, 0, 1, 1);
            wm.AddWindow(canvas);

            statusBox = new TextBox(window, 0, 0, 1, 1);
            statusBox.ReadOnly = true;
            statusBox.Text = "Ready";
            wm.AddWindow(statusBox);

            functions.Add(new FunctionSeries
            {
                Expression = "sin(x)",
                ColorName = "Blue",
                Color = ResolveColor("Blue")
            });

            RefreshFunctionTable();
            RefreshPointTable();
            Layout();
            RenderGraph();
            wm.Update(window);
        }

        public override void Run()
        {
        }

        private class ExpressionParser
        {
            private readonly string text;
            private int pos;
            private Node root;

            internal string ErrorMessage { get; private set; } = "Unknown";

            internal ExpressionParser(string expr)
            {
                text = expr ?? string.Empty;
                pos = 0;
            }

            internal bool Parse()
            {
                try
                {
                    pos = 0;
                    root = ParseExpression();
                    SkipSpaces();
                    if (pos != text.Length)
                    {
                        ErrorMessage = "Unexpected token near position " + pos;
                        return false;
                    }
                    return root != null;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
            }

            internal bool TryEvaluate(double x, out double y)
            {
                y = 0;
                if (root == null)
                {
                    return false;
                }

                try
                {
                    y = root.Eval(x);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private void SkipSpaces()
            {
                while (pos < text.Length && char.IsWhiteSpace(text[pos]))
                {
                    pos++;
                }
            }

            private bool Match(char c)
            {
                SkipSpaces();
                if (pos < text.Length && text[pos] == c)
                {
                    pos++;
                    return true;
                }
                return false;
            }

            private string ParseIdentifier()
            {
                SkipSpaces();
                int start = pos;
                while (pos < text.Length && (char.IsLetter(text[pos]) || char.IsDigit(text[pos]) || text[pos] == '_'))
                {
                    pos++;
                }
                if (start == pos)
                {
                    return string.Empty;
                }
                return text.Substring(start, pos - start);
            }

            private Node ParseNumber()
            {
                SkipSpaces();
                int start = pos;
                bool dot = false;
                while (pos < text.Length)
                {
                    char c = text[pos];
                    if (char.IsDigit(c))
                    {
                        pos++;
                        continue;
                    }
                    if (c == '.' && !dot)
                    {
                        dot = true;
                        pos++;
                        continue;
                    }
                    break;
                }

                if (start == pos)
                {
                    return null;
                }

                double value = double.Parse(text.Substring(start, pos - start));
                return new NumberNode(value);
            }

            private Node ParsePrimary()
            {
                SkipSpaces();

                if (Match('('))
                {
                    Node inner = ParseExpression();
                    if (!Match(')'))
                    {
                        throw new Exception("Missing ')'");
                    }
                    return inner;
                }

                if (Match('+'))
                {
                    return ParsePrimary();
                }
                if (Match('-'))
                {
                    return new UnaryNode('-', ParsePrimary());
                }

                Node number = ParseNumber();
                if (number != null)
                {
                    return number;
                }

                string ident = ParseIdentifier();
                if (ident.Length > 0)
                {
                    string id = ident.ToLower();
                    if (id == "x")
                    {
                        return new VarNode();
                    }
                    if (id == "pi")
                    {
                        return new NumberNode(Math.PI);
                    }
                    if (id == "e")
                    {
                        return new NumberNode(Math.E);
                    }

                    if (!Match('('))
                    {
                        throw new Exception("Function call expected for '" + ident + "'");
                    }
                    Node arg = ParseExpression();
                    if (!Match(')'))
                    {
                        throw new Exception("Missing ')' after function " + ident);
                    }
                    return new FuncNode(id, arg);
                }

                throw new Exception("Unexpected token at " + pos);
            }

            private Node ParsePow()
            {
                Node left = ParsePrimary();
                SkipSpaces();
                if (Match('^'))
                {
                    Node right = ParsePow();
                    return new BinNode('^', left, right);
                }
                return left;
            }

            private Node ParseMulDiv()
            {
                Node left = ParsePow();
                while (true)
                {
                    SkipSpaces();
                    if (Match('*'))
                    {
                        left = new BinNode('*', left, ParsePow());
                    }
                    else if (Match('/'))
                    {
                        left = new BinNode('/', left, ParsePow());
                    }
                    else
                    {
                        break;
                    }
                }
                return left;
            }

            private Node ParseExpression()
            {
                Node left = ParseMulDiv();
                while (true)
                {
                    SkipSpaces();
                    if (Match('+'))
                    {
                        left = new BinNode('+', left, ParseMulDiv());
                    }
                    else if (Match('-'))
                    {
                        left = new BinNode('-', left, ParseMulDiv());
                    }
                    else
                    {
                        break;
                    }
                }
                return left;
            }

            private abstract class Node
            {
                internal abstract double Eval(double x);
            }

            private class NumberNode : Node
            {
                private readonly double v;
                internal NumberNode(double value) { v = value; }
                internal override double Eval(double x) { return v; }
            }

            private class VarNode : Node
            {
                internal override double Eval(double x) { return x; }
            }

            private class UnaryNode : Node
            {
                private readonly char op;
                private readonly Node arg;
                internal UnaryNode(char oper, Node node) { op = oper; arg = node; }
                internal override double Eval(double x)
                {
                    double v = arg.Eval(x);
                    return op == '-' ? -v : v;
                }
            }

            private class BinNode : Node
            {
                private readonly char op;
                private readonly Node a;
                private readonly Node b;
                internal BinNode(char oper, Node left, Node right) { op = oper; a = left; b = right; }
                internal override double Eval(double x)
                {
                    double lv = a.Eval(x);
                    double rv = b.Eval(x);
                    switch (op)
                    {
                        case '+': return lv + rv;
                        case '-': return lv - rv;
                        case '*': return lv * rv;
                        case '/': return rv == 0 ? double.NaN : lv / rv;
                        case '^': return Math.Pow(lv, rv);
                        default: return double.NaN;
                    }
                }
            }

            private class FuncNode : Node
            {
                private readonly string fn;
                private readonly Node arg;
                internal FuncNode(string name, Node node) { fn = name; arg = node; }
                internal override double Eval(double x)
                {
                    double v = arg.Eval(x);
                    switch (fn)
                    {
                        case "sin": return Math.Sin(v);
                        case "cos": return Math.Cos(v);
                        case "tan": return Math.Tan(v);
                        case "abs": return Math.Abs(v);
                        case "sqrt": return v < 0 ? double.NaN : Math.Sqrt(v);
                        case "log": return v <= 0 ? double.NaN : Math.Log(v);
                        case "exp": return Math.Exp(v);
                        default: return double.NaN;
                    }
                }
            }
        }
    }
}
