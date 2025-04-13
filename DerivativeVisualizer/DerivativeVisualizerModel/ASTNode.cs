using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
namespace DerivativeVisualizerModel
{
    // TODO: Mivel ide is került azóta néhány függvény, amik elég fontosak, így ezt a classt is tesztelni kell.
    public class ASTNode
    {
        private static int counter = 0;
        public string Value { get; }
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }

        /// <summary>
        /// Used to locate nodes for differentiation.
        /// </summary>
        public int Locator { get; private set; }

        public bool ToBeDifferentiated { get; set; }

        /// <summary>
        /// Represents the differentiation rule of a node in human-readable format.
        /// </summary>
        public string DiffRule
        {
            get
            {
                if (!ToBeDifferentiated)
                {
                    return "";
                }
                if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || Value == "e")
                {
                    return "a' = 0 (a valós szám)";
                }
                switch (Value)
                {
                    case "x": return "x' = 1";
                    case "+":
                    case "-":
                        return $"(f {Value} g)' = f' {Value} g'";
                    case "*":
                        return "(f * g)' = f' * g + f * g'";
                    case "/":
                        return "(f / g)' = (f' * g - f * g') / (g ^ 2)";
                    case "^":
                        if (Left.Value == "e" && Right.Value == "x")
                        {
                            return "(e ^ x)' = e ^ x";
                        }
                        else if (Left.Value == "e")
                        {
                            return "(e ^ f)' = e ^ f * f'";
                        }
                        else if (double.TryParse(Left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double a) && Right.Value == "x")
                        {
                            if (a > 0)
                            {
                                return "(a ^ x)' = a ^ x * ln(a) (a > 0)";
                            }
                            else
                            {
                                return $"Ha a <= 0 (a = {a}), akkor a^x nem deriválható";
                            }
                        }
                        else if (double.TryParse(Left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double c))
                        {
                            if (c > 0)
                            {
                                return "(a ^ f)' = a ^ f * ln(a) * f' (a > 0)";
                            }
                            else
                            {
                                return $"Ha a <= 0 (a = {a}), akkor a^x nem deriválható";
                            }
                        }
                        else if ((double.TryParse(Right.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || Right.Value == "e") && Left.Value == "x")
                        {
                            return "(x ^ n)' =  n * x ^ (n - 1)";
                        }
                        else if (double.TryParse(Right.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || Right.Value == "e")
                        {
                            return "(f ^ n)' = n * f ^ (n - 1) * f'";
                        }
                        else
                        {
                            return "(f ^ g)' = (f ^ g)*(f' * g / f + g' * ln(f)) (f > 0, nem ellenőrzött)";
                        }
                    case "log":
                        if (double.TryParse(Left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double b) || Left.Value == "e")
                        {
                            if (Left.Value == "e")
                            {
                                if (Right.Value == "x")
                                {
                                    return "log'(e,x) = 1 / x";
                                }
                                return "log'(e,f) = f' / f";
                            }
                            if (b <= 0 || b == 1)
                            {
                                return "A logaritmus alapja egy pozitív szám, kivéve 1.";
                            }
                            if (Right.Value == "x")
                            {
                                return "log'(a,x) = 1 / (x * ln(a))";
                            }
                            return "log'(a,f) = f' / (f * ln(a))";
                        }
                        return "A logaritmus alapja egy pozitív szám, kivéve 1.";
                    case "ln":
                        if (Left.Value == "x")
                        {
                            return "ln'(x) = 1 / x";
                        }
                        return "ln'(f) = f' / f";
                    case "sin":
                        if (Left.Value == "x")
                        {
                            return "sin'(x) = cos(x)";
                        }
                        return "sin'(f) = cos(f) * f'";
                    case "cos":
                        if (Left.Value == "x")
                        {
                            return "cos'(x) = -1 * sin(x)";
                        }
                        return "cos'(f) = -1 * sin(f) * f'";
                    case "tg":
                        if (Left.Value == "x")
                        {
                            return "tg'(x) = 1 / cos^2(x)";
                        }
                        return "tg'(f) = f' / cos^2(f)";
                    case "ctg":
                        if (Left.Value == "x")
                        {
                            return "ctg'(x) = -1 / sin^2(x)";
                        }
                        return "ctg'(f) = (-1 * f') / sin^2(f)";
                    case "arcsin":
                        if (Left.Value == "x")
                        {
                            return "arcsin'(x) = 1 / (1 - x ^ 2) ^ (1 / 2)";
                        }
                        return "arcsin'(f) = f' / (1 - f ^ 2) ^ (1 / 2)";
                    case "arccos":
                        if (Left.Value == "x")
                        {
                            return "arccos'(x) = -1 / (1 - x ^ 2) ^ (1 / 2)";
                        }
                        return "arccos'(f) = (-1 * f') / (1 - f ^ 2) ^ (1 / 2)";
                    case "arctg":
                        if (Left.Value == "x")
                        {
                            return "arctg'(x) = 1 / (1 + x ^ 2)";
                        }
                        return "arctg'(f) = f' / (1 + f ^ 2)";
                    case "arcctg":
                        if (Left.Value == "x")
                        {
                            return "arcctg'(x) = -1 / (1 + x ^ 2)";
                        }
                        return "arcctg'(f) = (-1 * f') / (1 + f ^ 2)";
                    case "sh":
                        if (Left.Value == "x")
                        {
                            return "sh'(x) = ch(x)";
                        }
                        return "sh'(f) = ch(f) * f'";
                    case "ch":
                        if (Left.Value == "x")
                        {
                            return "ch'(x) = sh(x)";
                        }
                        return "ch'(f) = sh(f) * f'";
                    case "th":
                        if (Left.Value == "x")
                        {
                            return "th'(x) = 1 / ch^2(x)";
                        }
                        return "th'(f) = f' / ch^2(f)";
                    case "cth":
                        if (Left.Value == "x")
                        {
                            return "cth'(x) = -1 / sh^2(x)";
                        }
                        return "cth'(f) = (-1 * f') / sh^2(f)";
                    case "arsh":
                        if (Left.Value == "x")
                        {
                            return "arsh'(x) = 1 / (x ^ 2 + 1) ^ (1 / 2)";
                        }
                        return "arsh'(f) = f' / (f ^ 2 + 1) ^ (1 / 2)";
                    case "arch":
                        if (Left.Value == "x")
                        {
                            return "arch'(x) = 1 / (x ^ 2 - 1) ^ (1 / 2)";
                        }
                        return "arch'(f) = f' / (f ^ 2 - 1) ^ (1 / 2)";
                    case "arth":
                    case "arcth":
                        if (Left.Value == "x")
                        {
                            return $"{Value}'(x) = 1 / (1 - x ^ 2)";
                        }
                        return $"{Value}'(f) = f' / (1 - f ^ 2)";
                    default: return $"Nem deriválható: {Value}";
                }
            }
        }

        public ASTNode(string value, ASTNode left = null!, ASTNode right = null!)
        {
            Value = value;
            Left = left;
            Right = right;
            Locator = counter;
            counter++;
        }

        /// <summary>
        /// Deep copies a tree keeping the NeedsDifferentiation and Locator values.
        /// </summary>
        /// <returns></returns>
        public ASTNode DeepCopy()
        {
            ASTNode? leftCopy = Left?.DeepCopy();
            ASTNode? rightCopy = Right?.DeepCopy();

            return new ASTNode(Value, leftCopy!, rightCopy!)
            {
                ToBeDifferentiated = ToBeDifferentiated,
                Locator = Locator
            };
        }

        /// <summary>
        /// Determines if a tree contains any node with true NeedsDifferentiation value.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool HasDifferentiationNode(ASTNode node)
        {
            if (node == null) return false;
            if (node.ToBeDifferentiated) return true;

            return HasDifferentiationNode(node.Left) || HasDifferentiationNode(node.Right);
        }

        /// <summary>
        /// Adds "( )'" around a subtree's ToString() if it needs differentiation.
        /// AI Generated
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            if (ToBeDifferentiated)
            {
                return $"({SubtreeToString()})'";
            }

            return SubtreeToString();
        }

