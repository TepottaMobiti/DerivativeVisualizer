using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DerivativeVisualizerModel
{
    // TODO: Ellenőrizd, hogy a Deriváló függvényben biztosan nincsenek-e referencia miatti hibák: ahol nem új ASTNode-ot csinálsz, ott biztos nincs gyereke a használt Node-nak? Ha félsz,
    // csinálj mindenhol új ASTNode-ot
    // TODO: Ha már majd az ID alapján történő keresést csinálod (kattintás vezérelt deriválás), akkor figyelj arra, hogy az ID-k is egyezzenek. Például a DeepCopy metódus mentse át az ID-kat is.
    // Legyen egy statikus ID mezője az ASTNode-nak, ami új Node létrehozásakor automatikusan egy új, egyedi ID-t ad, de lehessen valahogy kívülről is felülírni az ID-t DeepCopy-hoz.
    public class Differentiator
    {
        private ASTNode root;
        private List<ASTNode> differentiationSteps = new List<ASTNode>();

        public Differentiator(ASTNode root)
        {
            this.root = root;
            FlagNode(root);
            differentiationSteps.Add(root);
        }

        public List<ASTNode> GetSteps() => differentiationSteps;

        public ASTNode CurrentTree
        {
            get => differentiationSteps.Last();
        }

        public ASTNode Differentiate(int locator)
        {
            ASTNode lastTree = differentiationSteps.Last().DeepCopy();

            ASTNode nodeToDifferentiate = FindDifferentiationNode(lastTree, locator);

            ASTNode differentiatedNode = DifferentiateOnce(nodeToDifferentiate);

            ASTNode newTree = ReplaceNode(lastTree, nodeToDifferentiate, differentiatedNode);

            differentiationSteps.Add(newTree);
            return newTree;
        }

        public ASTNode FindDifferentiationNode(ASTNode node, int locator)
        {
            if (node == null) return null!;
            if (node.NeedsDifferentiation && node.Locator == locator) return node;

            ASTNode found = FindDifferentiationNode(node.Left, locator);
            return found ?? FindDifferentiationNode(node.Right, locator);
        }

        public void Differentiate()
        {
            do
            {
                ASTNode lastTree = differentiationSteps.Last().DeepCopy();
                
                //Megkeresni egy node-ot, ami deriválásra vár (legyen mondjuk A.).
                ASTNode nodeToDifferentiate = FindNextDifferentiationNode(lastTree);

                //Deriválni A.-t -> Új deriválni kívánt node-ok.
                ASTNode differentiatedNode = DifferentiateOnce(nodeToDifferentiate);

                //Kicserélni a kiindulási fában az A. node-ot a derivált node-dal.
                ASTNode newTree = ReplaceNode(lastTree,nodeToDifferentiate,differentiatedNode);

                //Elmenteni a kapott fát.
                differentiationSteps.Add(newTree);
            }
            while (FindNextDifferentiationNode(differentiationSteps.Last()) != null); // Amíg van deriválásra váró node.

        }

        // TODO: A végső cleanupnál ellenőrizni, hogy van-e felesleges művelet (pl. DeepCopy)
        // TODO: Kell, hogy az x-nél leálljon a deriválás mindenhol. (Ellenőrzés)
        public ASTNode DifferentiateOnce(ASTNode? node)
        {
            if (node is null)
            {
                return null!;
            }
            string value = node.Value;
            ASTNode? left = node.Left;
            ASTNode? right = node.Right;
            if (double.TryParse(value, out _) || value == "e") //a' = 0 (a is real)
            {
                return new ASTNode("0");
            }
            switch (value)
            {
                case "x": return new ASTNode("1"); // x' = 1
                case "+": // (f+g)' = f' + g'
                case "-": // (f-g)' = f' - g'
                    return new ASTNode(value, FlagNode(left.DeepCopy()), FlagNode(right.DeepCopy()));
                case "*": // (f*g)' = f'*g+f*g'
                    return new ASTNode("+",
                               new ASTNode("*", FlagNode(left.DeepCopy()), right),
                               new ASTNode("*", left, FlagNode(right.DeepCopy())));
                case "/": // (f/g)' / (f'g-f*g')/g^2
                    return new ASTNode("/",
                               new ASTNode("-",
                                   new ASTNode("*", FlagNode(left.DeepCopy()), right),
                                   new ASTNode("*", left, FlagNode(right.DeepCopy()))),
                               new ASTNode("^", right, new ASTNode("2")));
                case "^":
                    if (left.Value == "e" && right.Value == "x") // (e^x)' = e^x
                    {
                        return node;
                    }
                    else if (left.Value == "e") // (e^f)' = e^f*f'
                    {
                        return new ASTNode("*",
                                   new ASTNode("^",
                                       new ASTNode("e"),
                                       right),
                                   FlagNode(right.DeepCopy()));
                    }
                    else if (double.TryParse(left.Value, out double a) && right.Value == "x") // (a^x)' = a^x*ln(a) (a>0)
                    {
                        if (a > 0)
                        {
                            return new ASTNode("*",
                                       node,
                                       new ASTNode("ln", new ASTNode(left.Value)));
                        }
                        else
                        {
                            throw new Exception($"Can't differentiate a^x if a <= 0 (a = {a})");
                        }
                    }
                    else if ((double.TryParse(right.Value, out _) || right.Value == "e") && left.Value=="x") // (x^n)' =  n*x^(n-1)
                    {
                        if (right.Value=="e")
                        {
                            return new ASTNode("*",
                                       right,
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode((Math.E - 1).ToString())));
                        }
                        return new ASTNode("*",
                                   right,
                                   new ASTNode("^",
                                       left,
                                       new ASTNode((double.Parse(right.Value) - 1).ToString())));
                    }
                    else if (double.TryParse(right.Value, out _) || right.Value == "e") // (f^n)' = n*f^(n-1)*f'
                    {
                        if (right.Value == "e")
                        {
                            return new ASTNode("*",
                                       new ASTNode("*",
                                           right,
                                           new ASTNode("^",
                                               left,
                                               new ASTNode((Math.E - 1).ToString()))),
                                        FlagNode(left.DeepCopy()));
                        }
                        return new ASTNode("*",
                                   new ASTNode("*",
                                       right,
                                       new ASTNode("^",
                                           left,
                                           new ASTNode((double.Parse(right.Value) - 1).ToString()))),
                                    FlagNode(left.DeepCopy()));
                    }
                    else // (f^g)' = (f^g)*(f'*g/f+g'*ln(f)) (f>0, not checked)
                    {
                        return new ASTNode("*",
                                   new ASTNode("^", left, right),
                                   new ASTNode("+",
                                       new ASTNode("*",
                                           FlagNode(left.DeepCopy()),
                                           new ASTNode("/", right, left)),
                                       new ASTNode("*",
                                           FlagNode(right.DeepCopy()),
                                           new ASTNode("ln", left))));
                    }
                case "log":
                    if (double.TryParse(left.Value, out double b) || left.Value == "e")
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
                                       right);
                        }
                        if (b <= 0 || b == 1)
                        {
                            throw new Exception("The base of the logarithm should be a positive number other than 1.");
                        }
                        if (right.Value == "x") //log_a'(x) = 1/(x*ln(a))
                        {
                            return new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("*",
                                           right,
                                           new ASTNode("ln",
                                               new ASTNode(left.Value))));
                        }
                        // log_a'(f) = f'/(f*ln(a))
                        return new ASTNode("/",
                                   FlagNode(right.DeepCopy()),
                                   new ASTNode("*",
                                       right,
                                       new ASTNode("ln",
                                           new ASTNode(left.Value))));
                    }
                    throw new Exception("The base of the logarithm should be a positive number other than 1.");
                case "ln": // ln'(f) = f'/f
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("x"));
                    }
                    return new ASTNode("/",
                                       FlagNode(left.DeepCopy()),
                                       left);
                case "sin": // sin'(f) = cos(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("cos", left);
                    }
                    return new ASTNode("*",
                               new ASTNode("cos",
                                   left),
                               FlagNode(left.DeepCopy()));
                case "cos": // cos'(f)= -1*sin(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("*",
                                   new ASTNode("-1"),
                                   new ASTNode("sin", left));
                    }
                    return new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   new ASTNode("sin", left)),
                               FlagNode(left.DeepCopy()));
                case "tg": // tg'(f) = f'/cos^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("cos",
                                           left),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("cos",
                                       left),
                                   new ASTNode("2")));
                case "ctg": // ctg'(f) = (-1*f')/sin^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("sin",
                                           left),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("^",
                                   new ASTNode("sin",
                                       left),
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
                                               left,
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
                                           left,
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
                                               left,
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
                                           left,
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
                                           left,
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                case "arcctg": // arcctg'(f) = (-1*f')/(1+f^2)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("+",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           left,
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("+",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                case "sh": // sh'(f) = ch(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("ch", left);
                    }
                    return new ASTNode("*",
                               new ASTNode("ch",
                                   left),
                               FlagNode(left.DeepCopy()));
                case "ch": // ch'(f) = sh(f)*f'
                    if (left.Value == "x")
                    {
                        return new ASTNode("sh", left);
                    }
                    return new ASTNode("*",
                               new ASTNode("sh",
                                   left),
                               FlagNode(left.DeepCopy()));
                case "th": // th'(f) = f'/ch^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("ch",
                                           left),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("^",
                                   new ASTNode("ch",
                                       left),
                                   new ASTNode("2")));
                case "cth": // cth'(f) = (-1*f')/sh^2(f)
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("sh",
                                           left),
                                       new ASTNode("2")));
                    }
                    return new ASTNode("/",
                               new ASTNode("*",
                                   new ASTNode("-1"),
                                   FlagNode(left.DeepCopy())),
                               new ASTNode("^",
                                   new ASTNode("sh",
                                       left),
                                   new ASTNode("2")));
                case "arsh": // arsh'(f) = f'/(f^2+1)^1/2
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("+",
                                           new ASTNode("^",
                                               left,
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
                                           left,
                                           new ASTNode("2")),
                                       new ASTNode("1")),
                                   new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("2"))));
                case "arch": // arch'(f) = f'/(f^2-1)^1/2
                    if (left.Value == "x")
                    {
                        return new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("^",
                                               left,
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
                                           left,
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
                                           left,
                                           new ASTNode("2"))));
                    }
                    return new ASTNode("/",
                               FlagNode(left.DeepCopy()),
                               new ASTNode("-",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       left,
                                       new ASTNode("2"))));
                default: throw new Exception($"Can't differentiate node with value {value}");
            }
        }

        private ASTNode FindNextDifferentiationNode(ASTNode node)
        {
            if (node == null) return null!;
            if (node.NeedsDifferentiation) return node;

            ASTNode found = FindNextDifferentiationNode(node.Left);
            return found ?? FindNextDifferentiationNode(node.Right);
        }

        

        private ASTNode FlagNode(ASTNode node)
        {
            if (node != null)
            {
                node.NeedsDifferentiation = true;
            }
            return node!;
        }

        private ASTNode ReplaceNode(ASTNode root, ASTNode target, ASTNode replacement)
        {
            if (root == null || target == null || replacement == null) return root!;

            if (root == target)
            {
                return replacement;
            }

            root.Left = ReplaceNode(root.Left, target, replacement);
            root.Right = ReplaceNode(root.Right, target, replacement);

            return root;
        }
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
                                   new ASTNode("*", Differentiate(left),right),
                                   new ASTNode("*",left, Differentiate(right))),
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
