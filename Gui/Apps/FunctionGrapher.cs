using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class FunctionGrapher : Process
    {
        internal FunctionGrapher() : base("Function Grapher", ProcessType.Application) { }

        private AppWindow window;
        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private TextBox expressionBox;
        private NumericUpDown scaleInput;
        private TextBox statusBox;
        private Window canvas;

        private string currentExpression = "sin(x)";
        private readonly Color canvasBg = Color.FromArgb(250, 252, 255);
        private readonly Color axisColor = Color.FromArgb(156, 170, 188);
        private readonly Color gridColor = Color.FromArgb(226, 233, 244);
        private readonly Color curveColor = Color.FromArgb(44, 102, 217);

        private void Layout()
        {
            int top = 34;
            int side = 8;
            int inputH = 24;
            int statusH = 24;

            expressionBox.MoveAndResize(side, top, window.Width - 200, inputH);
            scaleInput.MoveAndResize(window.Width - 184, top, 86, inputH);

            int buttonY = top;
            int buttonW = 40;
            int buttonX = window.Width - 92;

            int canvasY = top + inputH + 8;
            int canvasH = window.Height - canvasY - statusH - 10;
            canvas.MoveAndResize(side, canvasY, window.Width - (side * 2), canvasH);
            statusBox.MoveAndResize(side, window.Height - statusH - 6, window.Width - (side * 2), statusH);
        }

        private void DrawGridAndAxes(int centerX, int centerY, int pixelPerUnit)
        {
            if (pixelPerUnit < 4)
            {
                pixelPerUnit = 4;
            }

            for (int x = centerX; x < canvas.Width; x += pixelPerUnit)
            {
                canvas.DrawLine(x, 0, x, canvas.Height - 1, gridColor);
            }
            for (int x = centerX; x >= 0; x -= pixelPerUnit)
            {
                canvas.DrawLine(x, 0, x, canvas.Height - 1, gridColor);
            }
            for (int y = centerY; y < canvas.Height; y += pixelPerUnit)
            {
                canvas.DrawLine(0, y, canvas.Width - 1, y, gridColor);
            }
            for (int y = centerY; y >= 0; y -= pixelPerUnit)
            {
                canvas.DrawLine(0, y, canvas.Width - 1, y, gridColor);
            }

            canvas.DrawLine(0, centerY, canvas.Width - 1, centerY, axisColor);
            canvas.DrawLine(centerX, 0, centerX, canvas.Height - 1, axisColor);
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

            ExpressionParser parser = new ExpressionParser(currentExpression);
            if (!parser.Parse())
            {
                statusBox.Text = "Parse error: " + parser.ErrorMessage;
                statusBox.Render();
                wm.Update(statusBox);
                wm.Update(canvas);
                return;
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

                if (hasLast)
                {
                    if (Math.Abs(sy - lastY) < canvas.Height)
                    {
                        canvas.DrawLine(lastX, lastY, sx, sy, curveColor);
                    }
                }
                hasLast = true;
                lastX = sx;
                lastY = sy;
            }

            statusBox.Text = "OK: y = " + currentExpression;
            statusBox.Render();
            wm.Update(statusBox);
            wm.Update(canvas);
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 140, 80, 900, 560);
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

            expressionBox = new TextBox(window, 8, 34, 680, 24);
            expressionBox.Text = currentExpression;
            expressionBox.PlaceholderText = "Enter expression, e.g. sin(x), x^2+2*x+1";
            expressionBox.Submitted = () =>
            {
                currentExpression = expressionBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(currentExpression))
                {
                    currentExpression = "x";
                    expressionBox.Text = currentExpression;
                }
                RenderGraph();
            };
            wm.AddWindow(expressionBox);

            scaleInput = new NumericUpDown(window, 696, 34, 86, 24);
            scaleInput.Minimum = 8;
            scaleInput.Maximum = 80;
            scaleInput.Value = 24;
            scaleInput.Step = 2;
            scaleInput.Changed = (_) => RenderGraph();
            wm.AddWindow(scaleInput);

            Button plotButton = new Button(window, 790, 34, 46, 24);
            plotButton.Text = "Plot";
            plotButton.OnClick = (_, _) =>
            {
                currentExpression = expressionBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(currentExpression))
                {
                    currentExpression = "x";
                    expressionBox.Text = currentExpression;
                }
                RenderGraph();
            };
            wm.AddWindow(plotButton);

            Button clearButton = new Button(window, 842, 34, 50, 24);
            clearButton.Text = "Clear";
            clearButton.OnClick = (_, _) =>
            {
                expressionBox.Text = "x";
                currentExpression = "x";
                RenderGraph();
            };
            wm.AddWindow(clearButton);

            canvas = new Window(this, window, 8, 66, window.Width - 16, window.Height - 98);
            wm.AddWindow(canvas);

            statusBox = new TextBox(window, 8, window.Height - 30, window.Width - 16, 24);
            statusBox.ReadOnly = true;
            statusBox.Text = "Ready";
            wm.AddWindow(statusBox);

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
