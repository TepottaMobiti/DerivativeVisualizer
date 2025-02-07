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

        // AI, Dokumentáció: mivel, prompt.
        public void Print(string indent = "", bool last = true)
        {
            Console.WriteLine(indent + (last ? "└── " : "├── ") + Value);
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
    }
}