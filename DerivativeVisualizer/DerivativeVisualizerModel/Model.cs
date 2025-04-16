using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    public class Model
    {
        private Tokenizer tokenizer = null!;
        private Parser parser = null!;
        private Differentiator differentiator = null!;
        private List<Token>? tokens = null!;

        public delegate void InputProcessedDelegate(ASTNode? tree, string msg);

        public event InputProcessedDelegate InputProcessed = null!;

        private void OnInputProcessed(ASTNode? tree, string msg)
        {
            InputProcessed?.Invoke(tree, msg);
        }

        public delegate void TreeDelegate(ASTNode? tree);

        public event TreeDelegate TreeUpdated = null!;

        private void OnTreeUpdated(ASTNode? tree)
        {
            TreeUpdated?.Invoke(tree);
        }

        public event TreeDelegate TreeReady = null!;

        private void OnTreeReady(ASTNode? tree)
        {
            TreeReady?.Invoke(tree);
        }

        public event TreeDelegate DifferentiationFinished = null!;

        private void OnDifferentiationFinished(ASTNode? tree)
        {
            DifferentiationFinished?.Invoke(tree);
        }

        /// <summary>
        /// Tokenizes and parses the user’s input string into an abstract syntax tree, initializes the differentiator if parsing is successful,
        /// and triggers corresponding events based on success or failure.
        /// </summary>
        /// <param name="input"></param>
        public void ProcessInput(string input)
        {
            tokenizer = new Tokenizer(input);

            string msg;
            (tokens, msg) = tokenizer.Tokenize();
            if (tokens is null)
            {
                OnInputProcessed(null, msg);
            }
            else
            {
                parser = new Parser(tokens);
                ASTNode? tree;
                (tree, msg) = parser.ParseExpression();
                if (tree is not null)
                {
                    differentiator = new Differentiator(tree);
                    OnTreeReady(differentiator.CurrentTree);
                }
                OnInputProcessed(tree,msg);
            }
        }

        /// <summary>
        /// Performs one differentiation step at the specified locator in the syntax tree, triggers an update event, and if no further differentiation is needed,
        /// signals that differentiation is finished with a simplified tree.
        /// </summary>
        /// <param name="locator"></param>
        public void DifferentiateByLocator(int locator)
        {
            ASTNode differentiatedTree = differentiator.Differentiate(locator);
            OnTreeUpdated(differentiatedTree);

            if (!ASTNode.HasDifferentiationNode(differentiatedTree))
            {
                OnDifferentiationFinished(ASTNode.Simplify(differentiatedTree));
            }
        }
    }
}
