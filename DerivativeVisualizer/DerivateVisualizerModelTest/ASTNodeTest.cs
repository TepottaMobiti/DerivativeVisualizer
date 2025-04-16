using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivateVisualizerModelTest
{
    [TestClass]
    public class ASTNodeTest
    {
        //TODO: Simplify, többi fv tesztelése
        private ASTNode? function;
        private string? expectedText;

        /// <summary>
        /// Tests if a node's ToBeDifferentiated value is false then the DiffRule is empty.
        /// </summary>
        [TestMethod]
        public void TestDiffRuleOfNotToBeDifferentiatedNode()
        {
            function = new ASTNode("5");
            expectedText = "";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if a number's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestNumberDiffRule()
        {
            function = new ASTNode("5") { ToBeDifferentiated = true };
            expectedText = "a' = 0 (a valós szám)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if a variable's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestVariableDiffRule()
        {
            function = new ASTNode("x") { ToBeDifferentiated = true };
            expectedText = "x' = 1";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if addition's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestAdditionDiffRule()
        {
            function = new ASTNode("+") { ToBeDifferentiated = true };

            expectedText = "(f + g)' = f' + g'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if subtraction's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSubtractionDiffRule()
        {
            function = new ASTNode("-") { ToBeDifferentiated = true };

            expectedText = "(f - g)' = f' - g'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if multiplication's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestMultiplicationDiffRule()
        {
            function = new ASTNode("*",
                           new ASTNode("5"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "(c * f)' = c * f'";
            Assert.AreEqual(expectedText, function.DiffRule);

            function = new ASTNode("*",
                           new ASTNode("x"),
                           new ASTNode("5"))
            { ToBeDifferentiated = true };

            expectedText = "(f * c)' = f' * c";
            Assert.AreEqual(expectedText, function.DiffRule);

            function = new ASTNode("*",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "(f * g)' = f' * g + f * g'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if division's DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestDivisionDiffRule()
        {
            function = new ASTNode("/") { ToBeDifferentiated = true };
            expectedText = "(f / g)' = (f' * g - f * g') / (g ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if e^x DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleExpDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("e"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "(e ^ x)' = e ^ x";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if e^f DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestExpCompositionDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("e"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "(e ^ f)' = e ^ f * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if a^x DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralExp()
        {
            function = new ASTNode("^",
                           new ASTNode("2"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "(a ^ x)' = a ^ x * ln(a) (a > 0)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if the Diffrule is correct when trying to differentiate a^x when a<0.
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralExpErrorDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("-1"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "Ha a <= 0 (a = -1), akkor a^x nem deriválható";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if a^f DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestGeneralExpCompositionDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("2"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "(a ^ f)' = a ^ f * ln(a) * f' (a > 0)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if the Diffrule is correct when trying to differentiate a^f when a<0.
        /// </summary>
        [TestMethod]
        public void TestGeneralExpCompositionErrorDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("-1"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "Ha a <= 0 (a = -1), akkor a^f nem deriválható";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if x^n DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestBasicExponentiationDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2"))
            { ToBeDifferentiated = true };

            expectedText = "(x ^ n)' =  n * x ^ (n - 1)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if f^n DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestFunctionBaseExponentiationDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("2"))
            { ToBeDifferentiated = true };

            expectedText = "(f ^ n)' = n * f ^ (n - 1) * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if f^g DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestFunctionalExponentiationDiffRule()
        {
            function = new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "(f ^ g)' = (f ^ g)*(f' * g / f + g' * ln(f)) (f > 0, nem ellenőrzött)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if log_e'(x) = 1/x.
        /// </summary>
        [TestMethod]
        public void TestLogBaseEXDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("e"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "log'(e,x) = 1 / x";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if log_e'(f) = 1/f.
        /// </summary>
        [TestMethod]
        public void TestLogBaseEDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("e"),
                           new ASTNode("sin",
                               new ASTNode("x")))
            { ToBeDifferentiated = true };

            expectedText = "log'(e,f) = f' / f";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if the DiffRule is correct when a<=0 in log(a,x)
        /// </summary>
        [TestMethod]
        public void TestNotPositiveLogBaseDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("-1"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "A logaritmus alapja egy pozitív szám, kivéve 1.";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if the DiffRule is correct when a=1 in log(a,x)
        /// </summary>
        [TestMethod]
        public void TestOneLogBaseDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("1"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "A logaritmus alapja egy pozitív szám, kivéve 1.";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if the DiffRule is correct when the base of the log is not a number.
        /// </summary>
        [TestMethod]
        public void TestNaNLogBaseDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("x"),
                           new ASTNode("2"))
            { ToBeDifferentiated = true };

            expectedText = "A logaritmus alapja egy pozitív szám, kivéve 1.";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if log_a(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralLogDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("2"),
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "log'(a,x) = 1 / (x * ln(a))";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if log_a(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestGeneralLogCompositionDiffRule()
        {
            function = new ASTNode("log",
                           new ASTNode("2"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "log'(a,f) = f' / (f * ln(a))";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ln(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleLnDiffRule()
        {
            function = new ASTNode("ln",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "ln'(x) = 1 / x";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ln(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestLnCompositionDiffRule()
        {
            function = new ASTNode("ln",
                           new ASTNode("sin",
                               new ASTNode("x")))
            { ToBeDifferentiated = true };

            expectedText = "ln'(f) = f' / f";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if sin(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleSinDiffRule()
        {
            function = new ASTNode("sin",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "sin'(x) = cos(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if sin(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSinCompositionDiffRule()
        {
            function = new ASTNode("sin",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "sin'(f) = cos(f) * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if cos(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleCosDiffRule()
        {
            function = new ASTNode("cos",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "cos'(x) = -1 * sin(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if cos(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestCosCompositionDiffRule()
        {
            function = new ASTNode("cos",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "cos'(f) = -1 * sin(f) * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if tg(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleTgDiffRule()
        {
            function = new ASTNode("tg",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "tg'(x) = 1 / cos^2(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if tg(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestTgCompositionDiffRule()
        {
            function = new ASTNode("tg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "tg'(f) = f' / cos^2(f)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ctg(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleCtgDiffRule()
        {
            function = new ASTNode("ctg",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "ctg'(x) = -1 / sin^2(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ctg(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestCtgCompositionDiffRule()
        {
            function = new ASTNode("ctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "ctg'(f) = (-1 * f') / sin^2(f)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcsin(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArcsinDiffRule()
        {
            function = new ASTNode("arcsin",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arcsin'(x) = 1 / (1 - x ^ 2) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcsin(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArcsinCompositionDiffRule()
        {
            function = new ASTNode("arcsin",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arcsin'(f) = f' / (1 - f ^ 2) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arccos(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArccosDiffRule()
        {
            function = new ASTNode("arccos",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arccos'(x) = -1 / (1 - x ^ 2) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arccos(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArccosCompositionDiffRule()
        {
            function = new ASTNode("arccos",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arccos'(f) = (-1 * f') / (1 - f ^ 2) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arctg(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArctgDiffRule()
        {
            function = new ASTNode("arctg",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arctg'(x) = 1 / (1 + x ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arctg(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArctgCompositionDiffRule()
        {
            function = new ASTNode("arctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arctg'(f) = f' / (1 + f ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcctg(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArcctgDiffRule()
        {
            function = new ASTNode("arcctg",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arcctg'(x) = -1 / (1 + x ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcctg(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArcctgCompositionDiffRule()
        {
            function = new ASTNode("arcctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arcctg'(f) = (-1 * f') / (1 + f ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if sh(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleShDiffRule()
        {
            function = new ASTNode("sh",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "sh'(x) = ch(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if sh(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestShCompositionDiffRule()
        {
            function = new ASTNode("sh",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "sh'(f) = ch(f) * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ch(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleChDiffRule()
        {
            function = new ASTNode("ch",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "ch'(x) = sh(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if ch(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestChCompositionDiffRule()
        {
            function = new ASTNode("ch",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "ch'(f) = sh(f) * f'";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if th(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleThDiffRule()
        {
            function = new ASTNode("th",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "th'(x) = 1 / ch^2(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if th(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestThCompositionDiffRule()
        {
            function = new ASTNode("th",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "th'(f) = f' / ch^2(f)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if cth(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleCthDiffRule()
        {
            function = new ASTNode("cth",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "cth'(x) = -1 / sh^2(x)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if cth(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestCthCompositionDiffRule()
        {
            function = new ASTNode("cth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "cth'(f) = (-1 * f') / sh^2(f)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arsh(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArshDiffRule()
        {
            function = new ASTNode("arsh",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arsh'(x) = 1 / (x ^ 2 + 1) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arsh(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArshCompositionDiffRule()
        {
            function = new ASTNode("arsh",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arsh'(f) = f' / (f ^ 2 + 1) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arch(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArchDiffRule()
        {
            function = new ASTNode("arch",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arch'(x) = 1 / (x ^ 2 - 1) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arch(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArchCompositionDiffRule()
        {
            function = new ASTNode("arch",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arch'(f) = f' / (f ^ 2 - 1) ^ (1 / 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arth(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArthDiffRule()
        {
            function = new ASTNode("arth",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arth'(x) = 1 / (1 - x ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arth(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArthCompositionDiffRule()
        {
            function = new ASTNode("arth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arth'(f) = f' / (1 - f ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcth(x) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestSimpleArcthDiffRule()
        {
            function = new ASTNode("arcth",
                           new ASTNode("x"))
            { ToBeDifferentiated = true };

            expectedText = "arcth'(x) = 1 / (1 - x ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }

        /// <summary>
        /// Tests if arcth(f) DiffRule is correct.
        /// </summary>
        [TestMethod]
        public void TestArcthCompositionDiffRule()
        {
            function = new ASTNode("arcth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")))
            { ToBeDifferentiated = true };

            expectedText = "arcth'(f) = f' / (1 - f ^ 2)";
            Assert.AreEqual(expectedText, function.DiffRule);
        }
    }
}
