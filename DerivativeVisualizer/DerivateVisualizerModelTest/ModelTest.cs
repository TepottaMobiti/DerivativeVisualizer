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
        public void ProcessInput_InvalidTokenization_TriggersInputProcessedWithNullTree()
        {
            // Arrange
            var model = new Model();
            bool called = false;
            ASTNode? resultTree = null;
            string resultMsg = "";

            model.InputProcessed += (tree, msg) =>
            {
                called = true;
                resultTree = tree;
                resultMsg = msg;
            };

            model.ProcessInput("@@@");

            Assert.IsTrue(called);
            Assert.IsNull(resultTree);
            Assert.IsFalse(string.IsNullOrEmpty(resultMsg));
        }

        /// <summary>
        /// Verifies that for a valid input, both the InputProcessed and TreeReady events are triggered,
        /// and that they refer to the same syntax tree.
        /// </summary>
        [TestMethod]
        public void ProcessInput_ValidInput_TriggersInputProcessedAndTreeReady()
        {
            var model = new Model();
            bool processed = false;
            bool treeReady = false;

            ASTNode? processedTree = null;
            ASTNode? readyTree = null;

            model.InputProcessed += (tree, msg) =>
            {
                processed = true;
                processedTree = tree;
            };

            model.TreeReady += (tree) =>
            {
                treeReady = true;
                readyTree = tree;
            };

            model.ProcessInput("x^2");

            Assert.IsTrue(processed);
            Assert.IsNotNull(processedTree);
            Assert.IsTrue(treeReady);
            Assert.AreSame(processedTree, readyTree);
        }

        /// <summary>
        /// Verifies that differentiating a tree node triggers both TreeUpdated and DifferentiationFinished events
        /// when no further differentiation is required after the step.
        /// </summary>
        [TestMethod]
        public void DifferentiateByLocator_TriggersTreeUpdated_AndDifferentiationFinished_WhenDone()
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
        public void DifferentiateByLocator_OnlyTriggersTreeUpdated_IfDifferentiationNotFinished()
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

        /// <summary>
        /// Helper method to create a test ASTNode with ToBeDifferentiated set to true,
        /// representing a node that is ready to be differentiated.
        /// </summary>
        private ASTNode GetDifferentiableNode()
        {
            var node = new ASTNode("x");
            node.ToBeDifferentiated = true;
            return node;
        }

    }
}
