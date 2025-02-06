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
                return tokenizer.Tokenize() ?? new List<Token>();
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
            input = "sin(x) + cos(3*x) - tan(4*x) * cot(2*x)";
            actualTokens = ["sin","(","x",")","+","cos","(","3","*","x",")","-","tan","(","4","*","x",")","*","cot","(","2","*","x",")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if inverse trigonometric functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestTrigonometricInverseInput()
        {
            input = "arcsin(x) + arccos(3-x) - arctan(4^x) * arccot(2+x)";
            actualTokens = ["arcsin", "(", "x", ")", "+", "arccos", "(", "3", "-", "x", ")", "-", "arctan", "(", "4", "^", "x", ")", "*", "arccot", "(", "2", "+", "x", ")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if hyperbolic functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestHyperbolicInput()
        {
            input = "sinh(x) + cosh(3/x) - tanh(4^x) / coth(2*x)";
            actualTokens = ["sinh", "(", "x", ")", "+", "cosh", "(", "3", "/", "x", ")", "-", "tanh", "(", "4", "^", "x", ")", "/", "coth", "(", "2", "*", "x", ")"];
            IsTokenizationCorrect(input, actualTokens);
        }

        /// <summary>
        /// Tests if inverse hyperbolic functions are tokenized correctly.
        /// </summary>
        [TestMethod]
        public void TestHyperbolicInverseInput()
        {
            input = "arsinh(x) * arcosh(3^x) - artanh(x^2) * arcoth(2*x)";
            actualTokens = ["arsinh", "(", "x", ")", "*", "arcosh", "(", "3", "^", "x", ")", "-", "artanh", "(", "x", "^", "2", ")", "*", "arcoth", "(", "2", "*", "x", ")"];
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
        /// Tests if an exception is thrown if there are multiple decimal points in a number.
        /// </summary>
        [TestMethod]
        public void TestMultipleDecimalPointsInput()
        {
            input = "3.5.15";
            Assert.ThrowsException<Exception>(() => tokenizer.Tokenize());
        }

        /// <summary>
        /// Tests if an exception is thrown if a function is unknown.
        /// </summary>
        [TestMethod]
        public void TestUnknownFunctionInput()
        {
            input = "sec(x)";
            Assert.ThrowsException<Exception>(() => tokenizer.Tokenize());
        }

        /// <summary>
        /// Tests if an exception is thrown if the variable of an input is not x.
        /// </summary>
        [TestMethod]
        public void TestInvalidVariableInput()
        {
            input = "y^2+2y+1";
            Assert.ThrowsException<Exception>(() => tokenizer.Tokenize());
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
    }
}