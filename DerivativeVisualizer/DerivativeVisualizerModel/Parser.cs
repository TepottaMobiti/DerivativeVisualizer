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
            while (Match(TokenType.Operator) && CurrentToken().Value == "^")
            {
                string op = Consume().Value;
                ASTNode right = ParsePrimary();
                node = new ASTNode(op, node, right);
            }
            return node;
        }

        private ASTNode ParsePrimary()
        {
            if (Match(TokenType.Number) || Match(TokenType.Variable))
            {
                return new ASTNode(Consume().Value);
            }
            if (Match(TokenType.Function))
            {
                string functionName = Consume().Value;
                Consume(TokenType.LeftParen);
                ASTNode argument = ParseExpression();
                Consume(TokenType.RightParen);
                return new ASTNode(functionName, argument);
            }
            if (Match(TokenType.LeftParen))
            {
                Consume();
                ASTNode expression = ParseExpression();
                Consume(TokenType.RightParen);
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
