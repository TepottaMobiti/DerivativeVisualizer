using DerivativeVisualizerModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace DerivateVisualizerModelTest
{
    // Szakdoga szöveg: Minden alapfüggvényt leteszteltem kompozíció nélkül és az f kör x^2 kompozícióval.

    [TestClass]
    public class DifferentiatorTest
    {
        private ASTNode function = null!;
        private ASTNode expectedDerivative = null!;
        private ASTNode actualDerivative = null!;
        private Differentiator Diff
        {
            get
            {
                return new Differentiator(function);
            }
        }

        /// <summary>
        /// Tests if the derivative of the function is the function if the locator is invalid.
        /// </summary>
        [TestMethod]
        public void TestInvalidLocator()
        {
            function = new ASTNode("5");
            actualDerivative = Diff.Differentiate(10);
            Assert.IsTrue(ASTNode.AreTreesEqual(function, actualDerivative));
        }

        /// <summary>
        /// Tests if the function and the derivative will be the same if the locator is valid,
        /// but it is of a node that's ToBeDifferentiated value is false.
        /// </summary>
        [TestMethod]
        public void TestLocatorNotToBeDifferentiated()
        {
            function = new ASTNode("+",
                           new ASTNode("x"),
                           new ASTNode("2"));
            actualDerivative = Diff.Differentiate(function.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(function, actualDerivative));
        }

        /// <summary>
        /// Tests if a number's derivative is 0.
        /// </summary>
        [TestMethod]
        public void TestNumber()
        {
            function = new ASTNode("5");
            expectedDerivative = new ASTNode("0");
            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if x'=1.
        /// </summary>
        [TestMethod]
        public void TestVariable()
        {
            function = new ASTNode("x");
            expectedDerivative = new ASTNode("1");
            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if (f+g)' = f' + g'.
        /// </summary>
        [TestMethod]
        public void TestAddition()
        {
            function = new ASTNode("+",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(function,actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("+",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("x"));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("+",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("1"));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f-g)' = f' + g'.
        /// </summary>
        [TestMethod]
        public void TestSubtraction()
        {
            function = new ASTNode("-",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(function, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("-",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("x"));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("-",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("1"));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (c*f)' = (f*c)' = c*f'.
        /// </summary>
        [TestMethod]
        public void TestMultiplicationWithNumber()
        {
            function = new ASTNode("*",
                           new ASTNode("5"),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(function,actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("5"),
                                     new ASTNode("1"));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));

            function = new ASTNode("*",
                           new ASTNode("x"),
                           new ASTNode("5"));

            d = new Differentiator(function);

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(function, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("1"),
                                     new ASTNode("5"));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f*g)' = f'*g+f*g'.
        /// </summary>
        [TestMethod]
        public void TestMultiplication()
        {
            function = new ASTNode("*",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("+",
                                     new ASTNode("*",
                                         new ASTNode("sin",
                                             new ASTNode("x")),
                                         new ASTNode("x")),
                                     new ASTNode("*",
                                         new ASTNode("sin",
                                             new ASTNode("x")),
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Left.Left.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Right.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("+",
                                     new ASTNode("*",
                                         new ASTNode("cos",
                                             new ASTNode("x")),
                                         new ASTNode("x")),
                                     new ASTNode("*",
                                         new ASTNode("sin",
                                             new ASTNode("x")),
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("+",
                                     new ASTNode("*",
                                         new ASTNode("cos",
                                             new ASTNode("x")),
                                         new ASTNode("x")),
                                     new ASTNode("*",
                                         new ASTNode("sin",
                                             new ASTNode("x")),
                                         new ASTNode("1")));

            actualDerivative = d.Differentiate(actualDerivative.Right.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f/g)' = (f'g-f*g')/g^2
        /// </summary>
        [TestMethod]
        public void TestDivision()
        {
            function = new ASTNode("/",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("-",
                                         new ASTNode("*",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("x")),
                                         new ASTNode("*",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("x"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Left.Left.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Left.Right.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("-",
                                         new ASTNode("*",
                                             new ASTNode("cos",
                                                 new ASTNode("x")),
                                             new ASTNode("x")),
                                         new ASTNode("*",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("x"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Left.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Right.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("-",
                                         new ASTNode("*",
                                             new ASTNode("cos",
                                                 new ASTNode("x")),
                                             new ASTNode("x")),
                                         new ASTNode("*",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Right.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (e^x)' = e^x.
        /// </summary>
        [TestMethod]
        public void TestSimpleExp()
        {
            function = new ASTNode("^",
                           new ASTNode("e"),
                           new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, function));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (e^f)' = e^f*f'.
        /// </summary>
        [TestMethod]
        public void TestExpComposition()
        {
            function = new ASTNode("^",
                           new ASTNode("e"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("e"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative,expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("e"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (a^x)' = a^x*ln(a) (a>0).
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralExp()
        {
            function = new ASTNode("^",
                           new ASTNode("2"),
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("2"),
                                         new ASTNode("x")),
                                     new ASTNode("ln",
                                         new ASTNode("2")));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if the right exception is thrown trying to differentiate a^x when a<0.
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralExpError()
        {
            function = new ASTNode("^",
                           new ASTNode("-1"),
                           new ASTNode("x"));

            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    actualDerivative = Diff.Differentiate(function.Locator);
                }
                catch (Exception e)
                {
                    if (e.Message == "Ha a <= 0 (a = -1), akkor a^x nem deriválható")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if (a^f)' = a^f*ln(a)*f' (a>0).
        /// </summary>
        [TestMethod]
        public void TestGeneralExpComposition()
        {
            function = new ASTNode("^",
                           new ASTNode("2"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("^",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("ln",
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("^",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("ln",
                                             new ASTNode("2"))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if the right exception is thrown trying to differentiate a^x when a<0.
        /// </summary>
        [TestMethod]
        public void TestGeneralExpCompositionError()
        {
            function = new ASTNode("^",
                           new ASTNode("-1"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    actualDerivative = Diff.Differentiate(function.Locator);
                }
                catch (Exception e)
                {
                    if (e.Message == "Ha a <= 0 (a = -1), akkor a^f nem deriválható")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if (x^e)' = e*x^(e-1)
        /// </summary>
        [TestMethod]
        public void TestXToThePowerOfE()
        {
            function = new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("e"));

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("e"),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode((Math.E-1).ToString(CultureInfo.InvariantCulture))));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative,expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (x^n)' = n*x^(n-1)
        /// </summary>
        [TestMethod]
        public void TestBasicExponentiation()
        {
            function = new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2"));

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("2"),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("1")));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f^e)' = e*f^(e-1)*f'.
        /// </summary>
        [TestMethod]
        public void TestFunctionToThePowerOfE()
        {
            function = new ASTNode("^",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("e"));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("e"),
                                         new ASTNode("^",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode((Math.E - 1).ToString(CultureInfo.InvariantCulture)))),
                                     new ASTNode("sin",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("e"),
                                         new ASTNode("^",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode((Math.E - 1).ToString(CultureInfo.InvariantCulture)))),
                                     new ASTNode("cos",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f^n)' = n*f^(n-1).
        /// </summary>
        [TestMethod]
        public void TestFunctionBaseExponentiation()
        {
            function = new ASTNode("^",
                           new ASTNode("sin",
                               new ASTNode("x")),
                           new ASTNode("2"));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("1"))),
                                     new ASTNode("sin",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("sin",
                                                 new ASTNode("x")),
                                             new ASTNode("1"))),
                                     new ASTNode("cos",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if (f^g)' = (f^g)*(f'*g/f+g'*ln(f)) (f>0, not checked).
        /// </summary>
        [TestMethod]
        public void TestFunctionalExponentiation()
        {
            function = new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("x"));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("x")),
                                     new ASTNode("+",
                                         new ASTNode("*",
                                             new ASTNode("x"),
                                             new ASTNode("/",
                                                 new ASTNode("x"),
                                                 new ASTNode("x"))),
                                         new ASTNode("*",
                                             new ASTNode("x"),
                                             new ASTNode("ln",
                                                 new ASTNode("x")))));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Right.Left.Left.ToBeDifferentiated);
            Assert.IsTrue(actualDerivative.Right.Right.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("x")),
                                     new ASTNode("+",
                                         new ASTNode("*",
                                             new ASTNode("1"),
                                             new ASTNode("/",
                                                 new ASTNode("x"),
                                                 new ASTNode("x"))),
                                         new ASTNode("*",
                                             new ASTNode("x"),
                                             new ASTNode("ln",
                                                 new ASTNode("x")))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Left.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Right.Right.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("x")),
                                     new ASTNode("+",
                                         new ASTNode("*",
                                             new ASTNode("1"),
                                             new ASTNode("/",
                                                 new ASTNode("x"),
                                                 new ASTNode("x"))),
                                         new ASTNode("*",
                                             new ASTNode("1"),
                                             new ASTNode("ln",
                                                 new ASTNode("x")))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Right.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if log_e'(x) = 1/x.
        /// </summary>
        [TestMethod]
        public void TestLogBaseEX()
        {
            function = new ASTNode("log",
                           new ASTNode("e"),
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("1"),
                                     new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if log_e'(f) = 1/f.
        /// </summary>
        [TestMethod]
        public void TestLogBaseE()
        {
            function = new ASTNode("log",
                           new ASTNode("e"),
                           new ASTNode("sin",
                               new ASTNode("x")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("sin",
                                         new ASTNode("x")),
                                     new ASTNode("sin",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("sin",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if the correct exception is thrown when the base of the log is not positive.
        /// </summary>
        [TestMethod]
        public void TestNotPositiveLogBase()
        {
            function = new ASTNode("log",
                           new ASTNode("-1"),
                           new ASTNode("x"));

            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    actualDerivative = Diff.Differentiate(function.Locator);
                }
                catch (Exception e)
                {
                    if (e.Message == "A logaritmus alapja egy pozitív szám, kivéve 1.")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if the correct exception is thrown when the base of the log is 1.
        /// </summary>
        [TestMethod]
        public void TestOneLogBase()
        {
            function = new ASTNode("log",
                           new ASTNode("1"),
                           new ASTNode("x"));

            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    actualDerivative = Diff.Differentiate(function.Locator);
                }
                catch (Exception e)
                {
                    if (e.Message == "A logaritmus alapja egy pozitív szám, kivéve 1.")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if the correct exception is thrown when the base of the log is not a number.
        /// </summary>
        [TestMethod]
        public void TestNaNLogBase()
        {
            function = new ASTNode("log",
                           new ASTNode("x"),
                           new ASTNode("2"));

            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    actualDerivative = Diff.Differentiate(function.Locator);
                }
                catch (Exception e)
                {
                    if (e.Message == "A logaritmus alapja egy pozitív szám, kivéve 1.")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if log_a'(x) = 1/(x*ln(a)).
        /// </summary>
        [TestMethod]
        public void TestSimpleGeneralLog()
        {
            function = new ASTNode("log",
                           new ASTNode("2"),
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                       new ASTNode("1"),
                                       new ASTNode("*",
                                           new ASTNode("x"),
                                           new ASTNode("ln",
                                               new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if log_a'(x) = 1/(x*ln(a)).
        /// </summary>
        [TestMethod]
        public void TestGeneralLogComposition()
        {
            function = new ASTNode("log",
                           new ASTNode("2"),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("*",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2")),
                                         new ASTNode("ln",
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("*",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2")),
                                         new ASTNode("ln",
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if ln'(x) = 1/x.
        /// </summary>
        [TestMethod]
        public void TestSimpleLn()
        {
            function = new ASTNode("ln",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("1"),
                                     new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests is ln'(f) = f'/f
        /// </summary>
        [TestMethod]
        public void TestLnComposition()
        {
            function = new ASTNode("ln",
                           new ASTNode("sin",
                               new ASTNode("x")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("sin",
                                         new ASTNode("x")),
                                     new ASTNode("sin",
                                         new ASTNode("x")));
            
            actualDerivative = d.Differentiate(function.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("cos",
                                         new ASTNode("x")),
                                     new ASTNode("sin",
                                         new ASTNode("x")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);

            Assert.IsTrue(ASTNode.AreTreesEqual(actualDerivative, expectedDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if sin'(x) = cos(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleSin()
        {
            function = new ASTNode("sin",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("cos",
                                     new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Left.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if sin'(f) = cos(f)*f'.
        /// </summary>
        [TestMethod]
        public void TestSinComposition()
        {
            function = new ASTNode("sin",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("cos",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("cos",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if cos'(x) = -1*sin(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleCos()
        {
            function = new ASTNode("cos",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("*",
                                      new ASTNode("-1"),
                                      new ASTNode("sin", new ASTNode("x")));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if cos'(f) = -1*sin(f)*f'.
        /// </summary>
        [TestMethod]
        public void TestCosComposition()
        {
            function = new ASTNode("cos",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("sin",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("sin",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if tg'(x) = 1/cos^2(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleTg()
        {
            function = new ASTNode("tg",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("cos",
                                           new ASTNode("x")),
                                       new ASTNode("2")));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }


        /// <summary>
        /// Tests if tg'(f) = f'/cos^2(f).
        /// </summary>
        [TestMethod]
        public void TestTgComposition()
        {
            function = new ASTNode("tg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("^",
                                         new ASTNode("cos",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("cos",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if ctg'(x) = -1/sin^2(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleCtg()
        {
            function = new ASTNode("ctg",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("-1"),
                                     new ASTNode("^",
                                         new ASTNode("sin",
                                             new ASTNode("x")),
                                         new ASTNode("2")));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if ctg'(f) = (-1*f')/sin^2(f).
        /// </summary>
        [TestMethod]
        public void TestCtgComposition()
        {
            function = new ASTNode("ctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("sin",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("*",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("1")))),
                                     new ASTNode("^",
                                         new ASTNode("sin",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arcsin'(x) = 1/(1-x^2)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArcsin()
        {
            function = new ASTNode("arcsin",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("1"),
                                           new ASTNode("^",
                                               new ASTNode("x"),
                                               new ASTNode("2"))),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arcsin'(f) = f'/(1-f^2)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestArcsinComposition()
        {
            function = new ASTNode("arcsin",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("1"),
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2"))),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("1"),
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2"))),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arccos'(x) = -1/(1-x^2)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArccos()
        {
            function = new ASTNode("arccos",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("1"),
                                           new ASTNode("^",
                                               new ASTNode("x"),
                                               new ASTNode("2"))),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arccos'(f) = (-1*f')/(1-f^2)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestArccosComposition()
        {
            function = new ASTNode("arccos",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("1"),
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2")),
                                                 new ASTNode("2"))),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("*",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("1")))),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("1"),
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2")),
                                                 new ASTNode("2"))),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arctg'(x) = 1'/(1+x^2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArctg()
        {
            function = new ASTNode("arctg",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("+",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arctg'(f) = f'/(1+f^2).
        /// </summary>
        [TestMethod]
        public void TestArctgComposition()
        {
            function = new ASTNode("arctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("+",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("+",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arcctg'(x) = -1'/(1+x^2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArcctg()
        {
            function = new ASTNode("arcctg",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("-1"),
                                   new ASTNode("+",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arcctg'(f) = (-1*f')/(1+f^2).
        /// </summary>
        [TestMethod]
        public void TestArcctgComposition()
        {
            function = new ASTNode("arcctg",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("+",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("*",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("1")))),
                                     new ASTNode("+",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if sh'(x) = ch(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleSh()
        {
            function = new ASTNode("sh",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("ch",
                                     new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Left.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if sh'(f) = ch(f)*f'.
        /// </summary>
        [TestMethod]
        public void TestShComposition()
        {
            function = new ASTNode("sh",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("ch",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("ch",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if ch'(x) = sh(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleCh()
        {
            function = new ASTNode("ch",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("sh",
                                     new ASTNode("x"));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Left.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if ch'(f) = sh(f)*f'.
        /// </summary>
        [TestMethod]
        public void TestChComposition()
        {
            function = new ASTNode("ch",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("sh",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("*",
                                     new ASTNode("sh",
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))));

            actualDerivative = d.Differentiate(actualDerivative.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if th'(x) = 1/ch^2(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleTh()
        {
            function = new ASTNode("th",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("ch",
                                           new ASTNode("x")),
                                       new ASTNode("2")));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }


        /// <summary>
        /// Tests if th'(f) = f'/ch^2(f).
        /// </summary>
        [TestMethod]
        public void TestThComposition()
        {
            function = new ASTNode("th",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("^",
                                         new ASTNode("ch",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("ch",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if cth'(x) = -1/sh^2(x).
        /// </summary>
        [TestMethod]
        public void TestSimpleCth()
        {
            function = new ASTNode("cth",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("-1"),
                                     new ASTNode("^",
                                         new ASTNode("sh",
                                             new ASTNode("x")),
                                         new ASTNode("2")));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if cth'(f) = (-1*f')/sh^2(f).
        /// </summary>
        [TestMethod]
        public void TestCthComposition()
        {
            function = new ASTNode("cth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("2"))),
                                     new ASTNode("^",
                                         new ASTNode("sh",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.Right.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("-1"),
                                         new ASTNode("*",
                                             new ASTNode("2"),
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("1")))),
                                     new ASTNode("^",
                                         new ASTNode("sh",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2"))),
                                         new ASTNode("2")));

            actualDerivative = d.Differentiate(actualDerivative.Left.Right.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arsh'(x) = 1/(x^2+1)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArsh()
        {
            function = new ASTNode("arsh",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("+",
                                           new ASTNode("^",
                                               new ASTNode("x"),
                                               new ASTNode("2")),
                                           new ASTNode("1")),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arsh'(f) = f'/(f^2+1)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestArshComposition()
        {
            function = new ASTNode("arsh",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("^",
                                         new ASTNode("+",
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2")),
                                             new ASTNode("1")),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("+",
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2")),
                                             new ASTNode("1")),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arch'(x) = 1/(x^2-1)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArch()
        {
            function = new ASTNode("arch",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("^",
                                       new ASTNode("-",
                                           new ASTNode("^",
                                               new ASTNode("x"),
                                               new ASTNode("2")),
                                           new ASTNode("1")),
                                       new ASTNode("/",
                                           new ASTNode("1"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arch'(f) = f'/(f^2-1)^(1/2).
        /// </summary>
        [TestMethod]
        public void TestArchComposition()
        {
            function = new ASTNode("arch",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2")),
                                             new ASTNode("1")),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("^",
                                         new ASTNode("-",
                                             new ASTNode("^",
                                                 new ASTNode("^",
                                                     new ASTNode("x"),
                                                     new ASTNode("2")),
                                                 new ASTNode("2")),
                                             new ASTNode("1")),
                                         new ASTNode("/",
                                             new ASTNode("1"),
                                             new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arth'(x) = 1'/(1-x^2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArth()
        {
            function = new ASTNode("arth",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arth'(f) = f'/(1-f^2).
        /// </summary>
        [TestMethod]
        public void TestArthComposition()
        {
            function = new ASTNode("arth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("-",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("-",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }

        /// <summary>
        /// Tests if arcth'(x) = 1'/(1-x^2).
        /// </summary>
        [TestMethod]
        public void TestSimpleArcth()
        {
            function = new ASTNode("arcth",
                           new ASTNode("x"));

            expectedDerivative = new ASTNode("/",
                                   new ASTNode("1"),
                                   new ASTNode("-",
                                       new ASTNode("1"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("2"))));

            actualDerivative = Diff.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(actualDerivative.Right.ToBeDifferentiated);
        }

        /// <summary>
        /// Tests if arcth'(f) = f'/(1-f^2).
        /// </summary>
        [TestMethod]
        public void TestArcthComposition()
        {
            function = new ASTNode("arcth",
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));

            Differentiator d = new Differentiator(function);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("^",
                                         new ASTNode("x"),
                                         new ASTNode("2")),
                                     new ASTNode("-",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(function.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsTrue(actualDerivative.Left.ToBeDifferentiated);

            expectedDerivative = new ASTNode("/",
                                     new ASTNode("*",
                                         new ASTNode("2"),
                                         new ASTNode("^",
                                             new ASTNode("x"),
                                             new ASTNode("1"))),
                                     new ASTNode("-",
                                         new ASTNode("1"),
                                         new ASTNode("^",
                                             new ASTNode("^",
                                                 new ASTNode("x"),
                                                 new ASTNode("2")),
                                           new ASTNode("2"))));

            actualDerivative = d.Differentiate(actualDerivative.Left.Locator);
            Assert.IsTrue(ASTNode.AreTreesEqual(expectedDerivative, actualDerivative));
            Assert.IsFalse(ASTNode.HasDifferentiationNode(actualDerivative));
        }
    }
}