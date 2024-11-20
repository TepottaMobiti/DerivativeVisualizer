using DerivativeVisualizerModel;
namespace DerivativeVisualizer
{
    // Általánosabb követelmények a kóddal: Ne legyenek felesleges sorok (kivétel függvények között 1, konvenció szerint),
    // C# konvencióknak megfeleljen, dokumentálva legyenek a függvények ///-rel. Clean Code
    // Hiba: mínuszos dolgokat jelenleg nem eszi meg, "0-1"-el lehet negatív számot beadni neki.
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "(0-1)/x";
            Tokenizer tokenizer = new Tokenizer(input);
            List<Token> tokens = tokenizer.Tokenize();
            Parser parser = new Parser(tokens);
            ASTNode ast = parser.ParseExpression();
            Console.WriteLine(ast.ToString());
        }
    }
}
