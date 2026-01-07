using CalculatorWPF.Models;

namespace CalculatorWPF.Services
{
    // Evaluates expressions using Reverse Polish Notation (RPN)
    public class ExpressionEvaluator
    {
        private readonly RpnConverter _rpnConverter;

        public ExpressionEvaluator()
        {
            _rpnConverter = new RpnConverter();
        }

        // Evaluates expression and returns result
        public decimal Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new InvalidOperationException("Expression cannot be empty");
            }

            // Tokenize the expression
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Validate parentheses matching
            _rpnConverter.ValidateParentheses(tokens);

            // Convert to RPN
            var rpnTokens = _rpnConverter.ConvertToRpn(tokens);

            // Evaluate RPN expression
            return EvaluateRpn(rpnTokens);
        }

        // Evaluates an RPN expression
        private decimal EvaluateRpn(List<Token> rpnTokens)
        {
            var stack = new Stack<decimal>();

            foreach (var token in rpnTokens)
            {
                if (token.Type == TokenType.Number)
                {
                    stack.Push(token.Value!.Value);
                }
                else if (token.Type == TokenType.Operator)
                {
                    if (stack.Count < 2)
                    {
                        throw new InvalidOperationException($"Invalid expression: not enough operands for operator '{token.Operator}' at position {token.Position}");
                    }

                    // Note: Pop order matters - second operand comes first
                    decimal right = stack.Pop();
                    decimal left = stack.Pop();

                    decimal result = token.Operator switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => DivideWithCheck(left, right),
                        "^" => Power(left, right),
                        _ => throw new InvalidOperationException($"Unknown operator: {token.Operator}")
                    };

                    stack.Push(result);
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("Invalid expression: too many operands");
            }

            return stack.Pop();
        }

        // Performs division with zero check
        private decimal DivideWithCheck(decimal left, decimal right)
        {
            if (right == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }
            return left / right;
        }

        // Computes power (exponentiation)
        private decimal Power(decimal baseValue, decimal exponent)
        {
            // For decimal, we need to handle fractional exponents differently
            // We'll use Math.Pow and convert to/from double
            // Note: This may lose some precision for very large numbers
            
            if (baseValue == 0 && exponent < 0)
            {
                throw new InvalidOperationException("Cannot raise zero to a negative power");
            }

            try
            {
                double result = Math.Pow((double)baseValue, (double)exponent);
                
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    throw new InvalidOperationException("Power operation resulted in overflow or invalid result");
                }

                return (decimal)result;
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException("Power operation resulted in overflow");
            }
        }
    }
}
