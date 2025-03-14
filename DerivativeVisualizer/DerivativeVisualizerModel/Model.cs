using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    // TODO: Implementálni mindent. Ez vonja össze az összes többi class funkcionalitását. Itt vannak az eventek.
    // Egyetlen input mezőre van szüksége, ennyit fog kapni a viewtől, és utána csak eventeket vált ki. Minden többi classtól jövő exceptiont önmagában lekezel, csak eventekkel kommunikál kifelé.
    // Itt legyen minden üzleti logika, a viewmodelben semmilyen komolyabb számítás ne legyen.
    // Modellben nem try-catchelünk, majd az appban kezeljük az ilyen errorokat az App-ban felugró ablakokkal.
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

        private void OnTreeUpdated(ASTNode tree)
        {
            TreeUpdated?.Invoke(tree);
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
                if (tree is not null) // Vagy így hozzuk létre a deriválót, és akkor sok felesleges deriváló fog létrejönni, vagy kell egy start differentiation process gomb, ami létrehozza ezt, és onnantól lehet kattintgatni.
                {
                    differentiator = new Differentiator(tree);
                    OnTreeUpdated(differentiator.CurrentTree);
                }
                OnInputProcessed(tree,msg);
            }
        }

        public void DifferentiateByLocator(int locator)
        {
            ASTNode differentiatedTree = differentiator.Differentiate(locator);
            OnTreeUpdated(differentiatedTree);
        }
    }
}
