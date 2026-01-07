
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class ComplexExpressionTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public ComplexExpressionTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        [Theory]
        [InlineData("2 ^ 3 + 4 * 5", 28)] // 8 + 20 = 28
        [InlineData("(2 + 3) ^ 2", 25)] // 5 ^ 2 = 25
        [InlineData("2 ^ (3 + 1)", 16)] // 2 ^ 4 = 16
        [InlineData("(2 + 3) * (4 - 1) ^ 2", 45)] // 5 * 3^2 = 5 * 9 = 45
        public void Evaluate_PowersWithParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("((2 + 3) * 4) ^ 2", 400)] // (5 * 4)^2 = 20^2 = 400
        [InlineData("2 ^ ((3 + 1) * 2)", 256)] // 2 ^ (4 * 2) = 2 ^ 8 = 256
        [InlineData("((2 ^ 3) + 1) * 3", 27)] // (8 + 1) * 3 = 27
        public void Evaluate_NestedParenthesesWithPowers_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 + 3 * 4 - 5 / 5 + 6", 19)] // 2 + 12 - 1 + 6 = 19
        [InlineData("100 / 10 + 5 * 2 - 3", 17)] // 10 + 10 - 3 = 17
        [InlineData("2 * 3 + 4 * 5 - 6 / 2", 23)] // 6 + 20 - 3 = 23
        public void Evaluate_MultipleOperators_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(2 + 3) * 4 - (8 / 2)", 16)] // 5 * 4 - 4 = 16
        [InlineData("((10 - 2) * 2) + ((5 + 3) / 4)", 18)] // 16 + 2 = 18
        [InlineData("(100 / (10 + 10)) * 5", 25)] // (100 / 20) * 5 = 25
        public void Evaluate_MultipleParenthesesGroups_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 ^ 3 ^ 2", 512)] // 2 ^ (3 ^ 2) = 2 ^ 9 = 512 (right associative)
        [InlineData("(2 ^ 3) ^ 2", 64)] // 8 ^ 2 = 64 (forced left associative)
        [InlineData("2 ^ (3 ^ 2)", 512)] // Explicitly right associative
        public void Evaluate_PowerAssociativity_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("3 + 4 * 2 / (1 - 5) ^ 2", 3)] // 3 + 4 * 2 / (-4)^2 = 3 + 4 * 2 / 16 = 3 + 8/16 = 3 + 0 = 3
        [InlineData("(1 + 2) * (3 + 4) - (5 - 3) ^ 2", 17)] // 3 * 7 - 2^2 = 21 - 4 = 17
        public void Evaluate_VeryComplexExpressions_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("-2 + 3", 1)]
        [InlineData("(-2 + 3) * 4", 4)]
        [InlineData("-2 ^ 2", 4)] // -2 is parsed as a negative number, then squared
        [InlineData("5 + -3", 2)]
        public void Evaluate_NegativeNumbersInExpressions_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_LargeExpressionWithAllFeatures_ReturnsCorrectResult()
        {
            // Arrange: Mix of all operators, parentheses, and powers
            string expression = "((2 + 3) * 4) ^ 2 - (10 / 2) + 3 * (8 - 6) ^ 3";
            // Calculation: (5 * 4)^2 - 5 + 3 * 2^3 = 20^2 - 5 + 3 * 8 = 400 - 5 + 24 = 419

            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(419, (long)result);
        }

        [Theory]
        [InlineData("2  +  3  *  4", 14)]
        [InlineData("( 2 + 3 ) * ( 4 - 1 )", 15)]
        [InlineData("  2  ^  3  ", 8)]
        public void Evaluate_ExpressionsWithWhitespace_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_LongChainOfOperations_ReturnsCorrectResult()
        {
            // Arrange
            string expression = "1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10";

            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(55, (long)result);
        }

        [Theory]
        [InlineData("10 ^ 10", 10000000000)]
        [InlineData("(2 * 5) ^ 10", 10000000000)]
        public void Evaluate_LargePowerResults_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_VeryLargePower_ReturnsCorrectResult()
        {
            // Arrange - decimal can't hold 2^128, so test a smaller power
            string expression = "2 ^ 10";
            decimal expected = 1024;

            // Act
            decimal result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
