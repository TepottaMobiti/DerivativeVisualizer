using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    public class Parser
    {
        private List<Token> tokens;
        private int currentIndex;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            currentIndex = 0;
        }

        public (ASTNode?, string) ParseExpression()
        {
            return ParseTerm();
        }

        private (ASTNode?, string) ParseTerm()
        {
            ASTNode? node;
            string msg;
            (node, msg) = ParseFactor();
            if (node is null)
            {
                return (null, msg);
            }
            while (Match(TokenType.Operator) && (CurrentToken().Value == "+" || CurrentToken().Value == "-"))
            {
                Token? token;
                (token, msg) = Consume();
                if (token is null)
                {
                    return (null, msg);
                }
                string op = token.Value;
                ASTNode? right;
                (right, msg) = ParseFactor();
                if (right is null)
                {
                    return (null, $"A(z) '{op}' bináris operátornak hiányzik a jobb oldali operandusa.");
                }
                node = new ASTNode(op, node, right);
            }
            return (node,"");
        }

        private (ASTNode?,string) ParseFactor()
        {
            ASTNode? node;
            string msg;
            (node, msg) = ParseExponent();
            if (node is null)
            {
                return (null, msg);
            }
            while (Match(TokenType.Operator) && (CurrentToken().Value == "*" || CurrentToken().Value == "/"))
            {
                Token? token;
                (token, msg) = Consume();
                if (token is null)
                {
                    return (null, msg);
                }
                string op = token.Value;
                ASTNode? right;
                (right, msg) = ParseExponent();
                if (right is null)
                {
                    return (null, $"A(z) '{op}' bináris operátornak hiányzik a jobb oldali operandusa.");
                }
                if (op == "/")
                {
                    ASTNode simplifiedRight = ASTNode.Simplify(right);
                    if (simplifiedRight.Value == "0")
                    {
                        return (null, "Nullával való osztás nem engedélyezett.");
                    }
                    
                }
                node = new ASTNode(op, node, right);
            }
            return (node,"");
        }

        private (ASTNode?,string) ParseExponent()
        {
            ASTNode? node;
            string msg;
            (node, msg) = ParsePrimary();
            if (node is null)
            {
                return (null,msg);
            }
            if (Match(TokenType.Operator) && CurrentToken().Value == "^")
            {
                Token? token;
                (token, msg) = Consume();
                if (token is null)
                {
                    return (null, msg);
                }
                string op = token.Value;
                ASTNode? right;
                (right, msg) = ParseExponent();
                if (right is null)
                {
                    return (null, "A '^' hatványozás operátornak hiányzik a kitevője.");
                }
                ASTNode simplifiedLeft = ASTNode.Simplify(node);
                ASTNode simplifiedRight = ASTNode.Simplify(right);

                if (simplifiedLeft.Value == "0" && simplifiedRight.Value == "0")
                {
                    return (null, "A 0^0 nem értelmezett.");
                }
                node = new ASTNode(op, node, right);
            }
            return (node,"");
        }


        private (ASTNode?, string) ParsePrimary()
        {
            Token? t;
            string msg;

            if (Match(TokenType.Operator) && CurrentToken().Value == "-")
            {
                Consume();
                if (!Match(TokenType.Number))
                {
                    return (null, "A negatív előjelet '-' számnak kell követnie.");
                }
                (t, msg) = Consume();
                if (t is null)
                {
                    return (null, msg);
                }
                string negativeValue = "-" + t.Value;

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, $"Hiányzó operátor a {negativeValue} után.");
                }

                return (new ASTNode(negativeValue), "");
            }

            if (Match(TokenType.Number) || Match(TokenType.Variable))
            {
                (t, msg) = Consume();
                if (t is null)
                {
                    return (null, msg);
                }
                ASTNode node = new ASTNode(t.Value);

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, $"Hiányzó operátor a(z) {t.Value} után.");
                }

                return (node, "");
            }

            if (Match(TokenType.Function))
            {
                Token? rightParen;
                (t, msg) = Consume();
                if (t is null)
                {
                    return (null, msg);
                }
                string functionName = t.Value;

                if (!Match(TokenType.LeftParen))
                    return (null, $"A '{functionName}' függvény után nyitó zárójelnek kell következnie: '('.");

                Consume(TokenType.LeftParen);

                if (Match(TokenType.RightParen))
                {
                    return (null, $"A(z) '{functionName}' függvénynek hiányzik az argumentuma.");
                }

                if (functionName == "log")
                {
                    if (Match(TokenType.Comma))
                    {
                        return (null, "A(z) 'log' függvényből hiányzik az alap.");
                    }

                    if (!Match(TokenType.Number))
                    {
                        return (null, $"Logaritmus alapjának pozitív számnak kell lennie.");
                    }

                    (t, msg) = Consume();
                    if (t is null)
                    {
                        return (null, msg);
                    }
                    ASTNode baseNode = new ASTNode(t.Value);

                    if (baseNode.Value == "1")
                    {
                        return (null, "Logaritmus alapja nem lehet 1.");
                    }

                    if (baseNode.Value == "0")
                    {
                        return (null, "Logaritmus alapja nem lehet 0.");
                    }

                    var (comma, commaMsg) = Consume(TokenType.Comma);
                    if (comma is null)
                    {
                        return (null, "A logaritmus alapja után vesszőnek kell következnie: ','.");
                    }

                    if (Match(TokenType.RightParen))
                    {
                        return (null, "A(z) 'log' függvényből hiányzik az argumentum a vessző után.");
                    }

                    ASTNode? argumentNode;
                    (argumentNode, msg) = ParseExpression();
                    if (argumentNode is null)
                    {
                        return (null, msg);
                    }

                    (rightParen, msg) = Consume(TokenType.RightParen);
                    if (rightParen is null)
                    {
                        return (null, $"A {functionName} függvény argumentuma után bezáró zárójelnek kell következnie: ')'.");
                    }

                    if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                    {
                        return (null, $"A {functionName} függvény bezáró zárójele után operátornak kell következnie.");
                    }

                    return (new ASTNode(functionName, baseNode, argumentNode), "");
                }

                ASTNode? argument;
                (argument, msg) = ParseExpression();
                if (argument is null)
                {
                    return (null, msg);
                }

                (rightParen, msg) = Consume(TokenType.RightParen);
                if (rightParen is null)
                {
                    return (null, $"A {functionName} függvény argumentuma után bezáró zárójelnek kell következnie: ')'.");
                }

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, $"A {functionName} függvény bezáró zárójele után operátornak kell következnie.");
                }

                ASTNode node = new ASTNode(functionName, argument);
                return (node, "");
            }

            if (Match(TokenType.LeftParen))
            {
                Consume();
                ASTNode? expression;
                (expression, msg) = ParseExpression();
                if (expression is null)
                {
                    return (null, msg);
                }

                (Token? rightParen, msg) = Consume(TokenType.RightParen);
                if (rightParen is null)
                {
                    return (null, "A kifejezésből hiányzik egy bezáró zárójel: ')'.");
                }

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, "A bezáró zárójel után operátornak kell következnie.");
                }

                return (expression, "");
            }

            return (null, "Nem várt token a kifejezésben.");
        }




        private bool Match(TokenType type)
        {
            return !IsAtEnd() && tokens[currentIndex].Type == type;
        }

        private (Token?,string) Consume()
        {
            if (IsAtEnd()) return (null,"A bemenet nem várt módon ért véget.");
            return (tokens[currentIndex++],"");
        }

        private (Token?,string) Consume(TokenType type)
        {
            if (!Match(type)) return (null,$"Elvárt token típusa: {type}.");
            return Consume();
        }

        private Token CurrentToken()
        {
            return tokens[currentIndex];
        }

        private bool IsAtEnd()
        {
            return currentIndex >= tokens.Count;
        }
    }
}