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

        public ASTNode ParseExpression()
        {
            return ParseTerm();
        }

        private ASTNode ParseTerm()
        {
            ASTNode node = ParseFactor();
            while (Match(TokenType.Operator) && (CurrentToken().Value == "+" || CurrentToken().Value == "-"))
            {
                string op = Consume().Value;
                ASTNode right = ParseFactor();
                node = new ASTNode(op, node, right);
            }
            return node;
        }

        private ASTNode ParseFactor()
        {
            ASTNode node = ParseExponent();
            while (Match(TokenType.Operator) && (CurrentToken().Value == "*" || CurrentToken().Value == "/"))
            {
                string op = Consume().Value;
                ASTNode right = ParseExponent();
                node = new ASTNode(op, node, right);
            }
            return node;
        }

        private ASTNode ParseExponent()
        {
            ASTNode node = ParsePrimary();
            if (Match(TokenType.Operator) && CurrentToken().Value == "^")
            {
                string op = Consume().Value;
                ASTNode right = ParseExponent();
                node = new ASTNode(op, node, right);
            }
            return node;
        }


        private ASTNode ParsePrimary()
        {
            if (Match(TokenType.Operator) && CurrentToken().Value == "-")
            {
                Consume();
                if (!Match(TokenType.Number))
                {
                    throw new Exception("Negative sign must be followed by a number.");
                }
                string negativeValue = "-" + Consume().Value;
                return new ASTNode(negativeValue);
            }
            if (Match(TokenType.Number) || Match(TokenType.Variable))
            {
                ASTNode node = new ASTNode(Consume().Value);

                if (Match(TokenType.Function) || Match(TokenType.Variable) || Match(TokenType.LeftParen))
                {
                    throw new Exception("Missing explicit multiplication operator (*)");
                }

                return node;
            }
            if (Match(TokenType.Function))
            {
                string functionName = Consume().Value;
                Consume(TokenType.LeftParen);

                if (functionName == "log")
                {
                    if (!Match(TokenType.Number))
                    {
                        throw new Exception($"{functionName} base must be a number.");
                    }
                    ASTNode baseNode = new ASTNode(Consume().Value);

                    if (baseNode.Value == "1")
                    {
                        throw new Exception("Logarithm base cannot be 1.");
                    }

                    Consume(TokenType.Comma);

                    ASTNode argumentNode = ParseExpression();
                    Consume(TokenType.RightParen);

                    return new ASTNode(functionName, baseNode, argumentNode);
                }

                ASTNode argument = ParseExpression();
                Consume(TokenType.RightParen);
                ASTNode node = new ASTNode(functionName, argument);

                if (Match(TokenType.Function) || Match(TokenType.Variable) || Match(TokenType.LeftParen))
                {
                    throw new Exception("Missing explicit multiplication operator (*)");
                }

                return node;
            }
            if (Match(TokenType.LeftParen))
            {
                Consume();
                ASTNode expression = ParseExpression();
                Consume(TokenType.RightParen);

                if (Match(TokenType.Number) || Match(TokenType.Function) || Match(TokenType.Variable) || Match(TokenType.LeftParen))
                {
                    throw new Exception("Missing explicit multiplication operator (*)");
                }

                return expression;
            }
            throw new Exception("Unexpected token in primary expression");
        }



        private bool Match(TokenType type)
        {
            return !IsAtEnd() && tokens[currentIndex].Type == type;
        }

        private Token Consume()
        {
            if (IsAtEnd()) throw new Exception("Unexpected end of input");
            return tokens[currentIndex++];
        }

        private void Consume(TokenType type)
        {
            if (!Match(type)) throw new Exception($"Expected token type {type}, but got {CurrentToken().Type}");
            Consume();
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