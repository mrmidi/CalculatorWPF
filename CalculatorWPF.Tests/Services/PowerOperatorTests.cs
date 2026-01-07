using System.Numerics;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class PowerOperatorTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public PowerOperatorTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        [Theory]
        [InlineData("2 ^ 3", 8)]
        [InlineData("5 ^ 2", 25)]
        [InlineData("10 ^ 0", 1)]
        [InlineData("2 ^ 10", 1024)]
        public void Evaluate_SimplePower_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 ^ 3 ^ 2", 512)] // Right associative: 2 ^ (3 ^ 2) = 2 ^ 9 = 512
        [InlineData("2 ^ 2 ^ 3", 256)] // Right associative: 2 ^ (2 ^ 3) = 2 ^ 8 = 256
        public void Evaluate_PowerRightAssociative_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 + 3 ^ 2", 11)] // 2 + 9 = 11
        [InlineData("10 - 2 ^ 3", 2)] // 10 - 8 = 2
        [InlineData("3 * 2 ^ 2", 12)] // 3 * 4 = 12
        [InlineData("16 / 2 ^ 2", 4)] // 16 / 4 = 4
        public void Evaluate_PowerPrecedence_PowerHasHighestPrecedence(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("2 ^ 2 + 3", 7)] // 4 + 3 = 7
        [InlineData("2 ^ 2 * 3", 12)] // 4 * 3 = 12
        [InlineData("3 * 2 ^ 3 + 1", 25)] // 3 * 8 + 1 = 25
        public void Evaluate_PowerWithMultipleOperators_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_NegativeExponent_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate("2 ^ -3"));
            Assert.Contains("Negative exponents are not supported", exception.Message);
        }

        [Fact]
        public void Evaluate_ZeroToZero_ReturnsOne()
        {
            // Act
            BigInteger result = _evaluator.Evaluate("0 ^ 0");

            // Assert
            Assert.Equal(1, (long)result);
        }

        [Theory]
        [InlineData("0 ^ 5", 0)]
        [InlineData("0 ^ 100", 0)]
        public void Evaluate_ZeroToPositivePower_ReturnsZero(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("1 ^ 100", 1)]
        [InlineData("1 ^ 999999", 1)]
        public void Evaluate_OneToAnyPower_ReturnsOne(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_LargePower_ReturnsCorrectResult()
        {
            // Arrange
            string expression = "2 ^ 100";
            BigInteger expected = BigInteger.Parse("1267650600228229401496703205376");

            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-2 ^ 3", -8)]
        [InlineData("-2 ^ 2", 4)]
        [InlineData("-3 ^ 3", -27)]
        public void Evaluate_NegativeBasePower_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }
    }
}
