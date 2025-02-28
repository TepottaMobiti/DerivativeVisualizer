using DerivativeVisualizerModel;
namespace DerivativeVisualizer
{
    // Általánosabb követelmények a kóddal: Ne legyenek felesleges sorok (kivétel függvények között 1, konvenció szerint),
    // C# konvencióknak megfeleljen, dokumentálva legyenek a függvények ///-rel. Clean Code

    //Dokumentáció: A program jól kezeli a mínuszt zárójel kirakása nélkül is. Legalábbis az alapján amit eddig láttam.
    //Tehát pl. a 3+-2-t elfogadja, 3 + (-2) -nek értelmezi, ami okés. De például a 3++2 kiakasztja, ami nem baj.
    //ln és log(e,_) külön kezeltek.

    internal class Program
    {
        static void Main(string[] args)
        {
            string input;
            Tokenizer tokenizer;
            List<Token> tokens;
            Parser parser;
            ASTNode ast;
            Console.WriteLine("Üdv a deriváló programban!");
            while (true)
            {
                Console.WriteLine("A használható függvények:\nsin,cos,tg,ctg,arcsin,arccos,arctg,arcctg\nsh,ch,th,cth,arsh,arch,arth,arcth\nlog(alap,argumentum),ln");
                Console.WriteLine("A befejezéshez írjon \"END\"-et");
                Console.Write("Adja meg a függvényt: ");
                input = Console.ReadLine() ?? "";
                if (input == "END") break;
                try
                {
                    tokenizer = new Tokenizer(input);
                    tokens = tokenizer.Tokenize();
                    parser = new Parser(tokens);
                    ast = parser.ParseExpression();
                    Console.WriteLine("\nA deriválási folyamat:");
                    Differentiator diff = new Differentiator(ast);
                    diff.Differentiate();
                    List<ASTNode> steps = diff.GetSteps();
                    foreach (ASTNode step in steps)
                    {
                        (step).Print();
                        Console.WriteLine("-------------------------------");
                    }
                    Console.WriteLine("A deriváltfüggvény szintaxisfája:");
                    ASTNode.Simplify(steps.Last()).Print();
                    Console.Write("\nA deriváltfüggvény hozzárendelési szabálya:\nf(x) = ");
                    Console.WriteLine(ASTNode.Simplify(Differentiator.Differentiate(ast)).ToString());
                    Console.WriteLine("-------------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Hiba: {e.Message}");
                }
            }
        }
    }
}
