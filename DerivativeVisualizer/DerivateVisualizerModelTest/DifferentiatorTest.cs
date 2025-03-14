using DerivativeVisualizerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivateVisualizerModelTest
{
    [TestClass]
    public class DifferentiatorTest
    {
        // TODO: Exception tesztelésnél tesztelni mindenhol, hogy jó exception jön-e, ld. 220. sor
        // TODO: Egyszerűsítsd az összetett függvényes dolgokat: Ha csak x van pl ^ bal oldalán, akkor egyszerűsíts, illetve ha fv inputja csak x, akkor is.
        // Ezt persze csak az új deriválás függvényben csináld meg, a régit tartsd meg úgy ahogy van, letesztelve, jó az.
        // TODO: Írd át a teszteket úgy, hogy a lépésenkénti deriválást használják, és az utolsó lépéssel hasonlítsd össze az expectedet.

        private ASTNode tree = null!;
        private ASTNode expected = null!;

        /// <summary>
        /// Tests if a null tree's derivative is null.
        /// </summary>
        [TestMethod]
        public void TestNull()
        {
            Assert.IsNull(Differentiator.Differentiate(tree));
        }

        /// <summary>
        /// Tests if a constant function's derivative is 0.
        /// a' = 0 (a is real)
        /// </summary>
        [TestMethod]
        public void TestConstantFunction()
        {
            tree = new ASTNode("e");
            expected = new ASTNode("0");
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
            tree = new ASTNode("5");
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if the identity function's derivative is 1.
        /// x' = 1
        /// </summary>
        [TestMethod]
        public void TestIdentityFunction()
        {
            tree = new ASTNode("x");
            expected = new ASTNode("1");
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if addition's differentiation rule is implemented correctly.
        /// (f+g)' = f' + g'
        /// </summary>
        [TestMethod]
        public void TestAddition()
        {
            tree = new ASTNode("+",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")),
                       new ASTNode("x"));

            expected = new ASTNode("+",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("1"));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if subtraction's differentiation rule is implemented correctly.
        /// (f-g)' = f' - g'
        /// </summary>
        [TestMethod]
        public void TestSubtraction()
        {
            tree = new ASTNode("-",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")),
                       new ASTNode("x"));

            expected = new ASTNode("-",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("1"));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if multiplication's differentiation rule is implemented correctly.
        /// (f*g)' = f'*g+f*g'
        /// </summary>
        [TestMethod]
        public void TestMultiplication()
        {
            tree = new ASTNode("*",
                       new ASTNode("2"),
                       new ASTNode("x"));

            expected = new ASTNode("+",
                           new ASTNode("*",
                               new ASTNode("0"),
                               new ASTNode("x")),
                           new ASTNode("*",
                               new ASTNode("2"),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if division's differentiation rule is implemented correctly.
        /// (f/g)' / (f'*g-f*g')/g^2
        /// </summary>
        [TestMethod]
        public void TestDivision()
        {
            tree = new ASTNode("/",
                       new ASTNode("2"),
                       new ASTNode("x"));

            expected = new ASTNode("/",
                           new ASTNode("-",
                               new ASTNode("*",
                                   new ASTNode("0"),
                                   new ASTNode("x")),
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("1"))),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if e^x's derivative function is e^x.
        /// (e^x)' = e^x
        /// </summary>
        [TestMethod]
        public void TestSimpleExponentialFunctionBaseE()
        {
            tree = new ASTNode("^",
                       new ASTNode("e"),
                       new ASTNode("x"));
            Assert.IsTrue(ASTNode.AreTreesEqual(tree, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if e^f composition is differentiated correctly.
        /// (e^f)' = e^f*f'
        /// </summary>
        [TestMethod]
        public void TestExponentialComposition()
        {
            tree = new ASTNode("^",
                       new ASTNode("e"),
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("*",
                           new ASTNode("^",
                               new ASTNode("e"),
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode("2"))),
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if the general exponential function with positive base is differentiated correctly.
        /// (a^x)' = a^x*ln(a) (a>0)
        /// </summary>
        [TestMethod]
        public void TestSimpleExponentialFunctionBasePositiveNotE()
        {
            tree = new ASTNode("^",
                       new ASTNode("2"),
                       new ASTNode("x"));

            expected = new ASTNode("*",
                           new ASTNode("^",
                               new ASTNode("2"),
                               new ASTNode("x")),
                           new ASTNode("ln",
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if an exception is thrown when differentiating the general exponential function with negative base.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestSimpleExponentialFunctionBaseNegative()
        {
            tree = new ASTNode("^",
                       new ASTNode("-2"),
                       new ASTNode("x"));

            Assert.ThrowsException<Exception>(() => { 
                try
                {
                    Differentiator.Differentiate(tree);
                }
                catch (Exception e)
                {
                    if (e.Message== "Can't differentiate a^x if a <= 0 (a = -2)")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if an exponentiation is differentiated correctly if the exponent is not e.
        /// (f^n)' = n*f^(n-1)*f'
        /// </summary>
        [TestMethod]
        public void TestExponentiationExponentNotE()
        {
            tree = new ASTNode("^",
                       new ASTNode("x"),
                       new ASTNode("-2"));

            expected = new ASTNode("*",
                           new ASTNode("*",
                               new ASTNode("-2"),
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode("-3"))),
                           new ASTNode("1"));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if an exponentiation is differentiated correctly if the exponent is e.
        /// (f^n)' = n*f^(n-1)*f'
        /// </summary>
        [TestMethod]
        public void TestExponentiationExponentE()
        {
            tree = new ASTNode("^",
                       new ASTNode("x"),
                       new ASTNode("e"));

            expected = new ASTNode("*",
                           new ASTNode("*",
                               new ASTNode("e"),
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode((Math.E-1).ToString()))),
                           new ASTNode("1"));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if a functional exponentiation is differentiated correctly.
        /// (f^g)' = (f^g)*(f'*g/f+g'*ln(f)) (f>0, not checked)
        /// </summary>
        [TestMethod]
        public void TestFunctionalExponentiation()
        {
            tree = new ASTNode("^",
                       new ASTNode("x"),
                       new ASTNode("x"));

            expected = new ASTNode("*",
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
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if an exception is thrown when the logarithm has base 1.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestLogBase1()
        {
            tree = new ASTNode("log",
                       new ASTNode("1"),
                       new ASTNode("x"));
            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    Differentiator.Differentiate(tree);
                }
                catch (Exception e)
                {
                    if (e.Message == "The base of the logarithm should be a positive number other than 1.")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if an exception is thrown when the logarithm's base is not positive.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestLogBaseNotPositive()
        {
            tree = new ASTNode("log",
                       new ASTNode("0"),
                       new ASTNode("x"));
            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    Differentiator.Differentiate(tree);
                }
                catch (Exception e)
                {
                    if (e.Message == "The base of the logarithm should be a positive number other than 1.")
                    {
                        throw new Exception();
                    }
                }
            });

            tree = new ASTNode("log",
                       new ASTNode("-1"),
                       new ASTNode("x"));
            Assert.ThrowsException<Exception>(() =>
            {
                try
                {
                    Differentiator.Differentiate(tree);
                }
                catch (Exception e)
                {
                    if (e.Message == "The base of the logarithm should be a positive number other than 1.")
                    {
                        throw new Exception();
                    }
                }
            });
        }

        /// <summary>
        /// Tests if natural logarithm is differentiated correctly with log function.
        /// log_e'(f) = f'/f
        /// </summary>
        [TestMethod]
        public void TestLogBaseE()
        {
            tree = new ASTNode("log",
                       new ASTNode("e"),
                       new ASTNode("x"));

            expected = new ASTNode("/",
                           new ASTNode("1"),
                           new ASTNode("x"));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if general base logarithm is differentiated correctly.
        /// log_a'(f) = f'/(f*ln(a))
        /// </summary>
        [TestMethod]
        public void TestLogBaseNotE()
        {
            tree = new ASTNode("log",
                       new ASTNode("2"),
                       new ASTNode("x"));

            expected = new ASTNode("/",
                           new ASTNode("1"),
                           new ASTNode("*",
                               new ASTNode("x"),
                               new ASTNode("ln",
                                   new ASTNode("2"))));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if natural logarithm is differentiated correctly with ln function.
        /// ln'(f) = f'/f
        /// </summary>
        [TestMethod]
        public void TestLn()
        {
            tree = new ASTNode("ln",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("^",
                               new ASTNode("x"),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if sin is differentiated correctly.
        /// sin'(f) = cos(f)*f'
        /// </summary>
        [TestMethod]
        public void TestSin()
        {
            tree = new ASTNode("sin",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("*",
                           new ASTNode("cos",
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode("2"))),
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if cos is differentiated correctly.
        /// cos'(f)= -1*sin(f)*f'
        /// </summary>
        [TestMethod]
        public void TestCos()
        {
            tree = new ASTNode("cos",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("*",
                           new ASTNode("*",
                               new ASTNode("-1"),
                               new ASTNode("sin",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2")))),
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if tg is differentiated correctly.
        /// tg'(f) = f'/cos^2(f)
        /// </summary>
        [TestMethod]
        public void TestTg()
        {
            tree = new ASTNode("tg",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("^",
                               new ASTNode("cos",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2"))),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if ctg is differentiated correctly.
        /// ctg'(f) = (-1*f')/sin^2(f)
        /// </summary>
        [TestMethod]
        public void TestCtg()
        {
            tree = new ASTNode("ctg",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("-1"),
                               new ASTNode("*",
                                   new ASTNode("*",
                                       new ASTNode("2"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("1"))),
                                   new ASTNode("1"))),
                           new ASTNode("^",
                               new ASTNode("sin",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2"))),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arcsin is differentiated correctly.
        /// arcsin'(f) = f'/(1-f^2)^(1/2)
        /// </summary>
        [TestMethod]
        public void TestArcsin()
        {
            tree = new ASTNode("arcsin",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
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
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arccos is differentiated correctly.
        /// arccos'(f) = (-1*f')/(1-f^2)^(1/2)
        /// </summary>
        [TestMethod]
        public void TestArccos()
        {
            tree = new ASTNode("arccos",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("-1"),
                               new ASTNode("*",
                                   new ASTNode("*",
                                       new ASTNode("2"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("1"))),
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
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arctg is differentiated correctly.
        /// arctg'(f) = f'/(1+f^2)
        /// </summary>
        [TestMethod]
        public void TestArctg()
        {
            tree = new ASTNode("arctg",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("+",
                               new ASTNode("1"),
                               new ASTNode("^",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2")),
                                   new ASTNode("2"))));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arcctg is differentiated correctly.
        /// arcctg'(f) = (-1*f')/(1+f^2)
        /// </summary>
        [TestMethod]
        public void TestArcctg()
        {
            tree = new ASTNode("arcctg",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("-1"),
                               new ASTNode("*",
                                   new ASTNode("*",
                                       new ASTNode("2"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("1"))),
                                   new ASTNode("1"))),
                           new ASTNode("+",
                               new ASTNode("1"),
                               new ASTNode("^",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2")),
                                   new ASTNode("2"))));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if sh is differentiated correctly.
        /// sh'(f) = ch(f)*f'
        /// </summary>
        [TestMethod]
        public void TestSh()
        {
            tree = new ASTNode("sh",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("*",
                           new ASTNode("ch",
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode("2"))),
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if ch is differentiated correctly.
        /// ch'(f) = sh(f)*f'
        /// </summary>
        [TestMethod]
        public void TestCh()
        {
            tree = new ASTNode("ch",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("*",
                           new ASTNode("sh",
                               new ASTNode("^",
                                   new ASTNode("x"),
                                   new ASTNode("2"))),
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if th is differentiated correctly.
        /// th'(f) = f'/ch^2(f)
        /// </summary>
        [TestMethod]
        public void TestTh()
        {
            tree = new ASTNode("th",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("^",
                               new ASTNode("ch",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2"))),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if cth is differentiated correctly.
        /// cth'(f) = (-1*f')/sh^2(f)
        /// </summary>
        [TestMethod]
        public void TestCth()
        {
            tree = new ASTNode("cth",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("-1"),
                               new ASTNode("*",
                                   new ASTNode("*",
                                       new ASTNode("2"),
                                       new ASTNode("^",
                                           new ASTNode("x"),
                                           new ASTNode("1"))),
                                   new ASTNode("1"))),
                           new ASTNode("^",
                               new ASTNode("sh",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2"))),
                               new ASTNode("2")));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arsh is differentiated correctly.
        /// arsh'(f) = f'/(f^2+1)^1/2
        /// </summary>
        [TestMethod]
        public void TestArsh()
        {
            tree = new ASTNode("arsh",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
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
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arch is differentiated correctly.
        /// arch'(f) = f'/(f^2-1)^1/2
        /// </summary>
        [TestMethod]
        public void TestArch()
        {
            tree = new ASTNode("arch",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
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
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arth is differentiated correctly.
        /// arth'(f) = f'/(1-f^2)
        /// </summary>
        [TestMethod]
        public void TestArth()
        {
            tree = new ASTNode("arth",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("-",
                               new ASTNode("1"),
                               new ASTNode("^",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2")),
                                   new ASTNode("2"))));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }

        /// <summary>
        /// Tests if arcth is differentiated correctly.
        /// arcth'(f) = f'/(1-f^2)
        /// </summary>
        [TestMethod]
        public void TestArcth()
        {
            tree = new ASTNode("arcth",
                       new ASTNode("^",
                           new ASTNode("x"),
                           new ASTNode("2")));

            expected = new ASTNode("/",
                           new ASTNode("*",
                               new ASTNode("*",
                                   new ASTNode("2"),
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("1"))),
                               new ASTNode("1")),
                           new ASTNode("-",
                               new ASTNode("1"),
                               new ASTNode("^",
                                   new ASTNode("^",
                                       new ASTNode("x"),
                                       new ASTNode("2")),
                                   new ASTNode("2"))));
            Assert.IsTrue(ASTNode.AreTreesEqual(expected, Differentiator.Differentiate(tree)));
        }
    }
}
