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
        private ASTNode parsedExpr => parser.ParseExpression().Item1 ?? null!;

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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            ASTNode actualExpr = new ASTNode("-2");
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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
            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
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

            Assert.IsTrue(ASTNode.AreTreesEqual(parsedExpr, actualExpr));
        }

        /// <summary>
        /// Tests if the error message is correct if the token list is empty.
        /// </summary>
        [TestMethod]
        public void TestEmptyTokenList()
        {
            tokens = new List<Token>();
            IsErrorMessageCorrect("Nem várt token a kifejezésben.");
        }

        /// <summary>
        /// Tests if the error message is correct when an operator is missing its right operand.
        /// </summary>
        [TestMethod]
        public void TestMissingRightOperand()
        {
            tokens = new List<Token>()
            {
                new Token("x",TokenType.Variable),
                new Token("*",TokenType.Operator)
            };
            IsErrorMessageCorrect("A(z) '*' bináris operátornak hiányzik a jobb oldali operandusa.");
        }

        /// <summary>
        /// Tests if the error message is correct if the expression contains division by zero.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestDivisionByZero()
        {
            tokens = new List<Token>()
            {
                new Token("2",TokenType.Number),
                new Token("/",TokenType.Operator),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("-",TokenType.Operator),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("Nullával való osztás nem engedélyezett.");
        }

        /// <summary>
        /// Tests if the error message is correct when the exponentiation operator is missing its right operand.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestExponentiationExponentMissing()
        {
            tokens = new List<Token>()
            {
                new Token("2",TokenType.Number),
                new Token("^",TokenType.Operator)
            };
            IsErrorMessageCorrect("A '^' hatványozás operátornak hiányzik a kitevője.");
        }

        /// <summary>
        /// Tests if the error message is correct when parser encounters 0^0.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestZeroToThePowerOfZero()
        {
            tokens = new List<Token>()
            {
                new Token("0",TokenType.Number),
                new Token("^",TokenType.Operator),
                new Token("0",TokenType.Number)
            };
            IsErrorMessageCorrect("A 0^0 nem értelmezett.");
        }

        /// <summary>
        /// Tests if the error message is correct when the base of an exponentiation is negative or zero.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestExponentiationNegativeBase()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("-1",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token("^",TokenType.Operator),
                new Token("x",TokenType.Variable)
            };
            IsErrorMessageCorrect("A hatványozás alapja nem lehet negatív szám vagy 0.");
        }

        /// <summary>
        /// Tests if the error message is correct when the unary minus is not followed by a number.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestUnaryMinusNotFollowedByNumber()
        {
            tokens = new List<Token>()
            {
                new Token("-",TokenType.Operator),
                new Token("x",TokenType.Variable)
            };
            IsErrorMessageCorrect("A negatív előjelet ('-') számnak kell követnie.");
        }

        /// <summary>
        /// Tests if the error message is correct when an operator is missing after a negative number.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestMissingOperatorAfterNegativeNumber()
        {
            tokens = new List<Token>()
            {
                new Token("-",TokenType.Operator),
                new Token("1",TokenType.Number),
                new Token("x",TokenType.Variable)
            };
            IsErrorMessageCorrect("Hiányzó operátor a -1 után.");
        }

        /// <summary>
        /// Tests if the error message is correct when an operator is missing after a number.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestMissingOperatorAfterNumber()
        {
            tokens = new List<Token>()
            {
                new Token("5",TokenType.Number),
                new Token("x",TokenType.Variable)
            };
            IsErrorMessageCorrect("Hiányzó operátor a(z) 5 után.");
        }

        /// <summary>
        /// Tests if the error message is correct when a basic function name is not followed by a parentheses.
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
            IsErrorMessageCorrect("A(z) 'sin' függvény után nyitó zárójelnek kell következnie: '('.");
        }

        /// <summary>
        /// Tests if the error message is correct when a basic function has no argument.
        /// The parser does not distinguish between basic functions, so testing it with one of them is enough.
        /// </summary>
        [TestMethod]
        public void TestFunctionWithoutArgument()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("A(z) 'sin' függvénynek hiányzik az argumentuma.");
        }

        /// <summary>
        /// Tests if the error message is correct when the logarithm function has no base.
        /// </summary>
        [TestMethod]
        public void TestLogWithNoBase()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("A logaritmus függvényből hiányzik az alap.");
        }

        /// <summary>
        /// Tests if the error message is correct when a logarithm's base is not a number.
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
            IsErrorMessageCorrect("A logaritmus alapjának pozitív számnak kell lennie.");
        }

        /// <summary>
        /// Tests if the error message is correct when a logarithm's base is 1.
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
            IsErrorMessageCorrect("A logaritmus alapja nem lehet 1.");
        }

        /// <summary>
        /// Tests if the error message is correct when a logarithm's base is 0.
        /// </summary>
        [TestMethod]
        public void TestLogarithm0Base()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("0",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("A logaritmus alapja nem lehet 0.");
        }

        /// <summary>
        /// Tests if the error message is correct when a logarithm's base is not followed by a comma.
        /// </summary>
        [TestMethod]
        public void TestLogarithWithoutComma()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("2",TokenType.Number),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("A logaritmus alapja után vesszőnek kell következnie: ','.");
        }

        /// <summary>
        /// Tests if the error message is correct when a logarithm's argument is missing.
        /// </summary>
        [TestMethod]
        public void TestLogarithWithoutArgument()
        {
            tokens = new List<Token>()
            {
                new Token("log",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("2",TokenType.Number),
                new Token(",",TokenType.Comma),
                new Token(")",TokenType.RightParen)
            };
            IsErrorMessageCorrect("A logaritmus függvényből hiányzik az argumentum a vessző után.");
        }

        /// <summary>
        /// Tests if the error message is correct when a function's closing parenthesis is missing.
        /// </summary>
        [TestMethod]
        public void TestFunctionWithoutClosingParenthesis()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable)
            };
            IsErrorMessageCorrect("A(z) sin függvény argumentuma után berekesztő zárójelnek kell következnie: ')'.");
        }

        /// <summary>
        /// Tests if the error message is correct when an operator is missing after a function's closing parenthesis.
        /// </summary>
        [TestMethod]
        public void TestMissingOperatorAfterFunctionClosingParenthesis()
        {
            tokens = new List<Token>()
            {
                new Token("sin",TokenType.Function),
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token(")",TokenType.RightParen),
                new Token("2",TokenType.Number)
            };
            IsErrorMessageCorrect("A(z) sin függvény berekesztő zárójele után operátornak kell következnie.");
        }

        /// <summary>
        /// Tests if the error message is correct when an expression is missing a closing parenthesis.
        /// </summary>
        [TestMethod]
        public void TestMissingClosingParenthesis()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number)
            };
            IsErrorMessageCorrect("A kifejezésből hiányzik egy berekesztő zárójel: ')'.");
        }

        /// <summary>
        /// Tests if the error message is correct when an operator is missing after a closing parenthesis.
        /// </summary>
        [TestMethod]
        public void TestMissingOperatorAfterClosingParenthesis()
        {
            tokens = new List<Token>()
            {
                new Token("(",TokenType.LeftParen),
                new Token("x",TokenType.Variable),
                new Token("+",TokenType.Operator),
                new Token("2",TokenType.Number),
                new Token(")",TokenType.RightParen),
                new Token("2",TokenType.Number)
            };
            IsErrorMessageCorrect("A berekesztő zárójel után operátornak kell következnie.");
        }

        /// <summary>
        /// Tests if the error message is correct when an unexpected token is encountered.
        /// </summary>
        [TestMethod]
        public void TestUnexpectedToken()
        {
            tokens = new List<Token>()
            {
                new Token(",",TokenType.Comma)
            };
            IsErrorMessageCorrect("Nem várt token a kifejezésben.");
        }

        /// <summary>
        /// Determines if the error message is correct when parsing an incorrect expression.
        /// </summary>
        /// <param name="expectedErrorMsg"></param>
        private void IsErrorMessageCorrect(string expectedErrorMsg)
        {
            var (tree, msg) = parser.ParseExpression();
            Assert.IsTrue(tree is null && msg == expectedErrorMsg);
        }
    }
}