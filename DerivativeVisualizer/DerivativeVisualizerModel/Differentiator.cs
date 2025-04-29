using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DerivativeVisualizerModel
{
    public class Differentiator
    {
        private List<ASTNode> differentiationSteps = new List<ASTNode>();

        public Differentiator(ASTNode root)
        {
            FlagNode(root);
            differentiationSteps.Add(root);
        }

        public ASTNode CurrentTree
        {
            get => differentiationSteps.Last();
        }

        /// <summary>
        /// Finds the node marked for differentiation by its locator, differentiates it once, replaces the original in the expression tree, and adds the new tree to the list of differentiation steps.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public ASTNode Differentiate(int locator)
        {
            ASTNode lastTree = CurrentTree.DeepCopy();

            ASTNode nodeToDifferentiate = FindDifferentiationNode(lastTree, locator);

            ASTNode differentiatedNode = DifferentiateOnce(nodeToDifferentiate);

            ASTNode newTree = ReplaceNode(lastTree, nodeToDifferentiate, differentiatedNode);

            differentiationSteps.Add(newTree);
            return newTree;
        }

        /// <summary>
        /// Recursively searches the expression tree to find the node that needs differentiation based on a unique locator.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        private ASTNode FindDifferentiationNode(ASTNode node, int locator)
        {
            if (node == null) return null!;
            if (node.ToBeDifferentiated && node.Locator == locator) return node;

            ASTNode found = FindDifferentiationNode(node.Left, locator);
            return found ?? FindDifferentiationNode(node.Right, locator);
        }

        /// <summary>
        /// Applies a single symbolic differentiation rule to the given node based on standard calculus rules for various mathematical operations and functions.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private ASTNode DifferentiateOnce(ASTNode? node)
        {
            if (node is null)
            {
                return null!;
            }
            string value = node.Value;
            ASTNode? left = node.Left;
            ASTNode? right = node.Right;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double _) || value == "e") // c' = 0 (c valós szám)
            {
                return new ASTNode("0");
            }
            switch (value)
            {
                case "x": return new ASTNode("1"); // x' = 1

                case "+": // (f+g)' = f' + g'
                case "-": // (f-g)' = f' - g'
                    return new ASTNode(value, FlagNode(left.DeepCopy()), FlagNode(right.DeepCopy()));

                case "*": 
                    // (c*f)' = c*f'
                    if (double.TryParse(left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double _))
                    {
                        return new ASTNode("*",
                                   left.DeepCopy(),
                                   FlagNode(right.DeepCopy()));
                    }
                    // (f*c)' = f'*c
                    else if (double.TryParse(right.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double _))
                    {
                        return new ASTNode("*",
                                   FlagNode(left.DeepCopy()),
                                   right.DeepCopy());
                    }
                    // (f*g)' = f'*g+f*g'
                    return new ASTNode("+",
                               new ASTNode("*", FlagNode(left.DeepCopy()), right.DeepCopy()),
                               new ASTNode("*", left.DeepCopy(), FlagNode(right.DeepCopy())));

                case "/": // (f/g)' = (f'g-f*g')/g^2
                    return new ASTNode("/",
                               new ASTNode("-",
                                   new ASTNode("*", FlagNode(left.DeepCopy()), right.DeepCopy()),
                                   new ASTNode("*", left, FlagNode(right.DeepCopy()))),
                               new ASTNode("^", right, new ASTNode("2")));

                case "^":
                    if (left.Value == "e" && right.Value == "x") // (e^x)' = e^x
                    {
                        return UnflagNode(node.DeepCopy());
                    }
                    else if (left.Value == "e") // (e^f)' = e^f*f'
                    {
                        return new ASTNode("*",
                                   new ASTNode("^",
                                       new ASTNode("e"),
                                       right.DeepCopy()),
                                   FlagNode(right.DeepCopy()));
                    }
                    else if (double.TryParse(left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double a) && right.Value == "x") // (a^x)' = a^x*ln(a) (a>0)
                    {
                        if (a > 0)
                        {
                            return new ASTNode("*",
                                       UnflagNode(node.DeepCopy()),
                                       new ASTNode("ln", new ASTNode(left.Value)));
                        }
                        else
                        {
                            throw new Exception($"Ha a <= 0 (a = {a}), akkor a^x nem deriválható");
                        }
                    }
                    else if (double.TryParse(left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double c)) // (a^f)' = a^f*ln(a)*f' (a>0)
                    {
                        if (c > 0)
                        {
                            return new ASTNode("*",
                                       new ASTNode("*",
                                           UnflagNode(node.DeepCopy()),
                                           new ASTNode("ln", new ASTNode(left.Value))),
                                       FlagNode(right));
                        }
                        else
                        {
                            throw new Exception($"Ha a <= 0 (a = {c}), akkor a^f nem deriválható");
                        }
                    }
                    else if ((double.TryParse(right.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double num1) || right.Value == "e") && left.Value=="x") // (x^n)' =  n*x^(n-1)
                    {
                        if (right.Value=="e")
                        {
                            return new ASTNode("*",
                                       right.DeepCopy(),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode((Math.E - 1).ToString(CultureInfo.InvariantCulture))));
                        }
                        return new ASTNode("*",
                                   right.DeepCopy(),
                                   new ASTNode("^",
                                       left.DeepCopy(),
                                       new ASTNode((num1 - 1).ToString(CultureInfo.InvariantCulture))));
                    }
                    else if (double.TryParse(right.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double num2) || right.Value == "e") // (f^n)' = n*f^(n-1)*f'
                    {
                        if (right.Value == "e")
                        {
                            return new ASTNode("*",
                                       new ASTNode("*",
                                           right.DeepCopy(),
                                           new ASTNode("^",
                                               left.DeepCopy(),
                                               new ASTNode((Math.E - 1).ToString(CultureInfo.InvariantCulture)))),
                                        FlagNode(left.DeepCopy()));
                        }
                        return new ASTNode("*",
                                   new ASTNode("*",
                                       right.DeepCopy(),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode((num2 - 1).ToString(CultureInfo.InvariantCulture)))),
                                    FlagNode(left.DeepCopy()));
                    }
                    else // (f^g)' = (f^g)*(f'*g/f+g'*ln(f)) (f>0, not checked)
                    {
                        return new ASTNode("*",
                                   UnflagNode(node.DeepCopy()),
                                   new ASTNode("+",
                                       new ASTNode("*",
                                           FlagNode(left.DeepCopy()),
                                           new ASTNode("/", right.DeepCopy(), left.DeepCopy())),
                                       new ASTNode("*",
                                           FlagNode(right.DeepCopy()),
                                           new ASTNode("ln", left.DeepCopy()))));
                    }

                case "log":
                    if (double.TryParse(left.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double b) || left.Value == "e")
                    {
                        if (left.Value == "e") // log_e'(f) = f'/f
                        {
                            if (right.Value == "x")
                            {
                                return new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("x"));
                            }
                            return new ASTNode("/",
                                       FlagNode(right.DeepCopy()),
                                       right.DeepCopy());
                        }
                        if (b <= 0 || b == 1)
                        {
                            throw new Exception("A logaritmus alapja egy pozitív szám, kivéve 1.");
                        }
                        if (right.Value == "x") //log_a'(x) = 1/(x*ln(a))
                        {
                            return new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("*",
                                           right.DeepCopy(),
                                           new ASTNode("ln",
                                               new ASTNode(left.Value))));
                        }
                        // log_a'(f) = f'/(f*ln(a))
                        return new ASTNode("/",
                                   FlagNode(right.DeepCopy()),
                                   new ASTNode("*",
                                       right.DeepCopy(),
                                       new ASTNode("ln",
                                           new ASTNode(left.Value))));
                    }
                    throw new Exception("A logaritmus alapja egy pozitív szám, kivéve 1.");

                case "ln": // ln'(f) = f'/f
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("x"));
                    }
                    return new ASTNode("/",
                                       FlagNode(left.DeepCopy()),
                                       left.DeepCopy());

                case "sin": // sin'(f) = cos(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("cos", left.DeepCopy());
                    }
                    return new ASTNode("*",
                               new ASTNode("cos",
                                   left.DeepCopy()),
                               FlagNode(left.DeepCopy()));

                case "cos": // cos'(f)= -1*sin(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("*",
                                   new ASTNode("-1"),
                                   new ASTNode("sin", left.DeepCopy()));
                    }
                    return new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   new ASTNode("sin", left.DeepCopy())),
                               FlagNode(left.DeepCopy()));

                case "tg": // tg'(f) = f'/cos^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("cos",
                                           left.DeepCopy()),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("cos",
                                       left.DeepCopy()),
                                   new ASTNode("2")));

                case "ctg": // ctg'(f) = (-1*f')/sin^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("sin",
                                           left.DeepCopy()),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("^",
                                   new ASTNode("sin",
                                       left.DeepCopy()),
                                   new ASTNode("2")));

                case "arcsin": // arcsin'(f) = f'/(1-f^2)^(1/2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("1"),
                                           new ASTNode("^",
                                               left.DeepCopy(),
                                               new ASTNode("2"))),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2"))),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));

                case "arccos": // arccos'(f) = (-1*f')/(1-f^2)^(1/2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("1"),
                                           new ASTNode("^",
                                               left.DeepCopy(),
                                               new ASTNode("2"))),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2"))),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));

                case "arctg": // arctg'(f) = f'/(1+f^2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("+",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left.DeepCopy(),
                                       new ASTNode("2"))));

                case "arcctg": // arcctg'(f) = (-1*f')/(1+f^2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("+",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left.DeepCopy(),
                                       new ASTNode("2"))));

                case "sh": // sh'(f) = ch(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("ch", left.DeepCopy());
                    }
                    return new ASTNode("*",
                               new ASTNode("ch",
                                   left.DeepCopy()),
                               FlagNode(left.DeepCopy()));

                case "ch": // ch'(f) = sh(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("sh", left.DeepCopy());
                    }
                    return new ASTNode("*",
                               new ASTNode("sh",
                                   left.DeepCopy()),
                               FlagNode(left.DeepCopy()));

                case "th": // th'(f) = f'/ch^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("ch",
                                           left.DeepCopy()),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("ch",
                                       left.DeepCopy()),
                                   new ASTNode("2")));

                case "cth": // cth'(f) = (-1*f')/sh^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("sh",
                                           left.DeepCopy()),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("^",
                                   new ASTNode("sh",
                                       left.DeepCopy()),
                                   new ASTNode("2")));

                case "arsh": // arsh'(f) = f'/(f^2+1)^(1/2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("+",
                                           new ASTNode("^",
                                               left.DeepCopy(),
                                               new ASTNode("2")),
                                           new ASTNode("1")),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("+",
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2")),
                                       new ASTNode("1")),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));

                case "arch": // arch'(f) = f'/(f^2-1)^(1/2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("^",
                                               left.DeepCopy(),
                                               new ASTNode("2")),
                                           new ASTNode("1")),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("-",
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2")),
                                       new ASTNode("1")),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));

                case "arth": // arth'(f) = f'/(1-f^2)
                case "arcth": // arcth'(f) = f'/(1-f^2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left.DeepCopy(),
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("-",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left.DeepCopy(),
                                       new ASTNode("2"))));

                default: throw new Exception($"Nem deriválható: {value}");
            }
        }

        /// <summary>
        /// Sets the NeedsDifferentiation flag of the node to false.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private ASTNode UnflagNode(ASTNode node)
        {
            if (node != null)
            {
                node.ToBeDifferentiated = false;
            }
            return node!;
        }

        /// <summary>
        /// Sets the NeedsDifferentiation flag of the node to true.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private ASTNode FlagNode(ASTNode node)
        {
            if (node != null)
            {
                node.ToBeDifferentiated = true;
            }
            return node!;
        }

        /// <summary>
        /// Recursively traverses the original tree and replaces the toBeDifferentiated node with the differentiated node while preserving the overall structure.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="toBeDifferentiated"></param>
        /// <param name="differentiated"></param>
        /// <returns></returns>
        private ASTNode ReplaceNode(ASTNode original, ASTNode toBeDifferentiated, ASTNode differentiated)
        {
            if (original == null || toBeDifferentiated == null || differentiated == null) return original!;

            if (original == toBeDifferentiated)
            {
                return differentiated;
            }

            original.Left = ReplaceNode(original.Left, toBeDifferentiated, differentiated);
            original.Right = ReplaceNode(original.Right, toBeDifferentiated, differentiated);

            return original;
        }
    }
}