        /// <summary>
        /// Converts the current subtree of the expression into a human-readable string representation, adding parentheses and formatting based on the type of operation.
        /// AI Generated
        /// </summary>
        /// <returns></returns>
        private string SubtreeToString()
        {
            if (Left == null && Right == null)
            {
                return Value;
            }

            if (Value == "log" && Left != null && Right != null)
            {
                return $"log({Left}, {Right})";
            }

            if (Right == null)
            {
                return $"{Value}({Left})";
            }

            string? leftStr = AddParenthesesIfNeeded(Left, Value);
            string? rightStr = AddParenthesesIfNeeded(Right, Value);

            return $"{leftStr} {Value} {rightStr}";
        }

        /// <summary>
        /// Returns the string representation of a node, wrapping it in parentheses if needed based on operator precedence.
        /// AI Generated
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentOp"></param>
        /// <returns></returns>
        private string? AddParenthesesIfNeeded(ASTNode? node, string parentOp)
        {
            if (node == null) return null;

            string nodeString = node.ToString();

            if (node.Left == null && node.Right == null)
            {
                return nodeString;
            }

            bool needParens = NeedsParentheses(node, parentOp);
            return needParens ? $"({nodeString})" : nodeString;
        }

        /// <summary>
        /// Determines whether parentheses are required around a child node based on the precedence of the child's and parent’s operators.
        /// AI Generated
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentOp"></param>
        /// <returns></returns>
        private bool NeedsParentheses(ASTNode node, string parentOp)
        {
            string[] operators = { "+", "-", "*", "/", "^" };
            int parentPrecedence = Array.IndexOf(operators, parentOp);
            int childPrecedence = Array.IndexOf(operators, node.Value);

            if (parentOp == "^" && node.Value == "^")
            {
                return true;
            }

            return childPrecedence != -1 && childPrecedence < parentPrecedence;
        }

