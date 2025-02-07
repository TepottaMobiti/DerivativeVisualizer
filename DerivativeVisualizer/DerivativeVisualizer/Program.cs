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
            // Elég a ^, pow nem kell, és az alapján tudjuk eldönteni, hogy mi van, hogy mik a ^ node gyerekei. Lesz majd sok eset. pl left number, right variable -> a^x eset.
            // left variable, right number -> x^n eset. left se number, right se number -> f^g eset. Egyébként valszeg ez lefedi mind a kettőt, de mivel ez bonyolultabban néz ki, valósítsd meg
            // az egyszerűbbeket is. Azt meg csekkold, hogy tényleg lefedi-e a^x és x^n -t is a f^g eset.

            //-(x+2)^4 Hiba
            string input = "ln(x+2)";
            Tokenizer tokenizer = new Tokenizer(input);
            List<Token> tokens = tokenizer.Tokenize();
            Parser parser = new Parser(tokens);
            ASTNode ast = parser.ParseExpression();
            ast.Print();
            Console.WriteLine(ast.ToString());

            //List<Token> tokens = new List<Token>() //sin(log(2,x)*(x+3))
            //{
            //    new Token("sin",TokenType.Function),
            //    new Token("(",TokenType.LeftParen),
            //    new Token("log",TokenType.Function),
            //    new Token("(",TokenType.LeftParen),
            //    new Token("e",TokenType.Number),
            //    new Token(",",TokenType.Comma),
            //    new Token("x",TokenType.Variable),
            //    new Token(")",TokenType.RightParen),
            //    new Token("*",TokenType.Operator),
            //    new Token("(",TokenType.LeftParen),
            //    new Token("x",TokenType.Variable),
            //    new Token("+",TokenType.Operator),
            //    new Token("3",TokenType.Number),
            //    new Token(")",TokenType.RightParen),
            //    new Token(")",TokenType.RightParen)
            //};

            //ASTNode actualExpr = new ASTNode("sin",
            //                         new ASTNode("*",
            //                             new ASTNode("log",
            //                                 new ASTNode("2"),
            //                                 new ASTNode("x")),
            //                             new ASTNode("+",
            //                                 new ASTNode("2"),
            //                                 new ASTNode("3"))));
            //Parser parser = new Parser(tokens);
            //ASTNode parsedExpr = parser.ParseExpression();
            //parsedExpr.Print();
            //actualExpr.Print();
        }
    }
}
