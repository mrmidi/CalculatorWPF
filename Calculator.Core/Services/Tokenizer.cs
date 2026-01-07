using System.Text;
using CalculatorWPF.Models;

namespace CalculatorWPF.Services
{
    // Converts expression string into tokens
    public class Tokenizer
    {
        private readonly string _expression;
        private int _position;

        public Tokenizer(string expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _position = 0;
        }

        // Breaks down expression into number and operator tokens
        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            
            while (_position < _expression.Length)
            {
                SkipWhitespace();
                
                if (_position >= _expression.Length)
                    break;

                char current = _expression[_position];

                if (current == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParenthesis, _position));
                    _position++;
                }
                else if (current == ')')
                {
                    tokens.Add(new Token(TokenType.RightParenthesis, _position));
                    _position++;
                }
                else if (IsOperator(current))
                {
                    // Handle negative numbers (unary minus)
                    if (current == '-' && (tokens.Count == 0 || 
                        tokens.Last().Type == TokenType.Operator ||
                        tokens.Last().Type == TokenType.LeftParenthesis))
                    {
                        tokens.Add(ReadNumber(true));
                    }
                    // Support unary plus (e.g., "+5" or "+ 5")
                    else if (current == '+' && (tokens.Count == 0 || 
                        tokens.Last().Type == TokenType.Operator ||
                        tokens.Last().Type == TokenType.LeftParenthesis))
                    {
                        // Consume '+' and any following whitespace, then read number
                        _position++;
                        SkipWhitespace();

                        if (_position >= _expression.Length || !char.IsDigit(_expression[_position]))
                        {
                            throw new InvalidOperationException($"Invalid operator: '+'");
                        }

                        tokens.Add(ReadNumber(false));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Operator, _position, operatorSymbol: current.ToString()));
                        _position++;
                    }
                }
                else if (char.IsDigit(current) || current == '.')
                {
                    tokens.Add(ReadNumber(false));
                }
                else
                {
                    throw new InvalidOperationException($"Invalid character: '{current}'");
                }
            }

            tokens.Add(new Token(TokenType.End, _position));
            return tokens;
        }

        // Reads a number from current position (supports decimals)
        private Token ReadNumber(bool isNegative)
        {
            int startPos = _position;
            var sb = new StringBuilder();

            if (isNegative)
            {
                sb.Append('-');
                _position++;
                SkipWhitespace();
            }

            // Handle numbers starting with decimal point (e.g., ".5")
            if (_position >= _expression.Length || 
                (!char.IsDigit(_expression[_position]) && _expression[_position] != '.'))
            {
                throw new InvalidOperationException($"Expected digit at position {_position}");
            }

            bool hasDecimalPoint = false;
            bool hasDigits = false;

            // Read integer part and decimal part
            while (_position < _expression.Length)
            {
                char c = _expression[_position];
                
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    hasDigits = true;
                    _position++;
                }
                else if (c == '.' && !hasDecimalPoint)
                {
                    sb.Append(c);
                    hasDecimalPoint = true;
                    _position++;
                }
                else
                {
                    break;
                }
            }

            if (!hasDigits)
            {
                throw new InvalidOperationException($"Invalid number format at position {startPos}: no digits found");
            }

            string numberStr = sb.ToString();
            if (!decimal.TryParse(numberStr, out decimal value))
            {
                throw new InvalidOperationException($"Invalid number format at position {startPos}: '{numberStr}'");
            }

            return new Token(TokenType.Number, startPos, value: value);
        }

        // Skips spaces and tabs
        private void SkipWhitespace()
        {
            while (_position < _expression.Length && char.IsWhiteSpace(_expression[_position]))
            {
                _position++;
            }
        }

        // Checks if character is a math operator
        private static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }
    }
}
