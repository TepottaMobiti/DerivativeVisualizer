using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    public static class FunctionEvaluator
    {
        public static double Evaluate(ASTNode node, double xValue)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (double.TryParse(node.Value, out double number))
            {
                return number;
            }
            else if (node.Value == "x")
            {
                return xValue;
            }
            else if (node.IsOperator())
            {
                double left = Evaluate(node.Left, xValue);
                double right = Evaluate(node.Right, xValue);
                return node.Value switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => right == 0 ? throw new DivideByZeroException("Division by zero.") : left / right,
                    "^" => Math.Pow(left, right),
                    _ => throw new Exception($"Unknown operator: {node.Value}")
                };
            }
            else if (node.IsFunction())
            {
                double argument = Evaluate(node.Left, xValue);
                return node.Value switch
                {
                    "sin" => Math.Sin(argument),
                    "cos" => Math.Cos(argument),
                    "tg" => Math.Tan(argument),
                    "ctg" => 1 / Math.Tan(argument),
                    "arcsin" => Math.Asin(argument),
                    "arccos" => Math.Acos(argument),
                    "arctg" => Math.Atan(argument),
                    "arcctg" => Math.PI / 2 - Math.Atan(argument),
                    "sh" => Math.Sinh(argument),
                    "ch" => Math.Cosh(argument),
                    "th" => Math.Tanh(argument),
                    "cth" => 1 / Math.Tanh(argument),
                    "arsh" => Math.Asinh(argument),
                    "arch" => Math.Acosh(argument),
                    "arth" => Math.Atanh(argument),
                    "arcth" => 0.5 * Math.Log((argument + 1) / (argument - 1)),
                    "ln" => Math.Log(argument),
                    "log" => Math.Log(Evaluate(node.Right, xValue), argument),
                    _ => throw new Exception($"Unknown function: {node.Value}")
                };
            }
            throw new Exception($"Invalid node value: {node.Value}");
        }
    }
}