        /// <summary>
        /// Determines if two trees are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreTreesEqual(ASTNode a, ASTNode b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Value != b.Value) return false;
            return AreTreesEqual(a.Left, b.Left) && AreTreesEqual(a.Right, b.Right);
        }

        /// <summary>
        /// Simplifies a tree until it can't be simplified anymore.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ASTNode Simplify(ASTNode? node)
        {
            if (node is null) return null!;

            ASTNode previous, current = node;

            do
            {
                previous = current;
                current = SimplifyOnce(previous);
            }
            while (!AreTreesEqual(previous, current));

            return current;
        }

        /// <summary>
        /// Simplifies a tree top-down once based on some basic mathematical rules.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ASTNode SimplifyOnce(ASTNode? node)
        {
            if (node is null) return null!;

            // Addition
            if (node.Value == "+" && node.Left.Value == "0" && node.Right.Value == "0") return new ASTNode("0"); // 0 + 0 = 0
            if (node.Value == "+" && node.Left.Value == "0") return SimplifyOnce(node.Right); // 0 + x = x
            if (node.Value == "+" && node.Right.Value == "0") return SimplifyOnce(node.Left); // x + 0 = x

            // Subtraction
            if (node.Value == "-" && AreTreesEqual(node.Left, node.Right)) return new ASTNode("0"); // x - x = 0
            if (node.Value == "-" && node.Right.Value == "0") return SimplifyOnce(node.Left); // x - 0 = x

            // Multiplication
            if (node.Value == "*" && node.Left.Value == "0") return new ASTNode("0"); // 0 * x = 0
            if (node.Value == "*" && node.Right.Value == "0") return new ASTNode("0"); // x * 0 = 0
            if (node.Value == "*" && node.Left.Value == "1") return SimplifyOnce(node.Right); // 1 * x = x
            if (node.Value == "*" && node.Right.Value == "1") return SimplifyOnce(node.Left); // x * 1 = x

            // Division
            if (node.Value == "/" && AreTreesEqual(node.Left, node.Right) && node.Right.Value != "0") return new ASTNode("1"); // x / x = 1 (x not 0)
            if (node.Value == "/" && node.Left.Value == "0" && node.Right.Value != "0") return new ASTNode("0"); // 0 / x = 0
            if (node.Value == "/" && node.Right.Value == "1") return SimplifyOnce(node.Left); // x / 1 = x

            //Exponentiation
            if (node.Value == "^" && node.Left.Value == "1") return new ASTNode("1"); // 1 ^ x = 1
            if (node.Value == "^" && node.Right.Value == "1") return SimplifyOnce(node.Left); // x ^ 1 = x
            if (node.Value == "^" && node.Right.Value == "0" && node.Left.Value != "0") return new ASTNode("1"); // x ^ 0 = 1 (x not 0)
            if (node.Value == "^" && node.Left.Value == "0" && node.Right.Value != "0") return new ASTNode("0"); // 0 ^ x = 0 (x not 0)   

            return new ASTNode(node.Value, SimplifyOnce(node.Left), SimplifyOnce(node.Right)) { ToBeDifferentiated = node.ToBeDifferentiated};
        }

        /// <summary>
        /// Determines if a node represents an operator.
        /// </summary>
        /// <returns></returns>
        public bool IsOperator()
        {
            return Value is "+" or "-" or "*" or "/" or "^";
        }

        /// <summary>
        /// Determines if a node represents a function.
        /// </summary>
        /// <returns></returns>
        public bool IsFunction()
        {
            return Value is "sin" or "cos" or "tg" or "ctg" or "arcsin" or "arccos" or "arctg" or "arcctg" or
                   "sh" or "ch" or "th" or "cth" or "arsh" or "arch" or "arth" or "arcth" or "ln" or "log";
        }
    }
}