using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivateVisualizerModelTest
{
    // Entire class is AI generated.
    [TestClass]
    public class FunctionEvaluatorTest
    {
        private const double Step = 0.001;

        /// <summary>
        /// Creates a numeric ASTNode with the specified value.
        /// </summary>
        private static ASTNode Num(double value) => new ASTNode(value.ToString());

        /// <summary>
        /// Creates a variable ASTNode representing 'x'.
        /// </summary>
        private static ASTNode Var() => new ASTNode("x");

        /// <summary>
        /// Creates a binary operator ASTNode with the given operator and operands.
        /// </summary>
        private static ASTNode Op(string op, ASTNode left, ASTNode right) =>
            new ASTNode(op, left, right);

        /// <summary>
        /// Creates a function ASTNode with the given name, argument, and optional power.
        /// </summary>
        private static ASTNode Func(string funcName, ASTNode argument, ASTNode power = null!) =>
            new ASTNode(funcName, argument, power);

        /// <summary>
        /// Asserts that two doubles are approximately equal within epsilon.
        /// </summary>
        private static void AssertNear(double expected, double actual, double epsilon = 1e-6) =>
            Assert.IsTrue(Math.Abs(expected - actual) < epsilon, $"Expected {expected}, but got {actual}");

        /// <summary>
        /// Verifies that evaluating a null root throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestNullRootThrows()
        {
            FunctionEvaluator.Evaluate(null!, 0, Step);
        }

        /// <summary>
        /// Verifies that a constant node evaluates to its value.
        /// </summary>
        [TestMethod]
        public void TestConstantReturnsValue()
        {
            var node = Num(5.5);
            var result = FunctionEvaluator.Evaluate(node, 42, Step);
            Assert.AreEqual(5.5, result);
        }

        /// <summary>
        /// Verifies that a variable node returns the input value of x.
        /// </summary>
        [TestMethod]
        public void TestVariableReturnsX()
        {
            var node = Var();
            var result = FunctionEvaluator.Evaluate(node, 3.14, Step);
            Assert.AreEqual(3.14, result);
        }

        /// <summary>
        /// Tests addition of two constants.
        /// </summary>
        [TestMethod]
        public void TestOperatorAddition()
        {
            var node = Op("+", Num(2), Num(3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Tests subtraction of two constants.
        /// </summary>
        [TestMethod]
        public void TestOperatorSubtraction()
        {
            var node = Op("-", Num(5), Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(3, result);
        }

        /// <summary>
        /// Tests multiplication of two constants.
        /// </summary>
        [TestMethod]
        public void TestOperatorMultiplication()
        {
            var node = Op("*", Num(3), Num(4));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(12, result);
        }

        /// <summary>
        /// Tests division of two constants.
        /// </summary>
        [TestMethod]
        public void TestOperatorDivision()
        {
            var node = Op("/", Num(10), Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Verifies division by zero returns NaN.
        /// </summary>
        [TestMethod]
        public void TestOperatorDivisionByZero()
        {
            var node = Op("/", Num(1), Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        /// <summary>
        /// Tests exponentiation of two constants.
        /// </summary>
        [TestMethod]
        public void TestOperatorExponentiation()
        {
            var node = Op("^", Num(2), Num(3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(8, result);
        }

        /// <summary>
        /// Verifies that unknown operators throw an exception.
        /// </summary>
        [TestMethod]
        public void TestOperatorUnknownThrows()
        {
            var node = Op("%", Num(1), Num(2));
            Assert.ThrowsException<Exception>(() => FunctionEvaluator.Evaluate(node, 0, Step));
        }

        /// <summary>
        /// Tests evaluation of sin(π/2).
        /// </summary>
        [TestMethod]
        public void TestFunctionSin()
        {
            var node = Func("sin", Num(Math.PI / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        /// <summary>
        /// Tests evaluation of cos(0).
        /// </summary>
        [TestMethod]
        public void TestFunctionCos()
        {
            var node = Func("cos", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        /// <summary>
        /// Tests evaluation of tg(π/4).
        /// </summary>
        [TestMethod]
        public void TestFunctionTanValid()
        {
            var node = Func("tg", Num(Math.PI / 4));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        /// <summary>
        /// Verifies tg(π/2) results in NaN due to discontinuity.
        /// </summary>
        [TestMethod]
        public void TestFunctionTanDiscontinuity()
        {
            var node = Func("tg", Num(Math.PI / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        /// <summary>
        /// Tests evaluation of ctg(π/4).
        /// </summary>
        [TestMethod]
        public void TestFunctionCotValid()
        {
            var node = Func("ctg", Num(Math.PI / 4));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        /// <summary>
        /// Verifies ctg(π) results in NaN due to discontinuity.
        /// </summary>
        [TestMethod]
        public void TestFunctionCotDiscontinuity()
        {
            var node = Func("ctg", Num(Math.PI));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        /// <summary>
        /// Tests evaluation of log base 10 of 100.
        /// </summary>
        [TestMethod]
        public void TestFunctionLog()
        {
            var node = Func("log", Num(10), Num(100));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(2.0, result);
        }

        /// <summary>
        /// Tests evaluation of natural logarithm of e.
        /// </summary>
        [TestMethod]
        public void TestFunctionLn()
        {
            var node = Func("ln", Num(Math.E));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        /// <summary>
        /// Tests evaluation of arcsin(0).
        /// </summary>
        [TestMethod]
        public void TestFunctionArcSin()
        {
            var node = Func("arcsin", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// Tests evaluation of arccos(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionArcCos()
        {
            var node = Func("arccos", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// Tests evaluation of arctg(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionArcTg()
        {
            var node = Func("arctg", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Atan(1), result);
        }

        /// <summary>
        /// Tests evaluation of arcctg(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionArcCtg()
        {
            var node = Func("arcctg", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.PI / 4, result);
        }

        /// <summary>
        /// Tests evaluation of sh(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionSh()
        {
            var node = Func("sh", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Sinh(1), result);
        }

        /// <summary>
        /// Tests evaluation of ch(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionCh()
        {
            var node = Func("ch", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Cosh(1), result);
        }

        /// <summary>
        /// Tests evaluation of th(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionTh()
        {
            var node = Func("th", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Tanh(1), result);
        }

        /// <summary>
        /// Verifies cth(0) returns NaN due to discontinuity.
        /// </summary>
        [TestMethod]
        public void TestFunctionCthDiscontinuity()
        {
            var node = Func("cth", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        /// <summary>
        /// Tests evaluation of arsh(1).
        /// </summary>
        [TestMethod]
        public void TestFunctionArsh()
        {
            var node = Func("arsh", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Asinh(1), result);
        }

        /// <summary>
        /// Tests evaluation of arch(2).
        /// </summary>
        [TestMethod]
        public void TestFunctionArch()
        {
            var node = Func("arch", Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Acosh(2), result);
        }

        /// <summary>
        /// Tests evaluation of arth(0.5).
        /// </summary>
        [TestMethod]
        public void TestFunctionArth()
        {
            var node = Func("arth", Num(0.5));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.Atanh(0.5), result);
        }

        /// <summary>
        /// Verifies arcth(0.5) returns NaN as it's outside domain.
        /// </summary>
        [TestMethod]
        public void TestFunctionArcthInsideDomain()
        {
            var node = Func("arcth", Num(0.5));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        /// <summary>
        /// Tests valid arcth(2) evaluation.
        /// </summary>
        [TestMethod]
        public void TestFunctionArcthValid()
        {
            var node = Func("arcth", Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(0.5 * Math.Log(3.0), result);
        }

        /// <summary>
        /// Verifies that unknown function names throw an exception.
        /// </summary>
        [TestMethod]
        public void TestFunctionUnknownThrows()
        {
            var node = Func("unknown", Num(1));
            Assert.ThrowsException<Exception>(() => FunctionEvaluator.Evaluate(node, 0, Step));
        }

        /// <summary>
        /// Tests evaluation of negative base with fractional exponent (odd root).
        /// </summary>
        [TestMethod]
        public void TestExponentiationNegativeBaseFractionalExponent()
        {
            var node = Op("^", Num(-8), Num(1.0 / 3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(-2.0, result);
        }

        /// <summary>
        /// Verifies even root of negative number returns NaN.
        /// </summary>
        [TestMethod]
        public void TestExponentiationNegativeBaseEvenRoot()
        {
            var node = Op("^", Num(-8), Num(1.0 / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }
    }
}
