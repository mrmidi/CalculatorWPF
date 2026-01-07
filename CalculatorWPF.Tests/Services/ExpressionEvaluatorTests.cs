
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class ExpressionEvaluatorTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public ExpressionEvaluatorTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        [Theory]
        [InlineData("5 + 3", 8)]
        [InlineData("10 - 4", 6)]
        [InlineData("6 * 7", 42)]
        [InlineData("20 / 4", 5)]
        public void Evaluate_SimpleOperations_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 + 3 * 2", 8)]
        [InlineData("10 + 2 * 6", 22)]
        [InlineData("100 * 2 + 12", 212)]
        public void Evaluate_OperatorPrecedence_MultipliesFirst(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("10 / 2 + 3", 8)]
        [InlineData("20 - 10 / 2", 15)]
        [InlineData("100 / 2 / 5", 10)]
        public void Evaluate_OperatorPrecedence_DividesFirst(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_EmptyExpression_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate(""));
        }

        [Fact]
        public void Evaluate_DivisionByZero_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _evaluator.Evaluate("5 / 0"));
        }

        [Fact]
        public void Evaluate_LargeNumbers_ReturnsCorrectResult()
        {
            // Arrange
            string expression = "999999999999999999999 + 1";
            decimal expected = decimal.Parse("1000000000000000000000");

            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-5 + 3", -2)]
        [InlineData("-10 - -5", -5)]
        [InlineData("-2 * -3", 6)]
        public void Evaluate_NegativeNumbers_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 + 2 + 3 + 4", 10)]
        [InlineData("2 * 3 * 4", 24)]
        [InlineData("100 - 10 - 5", 85)]
        public void Evaluate_MultipleOperations_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 + 2 * 3 - 4 / 2", 5)]
        [InlineData("10 * 2 + 5 * 3 - 8 / 4", 33)]
        public void Evaluate_ComplexExpression_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }
    }
}
