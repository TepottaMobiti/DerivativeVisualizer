using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivateVisualizerModelTest
{
    [TestClass]
    public class ParserTest
    {
        private List<Token>? tokens;
        private Parser parser => new Parser(tokens ?? new List<Token>());
        private ASTNode parsedExpr => parser.ParseExpression();

        /// <summary>
        /// Tests if empty token list creates an exception.
        /// </summary>
        [TestMethod]
        public void TestEmptyTokenList()
        {
            tokens = new List<Token>();
            Assert.ThrowsException<Exception>(() => parser.ParseExpression());
        }

        /// <summary>
        /// Tests if the symbol "e" is accepted as a number.
        /// </summary>
        [TestMethod]
        public void TestNumberE()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("e",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };

            ASTNode actualExpr = new ASTNode("log", new ASTNode("e"), new ASTNode("x"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if addition is parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestAddition()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("+", new ASTNode("x"), new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if subtraction is parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestSubtraction()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("-",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("-", new ASTNode("x"), new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if multiplication is parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestMultiplication()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("*",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("*", new ASTNode("x"), new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if division is parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestDivision()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("/",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("/", new ASTNode("x"), new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if exponentiation is parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestExponentiation()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("^",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("^", new ASTNode("x"), new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if negative expressions are parsed correctly.
        /// </summary>
        [TestMethod]
        public void TestNegation()
        {
            tokens = new List<Token>()
            {
                new Token("-",TokenType.Operator),
                new Token("2",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("NEGATE", new ASTNode("2"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if function expressions f(x) are parsed correctly.
        /// The parser does not distinguish between basic functions, so testing it with one of them is enough.
        /// </summary>
        [TestMethod]
        public void TestFunctionWithParentheses()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };

            ASTNode actualExpr = new ASTNode("sin", new ASTNode("x"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if function expressions f x create an exception.
        /// The parser does not distinguish between basic functions, so testing it with one of them is enough.
        /// </summary>
        [TestMethod]
        public void TestFunctionWithoutParentheses()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("x",TokenType.Variable)
            };

            Assert.ThrowsException<Exception>(() => parser.ParseExpression());
        }

        /// <summary>
        /// Tests if logarithmic expressions are parsed correctly.
        /// Logarithm is the only function that contains a comma: log(base,argument).
        /// Base should be a positive number other than one.
        /// </summary>
        [TestMethod]
        public void TestLogarithmCorrectBase()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("2",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };

            ASTNode actualExpr = new ASTNode("log", new ASTNode("2"), new ASTNode("x"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if logarithmic expressions with not number bases create an exception.
        /// </summary>
        [TestMethod]
        public void TestLogarithmNotNumberBase()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token(",",TokenType.Comma),
                new Token("2",TokenType.Number),
                new Token(")",TokenType.RightParen)
            };

            Assert.ThrowsException<Exception>(() => parser.ParseExpression());
        }

        /// <summary>
        /// Tests if logarithmic expressions with base 1 create an exception.
        /// </summary>
        [TestMethod]
        public void TestLogarithm1Base()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("1",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };

            Assert.ThrowsException<Exception>(() => parser.ParseExpression());
        }

        /// <summary>
        /// Tests if a term is grouped left to right.
        /// This means that the input "x+2-3" is grouped like this: "(x+2)-3".
        /// </summary>
        [TestMethod]
        public void TestTermAssociativity()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("-",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("-",
                                     new ASTNode("+",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("3"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if a factor is grouped left to right.
        /// This means that the input "x*2/3" is grouped like this: "(x*2)/3".
        /// </summary>
        [TestMethod]
        public void TestFactorAssociativity()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("*",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("/",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("3"));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if an exponent is grouped right to left.
        /// This means that the input "x^2^3" is grouped like this: "x^(2^3)".
        /// </summary>
        [TestMethod]
        public void TestExponentAssociativity()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("^",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("^",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("^",
                                     new ASTNode("x"),
                                     new ASTNode("^",
                                         new ASTNode("2"),
                                         new ASTNode("3")));
            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if multiplication has a higher precedence than addition.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfAdditionAndMultiplication()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("*",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("+",
                                     new ASTNode("x"),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("3")));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if parentheses can change the precedence of addition and multiplication.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfAdditionAndMultiplicationWithParentheses()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token("*",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("*",
                                     new ASTNode("+",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("3"));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if exponentiation has a higher precedence than addition.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfAdditionAndExponentiation()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("^",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("+",
                                     new ASTNode("x"),
                                     new ASTNode("^",
                                         new ASTNode("2"),
                                         new ASTNode("3")));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if parentheses can change the precedence of addition and exponentiation.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfAdditionAndExponentiationWithParentheses()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token("^",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("^",
                                     new ASTNode("+",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("3"));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if exponentiation has a higher precedence than multiplication.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfMultiplicationAndExponentiation()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("*",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token("^",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("*",
                                     new ASTNode("x"),
                                     new ASTNode("^",
                                         new ASTNode("2"),
                                         new ASTNode("3")));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if parentheses can change the precedence of multiplication and exponentiation.
        /// </summary>
        [TestMethod]
        public void TestPrecedenceOfMultiplicationAndExponentiationWithParentheses()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("*",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token("^",TokenType.Operator),
                new Token("3",TokenType.Number)
            };

            ASTNode actualExpr = new ASTNode("^",
                                     new ASTNode("*",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("3"));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if the parser can handle function composition correctly.
        /// </summary>
        [TestMethod]
        public void TestFunctionComposition()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("2",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen),
                new Token("*",TokenType.Operator),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("3",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token(")",TokenType.RightParen)
            };

            ASTNode actualExpr = new ASTNode("sin",
                                     new ASTNode("*",
                                         new ASTNode("log",
                                             new ASTNode("2"),
                                             new ASTNode("x")),
                                         new ASTNode("+",
                                             new ASTNode("x"),
                                             new ASTNode("3"))));

            Assert.IsTrue(AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Determines if two trees are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool AreTreesEqual(ASTNode a, ASTNode b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Value != b.Value) return false;
            return AreTreesEqual(a.Left,b.Left) && AreTreesEqual(a.Right,b.Right);
        }
    }
}