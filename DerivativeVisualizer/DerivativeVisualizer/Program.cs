using DerivativeVisualizerModel;
using Fractions;
namespace DerivativeVisualizer
{
    // Általánosabb követelmények a kóddal: Ne legyenek felesleges sorok (kivétel függvények között 1, konvenció szerint),
    // C# konvencióknak megfeleljen, dokumentálva legyenek a függvények ///-rel. Clean Code

    //Dokumentáció: A program jól kezeli a mínuszt zárójel kirakása nélkül is. Legalábbis az alapján amit eddig láttam.
    //Tehát pl. a 3+-2-t elfogadja, 3 + (-2) -nek értelmezi, ami okés. De például a 3++2 kiakasztja, ami nem baj.
    //ln és log(e,_) külön kezeltek.



    /*
     "Binary operator '{op}' missing right-hand side operand." - "A(z) '{op}' bináris operátornak hiányzik a jobb oldali operandusa."

"Division by zero is not allowed." - "Nullával való osztás nem engedélyezett."

"Exponent operator '^' missing exponent." - "A '^' hatványozó operátornak hiányzik a kitevője."

"0^0 is undefined." - "A 0^0 nem értelmezett."

"Negative sign '-' must be followed by a number." - "A negatív előjelet '-' számnak kell követnie."

"Expected operator after {negativeValue}" - "Elvárt operátor a(z) {negativeValue} után."

"Expected operator after {t.Value}" - "Elvárt operátor a(z) {t.Value} után."

"Function '{functionName}' must be followed by '('." - "A(z) '{functionName}' függvényt zárójelnek '(' kell követnie."

"{functionName} base must be a positive number." - "A(z) {functionName} alapjának pozitív számnak kell lennie."

"Logarithm base cannot be 1." - "A logaritmus alapja nem lehet 1."

"Logarithm base cannot be 0." - "A logaritmus alapja nem lehet 0."

"Missing comma after log base." - "Hiányzó vessző a logaritmus alapja után."

"Missing closing parenthesis for function argument." - "Hiányzó zárójel a függvény argumentuma után."

"Expected operator after function." - "Elvárt operátor a függvény után."

"Missing closing parenthesis in expression." - "Hiányzó zárójel a kifejezésben."

"Expected operator after closing parenthesis." - "Elvárt operátor a zárójel bezárása után."

"Unexpected token in primary expression" - "Váratlan token az elsődleges kifejezésben"

     */

    internal class Program
    {
        static void Main(string[] args)
        {
            //string input;
            //Tokenizer tokenizer;
            //List<Token>? tokens;
            //Parser? parser;
            //ASTNode ast;
            //Console.WriteLine("Üdv a deriváló programban!");
            //while (true)
            //{
            //    Console.WriteLine("A használható függvények:\nsin,cos,tg,ctg,arcsin,arccos,arctg,arcctg\nsh,ch,th,cth,arsh,arch,arth,arcth\nlog(alap,argumentum),ln");
            //    Console.WriteLine("A befejezéshez írjon \"END\"-et");
            //    Console.Write("Adja meg a függvényt: ");
            //    input = Console.ReadLine() ?? "";
            //    if (input == "END") break;
            //    try
            //    {
            //        string msg;
            //        tokenizer = new Tokenizer(input);
            //        (tokens, msg) = tokenizer.Tokenize();
            //        parser = new Parser(tokens);
            //        (ast, msg) = parser.ParseExpression();
            //        Console.WriteLine("\nA deriválási folyamat:");
            //        Differentiator diff = new Differentiator(ast);
            //        diff.Differentiate();
            //        List<ASTNode> steps = diff.GetSteps();
            //        foreach (ASTNode step in steps)
            //        {
            //            (step).Print();
            //            Console.WriteLine("-------------------------------");
            //        }
            //        Console.WriteLine("A deriváltfüggvény szintaxisfája:");
            //        ASTNode.Simplify(steps.Last()).Print();
            //        Console.Write("\nA deriváltfüggvény hozzárendelési szabálya:\nf(x) = ");
            //        Console.WriteLine(ASTNode.Simplify(Differentiator.Differentiate(ast)).ToString());
            //        Console.WriteLine("-------------------------------");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine($"Hiba: {e.Message}");
            //    }
            //}
            Fraction fraction = Fraction.FromDoubleRounded(-1.15);
            Console.WriteLine(fraction);
        }
    }
}
