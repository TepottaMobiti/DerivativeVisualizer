using System;
using System.Collections.Generic;
using System.Xml;
namespace DerivativeVisualizerModel
{
    //TODO: Mivel ide is került azóta néhány függvény, amik elég fontosak, így ezt a classt is tesztelni kell.
    public class ASTNode
    {
        public string Value { get; }
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }

        public bool NeedsDifferentiation { get; set; }

        public ASTNode(string value, ASTNode left = null!, ASTNode right = null!)
        {
            Value = value;
            Left = left;
            Right = right;
        }

        public ASTNode DeepCopy()
        {
            return new ASTNode(Value, Left?.DeepCopy()!, Right?.DeepCopy()!)
            {
                NeedsDifferentiation = this.NeedsDifferentiation
            };
        }

        // AI, Dokumentáció: mivel, prompt.
        public void Print(string indent = "", bool last = true)
        {
            string displayValue = Value + (NeedsDifferentiation ? "'" : "");

            Console.WriteLine(indent + (last ? "└── " : "├── ") + displayValue);
            indent += last ? "    " : "│   ";

            if (Left != null) Left.Print(indent, Right == null);
            if (Right != null) Right.Print(indent, true);
        }

        public override string ToString()
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

        private string? AddParenthesesIfNeeded(ASTNode? node, string parentOp)
        {
            if (node?.Left == null && node?.Right == null)
            {
                return node?.ToString();
            }

            bool needParens = NeedsParentheses(node, parentOp);
            return needParens ? $"({node})" : node.ToString();
        }

        private bool NeedsParentheses(ASTNode node, string parentOp)
        {
            string[] operators = { "+", "-", "*", "/", "^" };
            int parentPrecedence = Array.IndexOf(operators, parentOp);
            int childPrecedence = Array.IndexOf(operators, node.Value);

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
            if (node.Value == "/" && AreTreesEqual(node.Left, node.Right)) return new ASTNode("1"); // x / x = 1
            if (node.Value == "/" && node.Left.Value == "0") return new ASTNode("0"); // 0 / x = 0
            if (node.Value == "/" && node.Right.Value == "1") return SimplifyOnce(node.Left); // x / 1 = x

            //Exponentiation
            if (node.Value == "^" && node.Left.Value == "1") return new ASTNode("1"); // 1 ^ x = 1
            if (node.Value == "^" && node.Right.Value == "1") return SimplifyOnce(node.Left); // x ^ 1 = x
            if (node.Value == "^" && node.Right.Value == "0" && node.Left.Value != "0") return new ASTNode("1"); // x ^ 0 = 1 (x not 0)
            if (node.Value == "^" && node.Left.Value == "0" && node.Right.Value != "0") return new ASTNode("0"); // 0 ^ x = 0 (x not 0)   

            return new ASTNode(node.Value, SimplifyOnce(node.Left), SimplifyOnce(node.Right)) { NeedsDifferentiation = node.NeedsDifferentiation};
        }
    }
}