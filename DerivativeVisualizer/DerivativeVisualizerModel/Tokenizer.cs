using Microsoft.VisualBasic;
using System.Globalization;
using System.Xml.Linq;

namespace DerivativeVisualizerModel
{
    // TODO: Ennél jobban dokumentáld, pontosan definiáld mikor jó az input, a number, a function, stb.
    public class Tokenizer
    {
        private string input;
        private int currentIndex;

        public Tokenizer(string input)
        {
            this.input = input;
            currentIndex = 0;
        }


        /// <summary>
        /// Scans the input string character by character and returns a list of tokens representing numbers,
        /// variables, functions, operators, parentheses, or commas, or an error message if an unexpected character is found.
        /// </summary>
        /// <returns>
        /// Returns the list of tokens and an empty string if the input is correct.
        /// Returns null and an error message if the input is incorrect.
        /// </returns>
        public (List<Token>?,string) Tokenize()
        {
            Token? t;
            string msg;
            List<Token> tokens = new List<Token>();
            while (currentIndex < input.Length)
            {
                char c = input[currentIndex];
                if (char.IsWhiteSpace(c))
                {
                    currentIndex++;
                    continue;
                }
                if (char.IsDigit(c))
                {
                    (t, msg) = TokenizeNumber();
                    if (t is null)
                    {
                        return (null, msg);
                    }
                    tokens.Add(t);
                    continue;
                }
                if (char.IsLetter(c))
                {
                    (t, msg) = TokenizeFunctionOrVariableOrE();
                    if (t is null)
                    {
                        return (null, msg);
                    }
                    tokens.Add(t);
                    continue;
                }
                switch (c)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                        tokens.Add(new Token(c.ToString(), TokenType.Operator));
                        currentIndex++;
                        continue;
                    case '(':
                        tokens.Add(new Token(c.ToString(), TokenType.LeftParen));
                        currentIndex++;
                        continue;
                    case ')':
                        tokens.Add(new Token(c.ToString(), TokenType.RightParen));
                        currentIndex++;
                        continue;
                    case ',':
                        tokens.Add(new Token(c.ToString(), TokenType.Comma));
                        currentIndex++;
                        continue;
                }
                return (null, $"Nem várt karakter: {c}.");
            }
            return (tokens,"");
        }

        /// <summary>
        /// Extracts a numeric token from the input, validating that it contains at most one decimal point and no more than two digits after it, and that its absolute value does not exceed 99.99.
        /// </summary>
        /// <returns>
        /// Returns the number token and an empty string if the input is correct.
        /// Returns null and an error message if the input is incorrect.
        /// </returns>
        private (Token?, string) TokenizeNumber()
        {
            string number = string.Empty;
            bool hasDecimal = false;
            int digitsAfterDecimal = 0;

            while (currentIndex < input.Length &&
                   (char.IsDigit(input[currentIndex]) || input[currentIndex] == '.'))
            {
                char currentChar = input[currentIndex];

                if (currentChar == '.')
                {
                    if (hasDecimal)
                    {
                        return (null, "Több tizedespont nem lehet egy számban.");
                    }
                    hasDecimal = true;
                }
                else if (hasDecimal)
                {
                    digitsAfterDecimal++;
                    if (digitsAfterDecimal > 2)
                    {
                        return (null, "A tizedespont után csak 2 szám engedélyezett.");
                    }
                }

                number += currentChar;
                currentIndex++;
            }

            if (number.EndsWith("."))
            {
                return (null, "A szám nem végződhet tizedesponttal.");
            }

            if (Math.Abs(double.Parse(number, NumberStyles.Float, CultureInfo.InvariantCulture)) > 99.99)
            {
                return (null, "A szám abszolút értékének maximuma 99.99 lehet.");
            }

            return (new Token(number, TokenType.Number), "");
        }

        /// <summary>
        /// Extracts a token representing a known mathematical function, the variable x, or the constant e, and returns an error message if the name is not recognized.
        /// </summary>
        /// <returns>
        /// Returns the variable or the function and an empty string if the input is correct.
        /// Returns null and an error message if the input is incorrect.
        /// </returns>
        private (Token?,string) TokenizeFunctionOrVariableOrE()
        {
            string name = string.Empty;
            while(currentIndex < input.Length && char.IsLetter(input[currentIndex]))
            {
                name += input[currentIndex];
                currentIndex++;
            }

            string[] functions = {"log", "ln", "sin", "cos", "tg", "ctg", "arcsin", "arccos", "arctg", "arcctg", "sh", "ch", "th", "cth", "arsh", "arch", "arth", "arcth"};
            if (Array.Exists(functions, func => func == name))
            {
                return (new Token(name, TokenType.Function), "");
            }
            if (name == "x")
            {
                return (new Token(name, TokenType.Variable), "");
            }
            if (name == "e")
            {
                return (new Token(name, TokenType.Number), "");
            }

            return (null,$"Ismeretlen token: {name}. Csak az 'x' változó és az ismert függvények engedélyezettek.");
        }
    }
}