using System.Numerics;
using System.Text;
using CalculatorWPF.Models;

namespace CalculatorWPF.Services
{
    /// <summary>
    /// Tokenizes mathematical expressions into a list of tokens
    /// </summary>
    public class Tokenizer
    {
        private readonly string _expression;
        private int _position;

        public Tokenizer(string expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _position = 0;
        }

        /// <summary>
        /// Tokenizes the entire expression
        /// </summary>
        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            
            while (_position < _expression.Length)
            {
                SkipWhitespace();
                
                if (_position >= _expression.Length)
                    break;

                char current = _expression[_position];

                // Check for operators
                if (IsOperator(current))
                {
                    // Handle unary minus (negative numbers)
                    if (current == '-' && (tokens.Count == 0 || tokens.Last().Type == TokenType.Operator))
                    {
                        tokens.Add(ReadNumber(true));
                    }
                    else if (current == '+' && (tokens.Count == 0 || tokens.Last().Type == TokenType.Operator))
                    {
                        // Unary plus - just skip it
                        _position++;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, _position, operatorSymbol: current.ToString()));
                        _position++;
                    }
                }
                else if (char.IsDigit(current))
                {
                    tokens.Add(ReadNumber(false));
                }
                else if (current == '.')
                {
                    throw new InvalidOperationException($"Invalid character at position {_position}: '{current}'. Decimal numbers are not supported.");
                }
                else
                {
                    throw new InvalidOperationException($"Invalid character at position {_position}: '{current}'");
                }
            }

            tokens.Add(new Token(TokenType.End, _position));
            return tokens;
        }

        private Token ReadNumber(bool isNegative)
        {
            int startPos = _position;
            var sb = new StringBuilder();

            if (isNegative)
            {
                sb.Append('-');
                _position++; // Skip the minus sign
                SkipWhitespace(); // Allow space after minus
            }

            if (_position >= _expression.Length || !char.IsDigit(_expression[_position]))
            {
                throw new InvalidOperationException($"Expected digit at position {_position}");
            }

            while (_position < _expression.Length && char.IsDigit(_expression[_position]))
            {
                sb.Append(_expression[_position]);
                _position++;
            }

            // Check if there's a decimal point following
            if (_position < _expression.Length && _expression[_position] == '.')
            {
                throw new InvalidOperationException($"Invalid character at position {_position}: '.'. Decimal numbers are not supported.");
            }

            string numberStr = sb.ToString();
            if (!BigInteger.TryParse(numberStr, out BigInteger value))
            {
                throw new InvalidOperationException($"Invalid number format at position {startPos}: '{numberStr}'");
            }

            return new Token(TokenType.Number, startPos, value: value);
        }

        private void SkipWhitespace()
        {
            while (_position < _expression.Length && char.IsWhiteSpace(_expression[_position]))
            {
                _position++;
            }
        }

        private static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }
    }
}
