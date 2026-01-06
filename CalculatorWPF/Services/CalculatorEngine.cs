using System.Numerics;

namespace CalculatorWPF.Services
{
    /// <summary>
    /// Main calculator engine that provides a high-level interface for calculations
    /// </summary>
    public class CalculatorEngine
    {
        private readonly ExpressionEvaluator _evaluator;

        public CalculatorEngine()
        {
            _evaluator = new ExpressionEvaluator();
        }

        /// <summary>
        /// Calculates the result of an expression
        /// </summary>
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

        /// <summary>
        /// Validates if an expression is valid without evaluating it
        /// </summary>
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
