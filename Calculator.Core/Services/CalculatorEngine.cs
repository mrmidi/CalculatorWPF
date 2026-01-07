using System.Numerics;

namespace CalculatorWPF.Services
{
    // Main calculator interface
    public class CalculatorEngine
    {
        private readonly ExpressionEvaluator _evaluator;

        public CalculatorEngine()
        {
            _evaluator = new ExpressionEvaluator();
        }

        // Calculates expression and returns result or error message
        public string Calculate(string expression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    return "Error - Expression cannot be empty";
                }

                BigInteger result = _evaluator.Evaluate(expression);
                return result.ToString();
            }
            catch (DivideByZeroException)
            {
                return "Error - Division by zero";
            }
            catch (InvalidOperationException ex)
            {
                return $"Error - {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error - {ex.Message}";
            }
        }

        // Checks if expression is valid without calculating
        public bool IsValidExpression(string expression, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    errorMessage = "Expression cannot be empty";
                    return false;
                }

                _evaluator.Evaluate(expression);
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
