
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class EdgeCaseTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public EdgeCaseTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        [Theory]
        [InlineData("1 - 1", 0)]
        [InlineData("1 - -1", 2)]
        [InlineData("1 + -1", 0)]
        [InlineData("1 + 1", 2)]
        public void Evaluate_SubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("-1 - 1", -2)]
        [InlineData("-1 - -1", 0)]
        [InlineData("-1 + -1", -2)]
        [InlineData("-1 + 1", 0)]
        public void Evaluate_NegativeMinusEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("0 - 0", 0)]
        [InlineData("0 - 1", -1)]
        [InlineData("0 - -1", 1)]
        [InlineData("1 - 0", 1)]
        [InlineData("-1 - 0", -1)]
        public void Evaluate_ZeroSubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 - 1 - 1", 0)]
        [InlineData("2 - -1 - -1", 4)]
        [InlineData("5 - 3 - 2", 0)]
        [InlineData("5 - 3 - -2", 4)]
        public void Evaluate_ChainedSubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(1 - 1)", 0)]
        [InlineData("(1 - -1)", 2)]
        [InlineData("(1 - 1) * 5", 0)]
        [InlineData("(1 - -1) * 5", 10)]
        public void Evaluate_ParenthesesSubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("0 + 0", 0)]
        [InlineData("0 * 0", 0)]
        [InlineData("0 * 1", 0)]
        [InlineData("1 * 0", 0)]
        [InlineData("0 + 1", 1)]
        [InlineData("1 + 0", 1)]
        public void Evaluate_ZeroOperationsEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 * 1", 1)]
        [InlineData("1 * -1", -1)]
        [InlineData("-1 * 1", -1)]
        [InlineData("-1 * -1", 1)]
        public void Evaluate_OneMultiplicationEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 / 1", 1)]
        [InlineData("-1 / 1", -1)]
        [InlineData("1 / -1", -1)]
        [InlineData("-1 / -1", 1)]
        [InlineData("0 / 1", 0)]
        [InlineData("0 / -1", 0)]
        public void Evaluate_OneDivisionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 ^ 1", 1)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("-1 ^ 1", -1)]
        [InlineData("-1 ^ 2", 1)]
        [InlineData("-1 ^ 3", -1)]
        [InlineData("2 ^ 1", 2)]
        [InlineData("2 ^ 0", 1)]
        public void Evaluate_PowerEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(1)", 1)]
        [InlineData("(-1)", -1)]
        [InlineData("(0)", 0)]
        [InlineData("((1))", 1)]
        [InlineData("(((0)))", 0)]
        public void Evaluate_SingleNumberInParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 - 1 + 1", 1)]
        [InlineData("1 - -1 + 1", 3)]
        [InlineData("1 - 1 - 1", -1)]
        [InlineData("1 - -1 - -1", 3)]
        public void Evaluate_MixedAdditionSubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("10 - 5 - 3 - 2", 0)]
        [InlineData("10 - 5 - 3 - -2", 4)]
        [InlineData("1 - 2 - 3 - 4", -8)]
        [InlineData("1 - 2 - -3 - 4", -2)]
        public void Evaluate_LongSubtractionChains_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(1 - 1) ^ 2", 0)]
        [InlineData("(1 - -1) ^ 2", 4)]
        [InlineData("2 ^ (1 - 1)", 1)]
        [InlineData("2 ^ (2 - 1)", 2)]
        public void Evaluate_SubtractionWithPowerEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(1 - 1) * (1 - 1)", 0)]
        [InlineData("(1 - -1) * (1 - 1)", 0)]
        [InlineData("(1 - -1) * (1 - -1)", 4)]
        [InlineData("(2 - 1) * (2 - 1)", 1)]
        public void Evaluate_DoubleSubtractionMultiplication_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("100 - 99", 1)]
        [InlineData("100 - -99", 199)]
        [InlineData("1000 - 999", 1)]
        [InlineData("1000 - -999", 1999)]
        public void Evaluate_LargeNumberSubtractionEdgeCases_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }
    }
}
