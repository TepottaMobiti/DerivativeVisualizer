using System;
using System.Collections.Generic;
namespace DerivativeVisualizerModel
{
    public class ASTNode
    {
        public string Value { get; }
        public ASTNode Left { get; }
        public ASTNode Right { get; }

        public ASTNode(string value, ASTNode left = null!, ASTNode right = null!)
        {
            Value = value;
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            if (IsNumber(Value) || IsVariable(Value))
            {
                return Value;
            }
            string leftStr = Left?.ToString() ?? "";
            string rightStr = Right?.ToString() ?? "";
            switch (Value)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                    return Combine(leftStr, rightStr, Value);
                case "sin":
                    return $"sin({leftStr})";
                case "cos":
                    return $"cos({leftStr})";
                case "tan":
                    return $"tan({leftStr})";
                default:
                    return Value;
            }
        }


        private string Combine(string left, string right, string op)
        {
            if (op == "*" && right.StartsWith("-"))
            {
                return $"{left} * {right}";
            }
            bool needsParenthesesLeft = NeedsParentheses(Left, op);
            bool needsParenthesesRight = NeedsParentheses(Right, op);
            if (needsParenthesesLeft)
            {
                left = $"({left})";
            }
            if (needsParenthesesRight)
            {
                right = $"({right})";
            }
            if (op == "*")
            {
                if (left == "1") return right;
                if (right == "1") return left;
                return $"{left} * {right}";
            }
            if (op == "+" || op == "-")
            {
                if (right.StartsWith("-"))
                {
                    return $"{left} {op} {right[1..]}";
                }
                return $"{left} {op} {right}";
            }
            return $"{left} {op} {right}";
        }

        private bool NeedsParentheses(ASTNode node, string parentOp)
        {
            if (node == null) return false;
            int precedence = GetPrecedence(parentOp);
            int nodePrecedence = GetPrecedence(node.Value);

            return nodePrecedence < precedence;
        }

        private int GetPrecedence(string op)
        {
            return op switch
            {
                "+" => 1,
                "-" => 1,
                "*" => 2,
                "/" => 2,
                "^" => 3,
                _ => 0
            };
        }

        private bool IsNumber(string value)
        {
            return double.TryParse(value, out _);
        }

        private bool IsVariable(string value)
        {
            return value == "x" || value == "y" || value == "z";
        }

        public void Print(string indent = "", bool last = true)
        {
            Console.WriteLine(indent + (last ? "└── " : "├── ") + Value);
            indent += last ? "    " : "│   ";

            if (Left != null) Left.Print(indent, Right == null);
            if (Right != null) Right.Print(indent, true);
        }
    }
}
