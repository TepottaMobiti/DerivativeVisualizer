using DerivativeVisualizerModel;

namespace DerivateVisualizerModelTest
{
    [TestClass]
    public class TokenizerTest
    {
        private string? input;
        private Tokenizer tokenizer
        {
            get
            {
                return new Tokenizer(input ?? "");
            }
        }
        private List<Token> tokenizedTokens
        {
            get
            {
                return tokenizer.Tokenize().Item1 ?? new List<Token>();
            }
        }
        private string[]? actualTokens;

        /// <summary>
        /// Tests if empty input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestEmptyInput()
        {
            input = "";
            Assert.AreEqual(0, tokenizedTokens.Count);
        }

        /// <summary>
        /// Tests if whitespace input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestWhitespaceInput()
        {
            input = "  \n  \t ";
            Assert.AreEqual(0, tokenizedTokens.Count);
        }

        /// <summary>
        /// Tests if polynomial input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestPolinomialInput()
        {
            input = "x ^ 2 + 2 * x + 3";
            actualTokens = ["x", "^", "2", "+", "2", "*", "x", "+", "3"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if exponential input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestExponentialInput()
        {
            input = "2^x / x^3";
            actualTokens = ["2","^","x","/","x","^","3"];
            IsTokenizationCorrect(input, actualTokens);
        }
        
        /// <summary>
        /// Tests if logarithmic input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestLogarithmicInput()
        {
            input = "log(2,x) ^ log(x,3)";
            actualTokens = ["log", "(", "2", ",", "x", ")", "^", "log", "(", "x", ",", "3", ")"];
            IsTokenizationCorrect(input,actualTokens);
        }

        /// <summary>
        /// Tests if trigonometric functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestTrigonometricInput()
        {
            input = "sin(x) + cos(3*x) - tg(4*x) * ctg(2*x)";
            actualTokens = ["sin","(","x",")","+","cos","(","3","*","x",")","-","tg","(","4","*","x",")","*","ctg","(","2","*","x",")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if inverse trigonometric functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestTrigonometricInverseInput()
        {
            input = "arcsin(x) + arccos(3-x) - arctg(4^x) * arcctg(2+x)";
            actualTokens = ["arcsin", "(", "x", ")", "+", "arccos", "(", "3", "-", "x", ")", "-", "arctg", "(", "4", "^", "x", ")", "*", "arcctg", "(", "2", "+", "x", ")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if hyperbolic functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestHyperbolicInput()
        {
            input = "sh(x) + ch(3/x) - th(4^x) / cth(2*x)";
            actualTokens = ["sh", "(", "x", ")", "+", "ch", "(", "3", "/", "x", ")", "-", "th", "(", "4", "^", "x", ")", "/", "cth", "(", "2", "*", "x", ")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if inverse hyperbolic functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestHyperbolicInverseInput()
        {
            input = "arsh(x) * arch(3^x) - arth(x^2) * arcth(2*x)";
            actualTokens = ["arsh", "(", "x", ")", "*", "arch", "(", "3", "^", "x", ")", "-", "arth", "(", "x", "^", "2", ")", "*", "arcth", "(", "2", "*", "x", ")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if decimal input is tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestDecimalNumberInput()
        {
            input = "3.5";
            Assert.AreEqual(1, tokenizedTokens.Count);
            Assert.AreEqual("3.5", tokenizedTokens[0].Value);
        }

        /// <summary>
        /// Tests if the error message is correct if there are multiple decimal points in a number.
        /// </summary>
        [TestMethod]
        public void TestMultipleDecimalPointsInput()
        {
            IsErrorMessageCorrect("2.3.4", "Több tizedespont nem lehet egy számban.");
        }

        /// <summary>
        /// Tests if the error message is correct if there are more than 2 decimal places in a number.
        /// </summary>
        [TestMethod]
        public void TestMorethan2DecimalPlacesInANumber()
        {
            IsErrorMessageCorrect("2.354", "A tizedespont után csak 2 szám engedélyezett.");
        }

        /// <summary>
        /// Tests if the error message is correct if a number ends with a decimal point.
        /// </summary>
        [TestMethod]
        public void TestNumberEndsWithDecimalPoint()
        {
            IsErrorMessageCorrect("2.", "A szám nem végzõdhet tizedesponttal.");
        }

        /// <summary>
        /// Tests if the error message is correct if a number's absolute value is larger than 100.
        /// </summary>
        [TestMethod]
        public void TestNumberWithAbsoluteValueGreaterThanOrEqualTo100()
        {
            IsErrorMessageCorrect("100", "A szám abszolút értékének maximuma 99.99 lehet.");
        }

        /// <summary>
        /// Tests if the error message is correct if the tokenizer encounters an unexpected character.
        /// </summary>
        [TestMethod]
        public void TestUnexpectedCharacter()
        {
            IsErrorMessageCorrect("&", "Nem várt karakter: &.");
        }

        /// <summary>
        /// Tests if the error message is correct if a function is unknown.
        /// </summary>
        [TestMethod]
        public void TestUnknownFunctionInput()
        {
            IsErrorMessageCorrect("sec(x)", "Ismeretlen token: sec. Csak az 'x' változó és az ismert függvények engedélyezettek.");
        }

        /// <summary>
        /// Tests if the error message is correct if the variable of an input is not x.
        /// </summary>
        [TestMethod]
        public void TestInvalidVariableInput()
        {
            IsErrorMessageCorrect("y^2", "Ismeretlen token: y. Csak az 'x' változó és az ismert függvények engedélyezettek.");
        }

        /// <summary>
        /// Determines if the tokenization of the input is correct.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="actualTokens"></param>
        private void IsTokenizationCorrect(string input, string[] actualTokens)
        {
            int count = 0;
            for (int i = 0; i < actualTokens.Length; i++)
            {
                if (actualTokens[i] == tokenizedTokens[i].Value)
                {
                    count++;
                }
            }
            Assert.AreEqual(actualTokens.Length, tokenizedTokens.Count);
            Assert.AreEqual(actualTokens.Length, count);
        }

        /// <summary>
        /// Determines if the error message of an incorrect input is correct.
        /// </summary>
        /// <param name="expectedErrorMsg"></param>
        private void IsErrorMessageCorrect(string incorrectInput, string expectedErrorMsg)
        {
            input = incorrectInput;
            var (tokenlist, msg) = tokenizer.Tokenize();
            Assert.IsTrue(tokenlist is null && msg == expectedErrorMsg);
        }
    }
}