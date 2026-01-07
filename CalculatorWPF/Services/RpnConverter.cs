using CalculatorWPF.Models;

namespace CalculatorWPF.Services
{
    // Converts infix expressions to Reverse Polish Notation (RPN) using Shunting Yard algorithm
    public class RpnConverter
    {
        // Converts list of tokens from infix to RPN (postfix) notation
        public List<Token> ConvertToRpn(List<Token> infixTokens)
        {
            var output = new List<Token>();
            var operatorStack = new Stack<Token>();

            foreach (var token in infixTokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        output.Add(token);
                        break;

                    case TokenType.Operator:
                        while (operatorStack.Count > 0 &&
                               operatorStack.Peek().Type == TokenType.Operator &&
                               ShouldPopOperator(token.Operator!, operatorStack.Peek().Operator!))
                        {
                            output.Add(operatorStack.Pop());
                        }
                        operatorStack.Push(token);
                        break;

                    case TokenType.LeftParenthesis:
                        operatorStack.Push(token);
                        break;

                    case TokenType.RightParenthesis:
                        // Pop operators until we find the matching left parenthesis
                        bool foundLeftParen = false;
                        while (operatorStack.Count > 0)
                        {
                            var top = operatorStack.Pop();
                            if (top.Type == TokenType.LeftParenthesis)
                            {
                                foundLeftParen = true;
                                break;
                            }
                            output.Add(top);
                        }
                        
                        if (!foundLeftParen)
                        {
                            throw new InvalidOperationException($"Mismatched parentheses: no opening parenthesis for closing parenthesis at position {token.Position}");
                        }
                        break;

                    case TokenType.End:
                        // Pop all remaining operators
                        while (operatorStack.Count > 0)
                        {
                            var top = operatorStack.Pop();
                            if (top.Type == TokenType.LeftParenthesis)
                            {
                                throw new InvalidOperationException($"Mismatched parentheses: unclosed opening parenthesis at position {top.Position}");
                            }
                            output.Add(top);
                        }
                        break;
                }
            }

            return output;
        }

        // Determines if we should pop an operator from the stack
        private bool ShouldPopOperator(string currentOp, string stackOp)
        {
            int currentPrecedence = GetPrecedence(currentOp);
            int stackPrecedence = GetPrecedence(stackOp);

            // Right-associative operators (like ^) should not pop operators of equal precedence
            if (IsRightAssociative(currentOp))
            {
                return stackPrecedence > currentPrecedence;
            }
            else
            {
                return stackPrecedence >= currentPrecedence;
            }
        }

        // Returns the precedence level of an operator (higher = higher precedence)
        private int GetPrecedence(string op)
        {
            return op switch
            {
                "+" or "-" => 1,
                "*" or "/" => 2,
                "^" => 3,
                _ => 0
            };
        }

        // Returns true if the operator is right-associative
        private bool IsRightAssociative(string op)
        {
            return op == "^";
        }

        // Validates that all parentheses are properly matched
        public void ValidateParentheses(List<Token> tokens)
        {
            int openCount = 0;
            Token? firstOpenParen = null;

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.LeftParenthesis)
                {
                    if (openCount == 0)
                    {
                        firstOpenParen = token;
                    }
                    openCount++;
                }
                else if (token.Type == TokenType.RightParenthesis)
                {
                    openCount--;
                    if (openCount < 0)
                    {
                        throw new InvalidOperationException($"Mismatched parentheses: no opening parenthesis for closing parenthesis at position {token.Position}");
                    }
                }
            }

            if (openCount > 0)
            {
                int position = firstOpenParen?.Position ?? 0;
                throw new InvalidOperationException($"Mismatched parentheses: unclosed opening parenthesis at position {position}");
            }
        }
    }
}
