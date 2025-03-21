using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerivativeVisualizerModel
{
    // Hiba: Valamiért elfogadunk olyan inputot, hogy "x5". Ki kéne kényszeríteni, hogy minden szám vagy x után csak operátor jöhessen? Igazából igen, mert a levelek úgyis csak szám meg x lehet a fában.
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
                    return (null, msg);
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
                    return (null, msg);
                }
                if (op == "/" && right.Value == "0")
                {
                    return (null, "Division by zero is not allowed.");
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
                    return (null, msg);
                }
                if (node.Value == "0" && right.Value == "0")
                {
                    return (null, "0^0 is undefined.");
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
                    return (null, "Negative sign must be followed by a number.");
                }
                (t, msg) = Consume();
                if (t is null)
                {
                    return (null, msg);
                }
                string negativeValue = "-" + t.Value;

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, $"Expected operator after {negativeValue}");
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
                    return (null, $"Expected operator after {t.Value}");
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
                Consume(TokenType.LeftParen);

                if (functionName == "log")
                {
                    if (!Match(TokenType.Number))
                    {
                        return (null, $"{functionName} base must be a number.");
                    }
                    (t, msg) = Consume();
                    if (t is null)
                    {
                        return (null, msg);
                    }
                    ASTNode baseNode = new ASTNode(t.Value);

                    if (baseNode.Value == "1")
                    {
                        return (null, "Logarithm base cannot be 1.");
                    }

                    var (comma, commaMsg) = Consume(TokenType.Comma);
                    if (comma is null)
                    {
                        return (null, "Missing comma after log base.");
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
                        return (null, "Missing closing parenthesis for function argument.");
                    }

                    if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                    {
                        return (null, "Expected operator after function.");
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
                    return (null, "Missing closing parenthesis for function argument.");
                }

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, "Expected operator after function.");
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
                    return (null, "Missing closing parenthesis in expression.");
                }

                if (!IsAtEnd() && !Match(TokenType.Operator) && !Match(TokenType.RightParen))
                {
                    return (null, "Expected operator after closing parenthesis.");
                }

                return (expression, "");
            }

            return (null, "Unexpected token in primary expression");
        }




        private bool Match(TokenType type)
        {
            return !IsAtEnd() && tokens[currentIndex].Type == type;
        }

        private (Token?,string) Consume()
        {
            if (IsAtEnd()) return (null,"Unexpected end of input");
            return (tokens[currentIndex++],"");
        }

        private (Token?,string) Consume(TokenType type)
        {
            if (!Match(type)) return (null,$"Expected token type {type}");
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