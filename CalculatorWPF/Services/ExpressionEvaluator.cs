using System.Numerics;
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
        public BigInteger Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new InvalidOperationException("Expression cannot be empty");
            }

            // Tokenize the expression
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Validate parentheses matching
            RpnConverter.ValidateParentheses(tokens);

            // Convert to RPN
            var rpnTokens = _rpnConverter.ConvertToRpn(tokens);

            // Evaluate RPN expression
            return EvaluateRpn(rpnTokens);
        }

        // Evaluates an RPN expression
        private BigInteger EvaluateRpn(List<Token> rpnTokens)
        {
            var stack = new Stack<BigInteger>();

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
                    BigInteger right = stack.Pop();
                    BigInteger left = stack.Pop();

                    BigInteger result = token.Operator switch
                    {
                        "+" => BigInteger.Add(left, right),
                        "-" => BigInteger.Subtract(left, right),
                        "*" => BigInteger.Multiply(left, right),
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
        private BigInteger DivideWithCheck(BigInteger left, BigInteger right)
        {
            if (right == 0)
            {
                throw new DivideByZeroException("Division by zero");
            }
            return BigInteger.Divide(left, right);
        }

        // Computes power (exponentiation)
        private BigInteger Power(BigInteger baseValue, BigInteger exponent)
        {
            if (exponent < 0)
            {
                throw new InvalidOperationException("Negative exponents are not supported");
            }

            // BigInteger.Pow requires int exponent
            if (exponent > int.MaxValue)
            {
                throw new InvalidOperationException("Exponent is too large");
            }

            return BigInteger.Pow(baseValue, (int)exponent);
        }
    }
}
