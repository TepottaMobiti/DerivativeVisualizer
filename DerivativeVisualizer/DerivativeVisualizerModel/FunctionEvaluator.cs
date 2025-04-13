using Fractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    // Dokumentáld majd le, hogy hogy működik, minden függvényt ami szóra érdemes. A matekról is ejts szót, az epszilonozásról, stb. TODO: Tear in Function Plot beszben van egy generált indoklás.
    // Tangens dilemma: Mindenhogy rossz. Lehet megoldás, hogy csak -pista /2, pista /2-n értelmezzük? Az lenne még talán a legjobb. Ctg ugyanez. Ez mondjuk nem ideális. De pl. azt lehet
    // biztosítani, hogy csak az egyik intervallumot értelmezzük, amit éppen célszerű.

    // Dokumentáld le majd a hatványozást részletesen. Mindent dokumentálj le részletesen igazából. Térj ki arra, hogy a BigIntegert intté konvertálni miért szabad (mert a pontok generálása
    // miatt nem lesznek olyan hú de nagy számok)

    // ChatGPT beszélgetés: Dokumentáció: Racionális számok

    public static class FunctionEvaluator
    {
        /// <summary>
        /// Recursively evaluates the abstract syntax tree at a specific x value, handling arithmetic operations, mathematical functions, and special cases like discontinuities and division by zero.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xValue"></param>
        /// <param name="stepSize"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double Evaluate(ASTNode node, double xValue, double stepSize)
        {
            if (node == null)
                throw new Exception("Adjon meg egy függvényt az ábrázoláshoz.");

            double epsilon = stepSize * 0.5;

            // Floating point aritmetikai hibák korrigálása.
            epsilon = Math.Round(epsilon, 6, MidpointRounding.AwayFromZero);
            xValue = Math.Round(xValue, 6, MidpointRounding.AwayFromZero);

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
                double left = Evaluate(node.Left, xValue, stepSize);
                double right = Evaluate(node.Right, xValue, stepSize);
                return node.Value switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => Math.Abs(right) <= epsilon ? double.NaN : left / right,
                    "^" => SafePower(left, right),
                    _ => throw new Exception($"Ismeretlen operátor: {node.Value}")
                };
            }
            else if (node.IsFunction())
            {
                // TODO: Ennek a matekját majd azért írd majd még le, hogy biztos értelmes-e, meg majd a dokumentációban is fejtegesd meg. Thetawise biztos segít. Írj neki körítést is.
                double argument = Evaluate(node.Left, xValue, stepSize);
                double pi = Math.PI;
                double n;
                double discontinuity;
                if (node.Value == "tg")
                {
                    n = Math.Round((argument - (pi / 2)) / pi);
                    discontinuity = (pi / 2) + n * pi;

                    return Math.Abs(argument - discontinuity) <= epsilon /*1e-10*/ ? double.NaN : Math.Tan(argument);
                }
                if (node.Value == "ctg")
                {
                    n = Math.Round(argument / pi);
                    discontinuity = n * pi;

                    return Math.Abs(argument - discontinuity) <= epsilon /*1e-10*/? double.NaN : 1 / Math.Tan(argument);
                }
                return node.Value switch
                {
                    "sin" => Math.Sin(argument),
                    "cos" => Math.Cos(argument),
                    "arcsin" => Math.Asin(argument),
                    "arccos" => Math.Acos(argument),
                    "arctg" => Math.Atan(argument),
                    "arcctg" => Math.PI / 2 - Math.Atan(argument),
                    "sh" => Math.Sinh(argument),
                    "ch" => Math.Cosh(argument),
                    "th" => Math.Tanh(argument),
                    "cth" => Math.Abs(argument) <= epsilon ? 1 / Math.Tanh(argument) : double.NaN,
                    "arsh" => Math.Asinh(argument),
                    "arch" => Math.Acosh(argument),
                    "arth" => Math.Atanh(argument),
                    "arcth" => Math.Abs(argument) > 1 ? 0.5 * Math.Log((argument + 1) / (argument - 1)) : double.NaN,
                    "ln" => Math.Log(argument),
                    "log" => Math.Log(Evaluate(node.Right, xValue, stepSize), argument),
                    _ => throw new Exception($"Ismeretlen függvény: {node.Value}")
                };
            }
            throw new Exception($"Nem feldolgozható érték: {node.Value}");
        }

        /// <summary>
        /// Safely computes the power function, especially for negative bases with fractional exponents, returning NaN if the result is not a real number or the exponent is too complex to process.
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static double SafePower(double baseValue, double exponent)
        {
            double result = Math.Pow(baseValue, exponent);
            if (!double.IsNaN(result))
                return result;

            if (baseValue < 0)
            {
                try
                {
                    var fraction = Fraction.FromDoubleRounded(exponent); // Optional tolerance
                    var numeratorBig = fraction.Numerator;
                    var denominatorBig = fraction.Denominator;

                    // Avoid overflow: refuse to process large fractions
                    const int maxIntValue = 1000000;
                    if (numeratorBig > maxIntValue || denominatorBig > maxIntValue)
                        return double.NaN;

                    int numerator = (int)numeratorBig;
                    int denominator = (int)denominatorBig;

                    if (denominator % 2 == 1) // Odd denominator → real result possible
                    {
                        double rootBase = Math.Pow(-baseValue, Math.Abs((double)numerator) / denominator);
                        double signedResult = numerator < 0 ? 1.0 / rootBase : rootBase;

                        return (numerator % 2 == 0) ? signedResult : -signedResult;
                    }
                }
                catch
                {
                    // Conversion or pow failed
                    return double.NaN;
                }
            }

            return double.NaN;
        }

    }
}
