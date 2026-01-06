using System.Numerics;
using CalculatorWPF.Models;

namespace CalculatorWPF.Services
{
    /// <summary>
    /// Evaluates mathematical expressions with proper operator precedence
    /// </summary>
    public class ExpressionEvaluator
    {
        private List<Token> _tokens;
        private int _currentIndex;

        public ExpressionEvaluator()
        {
            _tokens = new List<Token>();
            _currentIndex = 0;
        }

        /// <summary>
        /// Evaluates an expression string and returns the result
        /// </summary>
        public BigInteger Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new InvalidOperationException("Expression cannot be empty");
            }

            var tokenizer = new Tokenizer(expression);
            _tokens = tokenizer.Tokenize();
            _currentIndex = 0;

            BigInteger result = ParseExpression();

            // Ensure we've consumed all tokens except the End token
            if (_currentIndex < _tokens.Count - 1)
            {
                throw new InvalidOperationException($"Unexpected token at position {_tokens[_currentIndex].Position}");
            }

            return result;
        }

        /// <summary>
        /// Parses addition and subtraction (lowest precedence)
        /// </summary>
        private BigInteger ParseExpression()
        {
            BigInteger left = ParseTerm();

            while (_currentIndex < _tokens.Count)
            {
                Token current = _tokens[_currentIndex];
                
                if (current.Type == TokenType.Operator && (current.Operator == "+" || current.Operator == "-"))
                {
                    _currentIndex++;
                    BigInteger right = ParseTerm();

                    if (current.Operator == "+")
                    {
                        left = BigInteger.Add(left, right);
                    }
                    else
                    {
                        left = BigInteger.Subtract(left, right);
                    }
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        /// <summary>
        /// Parses multiplication and division (higher precedence)
        /// </summary>
        private BigInteger ParseTerm()
        {
            BigInteger left = ParseFactor();

            while (_currentIndex < _tokens.Count)
            {
                Token current = _tokens[_currentIndex];
                
                if (current.Type == TokenType.Operator && (current.Operator == "*" || current.Operator == "/"))
                {
                    string op = current.Operator;
                    _currentIndex++;
                    BigInteger right = ParseFactor();

                    if (op == "*")
                    {
                        left = BigInteger.Multiply(left, right);
                    }
                    else
                    {
                        if (right == 0)
                        {
                            throw new DivideByZeroException("Division by zero");
                        }
                        left = BigInteger.Divide(left, right);
                    }
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        /// <summary>
        /// Parses a number (factor)
        /// </summary>
        private BigInteger ParseFactor()
        {
            if (_currentIndex >= _tokens.Count)
            {
                throw new InvalidOperationException("Unexpected end of expression");
            }

            Token current = _tokens[_currentIndex];

            if (current.Type == TokenType.Number)
            {
                _currentIndex++;
                return current.Value!.Value;
            }

            throw new InvalidOperationException($"Expected number at position {current.Position}");
        }
    }
}
