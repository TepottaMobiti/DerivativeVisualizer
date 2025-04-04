using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    // TODO: 0^0 Check parserben.
    // TODO: a^f deriválási szabály.
    public class Model
    {
        private Tokenizer tokenizer = null!;
        private Parser parser = null!;
        private Differentiator differentiator = null!;
        List<Token>? tokens = null!;
        public string Input { get; private set; } = string.Empty;

        public delegate void InputProcessedDelegate(ASTNode? tree, string msg);

        public event InputProcessedDelegate InputProcessed = null!;

        private void OnInputProcessed(ASTNode? tree, string msg)
        {
            InputProcessed?.Invoke(tree, msg);
        }

        public delegate void TreeUpdatedDelegate(ASTNode? tree);

        public event TreeUpdatedDelegate TreeUpdated = null!;

        private void OnTreeUpdated(ASTNode? tree)
        {
            TreeUpdated?.Invoke(tree);
        }

        public event TreeUpdatedDelegate TreeReady = null!;

        private void OnTreeReady(ASTNode? tree)
        {
            TreeReady?.Invoke(tree);
        }

        public delegate void DifferentiationFinishedDelegate(ASTNode? tree);

        public event DifferentiationFinishedDelegate DifferentiationFinished = null!;

        private void OnDifferentiationFinished(ASTNode? tree)
        {
            DifferentiationFinished?.Invoke(tree);
        }

        public void ProcessInput(string input)
        {
            Input = input;

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
