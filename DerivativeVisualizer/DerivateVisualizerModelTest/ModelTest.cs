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
    public class ModelTest
    {
        /// <summary>
        /// Verifies that if tokenization fails, the InputProcessed event is triggered with a null syntax tree
        /// and an appropriate error message.
        /// </summary>
        [TestMethod]
        public void ProcessInputInvalidTokenizationTriggersInputProcessedWithNullTree()
        {
            var model = new Model();
            bool called = false;
            string resultMsg = "";

            model.InputProcessed += (msg) =>
            {
                called = true;
                resultMsg = msg;
            };

            model.ProcessInput("@@@");

            Assert.IsTrue(called);
            Assert.IsFalse(string.IsNullOrEmpty(resultMsg));
        }

        /// <summary>
        /// Verifies that for a valid input, both the InputProcessed and TreeReady events are triggered,
        /// and that they refer to the same syntax tree.
        /// </summary>
        [TestMethod]
        public void ProcessInputValidInputTriggersInputProcessedAndTreeReady()
        {
            var model = new Model();
            bool processed = false;
            bool treeReady = false;

            ASTNode? readyTree = null;

            model.InputProcessed += (msg) =>
            {
                processed = true;
            };

            model.TreeReady += (tree) =>
            {
                treeReady = true;
                readyTree = tree;
            };

            model.ProcessInput("x^2");

            Assert.IsTrue(processed);
            Assert.IsTrue(treeReady);
        }

        /// <summary>
        /// Verifies that differentiating a tree node triggers both TreeUpdated and DifferentiationFinished events
        /// when no further differentiation is required after the step.
        /// </summary>
        [TestMethod]
        public void DifferentiateByLocatorTriggersTreeUpdatedAndDifferentiationFinishedWhenDone()
        {
            var model = new Model();
            ASTNode? readyTree = null;

            model.TreeReady += (tree) =>
            {
                readyTree = tree;
            };

            model.ProcessInput("x^2");

            Assert.IsNotNull(readyTree, "TreeReady event should have provided a tree.");

            int locator = readyTree!.Locator;

            bool updated = false;
            bool finished = false;

            ASTNode? updatedTree = null;
            ASTNode? finishedTree = null;

            model.TreeUpdated += (tree) =>
            {
                updated = true;
                updatedTree = tree;
            };

            model.DifferentiationFinished += (tree) =>
            {
                finished = true;
                finishedTree = tree;
            };

            model.DifferentiateByLocator(locator);

            Assert.IsTrue(updated, "TreeUpdated should have been triggered.");
            Assert.IsNotNull(updatedTree, "Updated tree should not be null.");

            Assert.IsTrue(finished, "DifferentiationFinished should have been triggered.");
            Assert.IsNotNull(finishedTree, "Finished tree should not be null.");
        }

        /// <summary>
        /// Verifies that if the tree to differentiate does not contain any node flagged for differentiation,
        /// TreeUpdated is still triggered and DifferentiationFinished is also triggered immediately after.
        /// </summary>
        [TestMethod]
        public void DifferentiateByLocatorOnlyTriggersTreeUpdatedIfDifferentiationNotFinished()
        {
            var model = new Model();

            var nonDifferentiableTree = new ASTNode("x");
            nonDifferentiableTree.ToBeDifferentiated = false;

            var differentiatorField = typeof(Model).GetField("differentiator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            differentiatorField!.SetValue(model, new Differentiator(nonDifferentiableTree));

            bool updated = false;
            bool finished = false;

            model.TreeUpdated += (tree) => updated = true;
            model.DifferentiationFinished += (tree) => finished = true;

            model.DifferentiateByLocator(nonDifferentiableTree.Locator);

            Assert.IsTrue(updated);
            Assert.IsTrue(finished);
        }
    }
}