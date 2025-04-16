using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivateVisualizerModelTest
{
    [TestClass]
    public class FunctionEvaluatorTest
    {
        private const double Step = 0.001;

        private static ASTNode Num(double value) => new ASTNode(value.ToString());
        private static ASTNode Var() => new ASTNode("x");

        private static ASTNode Op(string op, ASTNode left, ASTNode right) =>
            new ASTNode(op, left, right);

        private static ASTNode Func(string funcName, ASTNode argument, ASTNode power = null!) =>
            new ASTNode(funcName, argument, power);

        private static void AssertNear(double expected, double actual, double epsilon = 1e-6) =>
            Assert.IsTrue(Math.Abs(expected - actual) < epsilon, $"Expected {expected}, but got {actual}");

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_NullRoot_Throws()
        {
            FunctionEvaluator.Evaluate(null!, 0, Step);
        }

        [TestMethod]
        public void Test_Constant_ReturnsValue()
        {
            var node = Num(5.5);
            var result = FunctionEvaluator.Evaluate(node, 42, Step);
            Assert.AreEqual(5.5, result);
        }

        [TestMethod]
        public void Test_Variable_ReturnsX()
        {
            var node = Var();
            var result = FunctionEvaluator.Evaluate(node, 3.14, Step);
            Assert.AreEqual(3.14, result);
        }

        [TestMethod]
        public void Test_Operator_Addition()
        {
            var node = Op("+", Num(2), Num(3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Test_Operator_Subtraction()
        {
            var node = Op("-", Num(5), Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Test_Operator_Multiplication()
        {
            var node = Op("*", Num(3), Num(4));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void Test_Operator_Division()
        {
            var node = Op("/", Num(10), Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Test_Operator_DivisionByZero()
        {
            var node = Op("/", Num(1), Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        [TestMethod]
        public void Test_Operator_Exponentiation()
        {
            var node = Op("^", Num(2), Num(3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void Test_Operator_Unknown_Throws()
        {
            var node = Op("%", Num(1), Num(2));
            Assert.ThrowsException<Exception>(() => FunctionEvaluator.Evaluate(node, 0, Step));
        }

        [TestMethod]
        public void Test_Function_Sin()
        {
            var node = Func("sin", Num(Math.PI / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        [TestMethod]
        public void Test_Function_Cos()
        {
            var node = Func("cos", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        [TestMethod]
        public void Test_Function_Tan_Discontinuity()
        {
            var node = Func("tg", Num(Math.PI / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        [TestMethod]
        public void Test_Function_Cot_Discontinuity()
        {
            var node = Func("ctg", Num(Math.PI));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        [TestMethod]
        public void Test_Function_Log()
        {
            var node = Func("log", Num(10), Num(100));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(2.0, result);
        }

        [TestMethod]
        public void Test_Function_Ln()
        {
            var node = Func("ln", Num(Math.E));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(1.0, result);
        }

        [TestMethod]
        public void Test_Function_ArcSin()
        {
            var node = Func("arcsin", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Test_Function_ArcCos()
        {
            var node = Func("arccos", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Test_Function_ArcCtg()
        {
            var node = Func("arcctg", Num(1));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(Math.PI / 4, result);
        }

        [TestMethod]
        public void Test_Function_Cth_Discontinuity()
        {
            var node = Func("cth", Num(0));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }

        [TestMethod]
        public void Test_Function_Arcth_InsideDomain()
        {
            var node = Func("arcth", Num(0.5));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result)); // Outside domain (|x| < 1)
        }

        [TestMethod]
        public void Test_Function_Arcth_Valid()
        {
            var node = Func("arcth", Num(2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(0.5 * Math.Log(3.0), result);
        }

        [TestMethod]
        public void Test_Function_Unknown_Throws()
        {
            var node = Func("unknown", Num(1));
            Assert.ThrowsException<Exception>(() => FunctionEvaluator.Evaluate(node, 0, Step));
        }

        [TestMethod]
        public void Test_Exponentiation_NegativeBaseFractionalExponent()
        {
            var node = Op("^", Num(-8), Num(1.0 / 3));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            AssertNear(-2.0, result);
        }

        [TestMethod]
        public void Test_Exponentiation_NegativeBaseEvenRoot()
        {
            var node = Op("^", Num(-8), Num(1.0 / 2));
            var result = FunctionEvaluator.Evaluate(node, 0, Step);
            Assert.IsTrue(double.IsNaN(result));
        }
    }
}
