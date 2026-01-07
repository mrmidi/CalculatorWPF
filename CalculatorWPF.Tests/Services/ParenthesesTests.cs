using System.Numerics;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class ParenthesesTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public ParenthesesTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        [Theory]
        [InlineData("(2 + 3)", 5)]
        [InlineData("(10)", 10)]
        [InlineData("(-5)", -5)]
        public void Evaluate_SimpleParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(2 + 3) * 4", 20)] // Without parens: 2 + 3 * 4 = 14
        [InlineData("2 * (3 + 4)", 14)] // Without parens: 2 * 3 + 4 = 10
        [InlineData("(10 - 5) / 5", 1)] // Without parens: 10 - 5 / 5 = 9
        public void Evaluate_ParenthesesChangePrecedence_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("((2 + 3))", 5)]
        [InlineData("(((10)))", 10)]
        [InlineData("((2 + 3) * 4)", 20)]
        public void Evaluate_NestedParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(2 + 3) * (4 + 5)", 45)] // 5 * 9 = 45
        [InlineData("(10 - 5) + (3 * 2)", 11)] // 5 + 6 = 11
        [InlineData("(8 / 4) * (6 - 2)", 8)] // 2 * 4 = 8
        public void Evaluate_MultipleParenthesesGroups_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("((2 + 3) * (4 + 5))", 45)]
        [InlineData("(2 * (3 + 4)) - 5", 9)]
        [InlineData("10 - (5 - (2 + 1))", 8)] // 10 - (5 - 3) = 10 - 2 = 8
        public void Evaluate_ComplexNestedParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(2 + 3) ^ 2", 25)] // (5) ^ 2 = 25
        [InlineData("2 ^ (3 + 1)", 16)] // 2 ^ 4 = 16
        [InlineData("(2 ^ 3) + 1", 9)] // 8 + 1 = 9
        public void Evaluate_ParenthesesWithPower_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_MismatchedParentheses_MissingOpeningParen_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate("2 + 3)"));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("no opening parenthesis", exception.Message);
        }

        [Fact]
        public void Evaluate_MismatchedParentheses_MissingClosingParen_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate("(2 + 3"));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("unclosed opening parenthesis", exception.Message);
        }

        [Theory]
        [InlineData("((2 + 3)")]
        [InlineData("(2 + (3 * 4)")]
        public void Evaluate_MismatchedParentheses_MultipleUnclosed_ThrowsException(string expression)
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate(expression));
            Assert.Contains("Mismatched parentheses", exception.Message);
        }

        [Theory]
        [InlineData("2 + 3))")]
        [InlineData("))2 + 3")]
        public void Evaluate_MismatchedParentheses_ExtraClosing_ThrowsException(string expression)
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _evaluator.Evaluate(expression));
            Assert.Contains("Mismatched parentheses", exception.Message);
        }

        [Theory]
        [InlineData("(-5 + 3)", -2)]
        [InlineData("(-2) * (-3)", 6)]
        public void Evaluate_NegativeNumbersInParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Theory]
        [InlineData("(  2  +  3  )", 5)]
        [InlineData("( 10 ) * ( 5 )", 50)]
        public void Evaluate_ParenthesesWithWhitespace_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }

        [Fact]
        public void Evaluate_VeryDeeplyNestedParentheses_ReturnsCorrectResult()
        {
            // Arrange
            string expression = "((((((2 + 3))))))";

            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(5, (long)result);
        }

        [Theory]
        [InlineData("(2 + 3) * (4 + 5) + (6 - 2)", 49)] // 5 * 9 + 4 = 49
        [InlineData("((2 + 3) * 4) + ((5 - 2) * 6)", 38)] // 20 + 18 = 38
        public void Evaluate_ComplexExpressionsWithParentheses_ReturnsCorrectResult(string expression, long expected)
        {
            // Act
            BigInteger result = _evaluator.Evaluate(expression);

            // Assert
            Assert.Equal(expected, (long)result);
        }
    }
}
