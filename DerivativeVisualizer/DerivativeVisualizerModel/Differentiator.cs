using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    public static class Differentiator
    {
        //TODO: FunctionEvaluator class Evaluate(ASTNode, double) kiértékelő metódussal
        public static ASTNode Differentiate(ASTNode? node)
        {
            if (node is null)
            {
                return null!;
            }
            string value = node.Value;
            ASTNode? left = node.Left;
            ASTNode? right = node.Right;
            if(double.TryParse(value, out _) || value == "e") //a' = 0 (a is real)
            {
                return new ASTNode("0");
            }
            switch(value)
            {
                case "x": return new ASTNode("1"); // x' = 1
                case "+": // (f+g)' = f' + g'
                case "-": // (f-g)' = f' - g'
                    return new ASTNode(value, Differentiate(left), Differentiate(right));
                case "*": // (f*g)' = f'*g+f*g'
                    return new ASTNode("+",
                               new ASTNode("*", Differentiate(left), right),
                               new ASTNode("*", left, Differentiate(right)));
                case "/": // (f/g)' / (f'g-f*g')/g^2
                    return new ASTNode("/",
                               new ASTNode("-",
                                   new ASTNode("*",Differentiate(left),right),
                                   new ASTNode("*",left,Differentiate(right))),
                               new ASTNode("^",right,new ASTNode("2")));
                case "^":
                    if(left.Value == "e" && right.Value == "x") // (e^x)' = e^x
                    {
                        return node;
                    }
                    else if (left.Value == "e") // (e^f)' = e^f*f'
                    {
                        return new ASTNode("*",
                                   new ASTNode("^",
                                       new ASTNode("e"),
                                       right),
                                   Differentiate(right));
                    }
                    else if (double.TryParse(left.Value, out double a) && right.Value=="x") // (a^x)' = a^x*ln(a) (a>0)
                    {
                        if (a>0)
                        {
                            return new ASTNode("*",
                                       node,
                                       new ASTNode("ln",new ASTNode(left.Value)));
                        }
                        else
                        {
                            throw new Exception($"Can't differentiate a^x if a <= 0 (a = {a})");
                        }
                    }
                    else if(double.TryParse(right.Value,out _) || right.Value=="e") // (f^n)' = n*f^(n-1)*f'
                    {
                        if (right.Value=="e")
                        {
                            return new ASTNode("*",
                                   new ASTNode("*",
                                       right,
                                       new ASTNode("^",
                                           left,
                                           new ASTNode((Math.E - 1).ToString()))),
                                    Differentiate(left));
                        }
                        return new ASTNode("*",
                                   new ASTNode("*",
                                       right,
                                       new ASTNode("^",
                                           left,
                                           new ASTNode((double.Parse(right.Value)-1).ToString()))),
                                    Differentiate(left));
                    }
                    else // (f^g)' = (f^g)*(f'*g/f+g'*ln(f)) (f>0, not checked)
                    {
                        return new ASTNode("*",
                                   new ASTNode("^", left, right),
                                   new ASTNode("+",
                                       new ASTNode("*",
                                           Differentiate(left),
                                           new ASTNode("/", right, left)),
                                       new ASTNode("*",
                                           Differentiate(right),
                                           new ASTNode("ln",left))));
                    }
                case "log":
                    if (double.TryParse(left.Value, out double b) || left.Value=="e")
                    {
                        if (left.Value == "e") // log_e'(f) = f'/f
                        {
                            return new ASTNode("/",
                                       Differentiate(right),
                                       right);
                        }
                        if (b <= 0 || b == 1)
                        {
                            throw new Exception("The base of the logarithm should be a positive number other than 1.");
                        }
                        // log_a'(f) = f'/(f*ln(a))
                        return new ASTNode("/",
                                   Differentiate(right),
                                   new ASTNode("*",
                                       right,
                                       new ASTNode("ln",
                                           new ASTNode(left.Value))));
                    }
                    throw new Exception("The base of the logarithm should be a positive number other than 1.");
                case "ln": // ln'(f) = f'/f
                    return new ASTNode("/",
                                       Differentiate(left),
                                       left);
                case "sin": // sin'(f) = cos(f)*f'
                    return new ASTNode("*",
                               new ASTNode("cos",
                                   left),
                               Differentiate(left));
                case "cos": // cos'(f)= -1*sin(f)*f'
                    return new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   new ASTNode("sin",left)),
                               Differentiate(left));
                case "tg": // tg'(f) = f'/cos^2(f)
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("^",
                                   new ASTNode("cos",
                                       left),
                                   new ASTNode("2")));
                case "ctg": // ctg'(f) = (-1*f')/sin^2(f)
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   Differentiate(left)),
                               new ASTNode("^",
                                   new ASTNode("sin",
                                       left),
                                   new ASTNode("2")));
                case "arcsin": // arcsin'(f) = f'/(1-f^2)^(1/2)
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left,
                                           new ASTNode("2"))),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));
                case "arccos": // arccos'(f) = (-1*f')/(1-f^2)^(1/2)
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   Differentiate(left)),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left,
                                           new ASTNode("2"))),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));
                case "arctg": // arctg'(f) = f'/(1+f^2)
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                case "arcctg": // arcctg'(f) = (-1*f')/(1+f^2)
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   Differentiate(left)),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                case "sh": // sh'(f) = ch(f)*f'
                    return new ASTNode("*",
                               new ASTNode("ch",
                                   left),
                               Differentiate(left));
                case "ch": // ch'(f) = sh(f)*f'
                    return new ASTNode("*",
                               new ASTNode("sh",
                                   left),
                               Differentiate(left));
                case "th": // th'(f) = f'/ch^2(f)
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("^",
                                   new ASTNode("ch",
                                       left),
                                   new ASTNode("2")));
                case "cth": // cth'(f) = (-1*f')/sh^2(f)
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   Differentiate(left)),
                               new ASTNode("^",
                                   new ASTNode("sh",
                                       left),
                                   new ASTNode("2")));
                case "arsh": // arsh'(f) = f'/(f^2+1)^1/2
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("^",
                                   new ASTNode("+",
                                       new ASTNode("^",
                                           left,
                                           new ASTNode("2")),
                                       new ASTNode("1")),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));
                case "arch": // arch'(f) = f'/(f^2-1)^1/2
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("^",
                                           left,
                                           new ASTNode("2")),
                                       new ASTNode("1")),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));
                case "arth": // arth'(f) = f'/(1-f^2)
                case "arcth": // arcth'(f) = f'/(1-f^2)
                    return new ASTNode("/",
                               Differentiate(left),
                               new ASTNode("-",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                default: throw new Exception($"Can't differentiate node with value {value}");
            }
        }

        
    }
}
