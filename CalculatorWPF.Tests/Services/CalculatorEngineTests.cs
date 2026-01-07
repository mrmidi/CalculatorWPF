using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class CalculatorEngineTests
    {
        private readonly CalculatorEngine _calculator;

        public CalculatorEngineTests()
        {
            _calculator = new CalculatorEngine();
        }

        [Theory]
        [InlineData("5 + 3", "8")]
        [InlineData("10 - 4", "6")]
        [InlineData("6 * 7", "42")]
        [InlineData("20 / 4", "5")]
        [InlineData("15 / 4", "3.75")] // Decimal division
        public void Calculate_SimpleOperations_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("2 + 3 * 2", "8")] // From specification
        [InlineData("10 + 2 * 6", "22")]
        [InlineData("100 * 2 + 12", "212")]
        [InlineData("100 * 2 + 12 / 3", "204")]
        [InlineData("10 - 2 * 3", "4")]
        public void Calculate_OperatorPrecedence_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-5 + 3", "-2")]
        [InlineData("2 + -3 * 2", "-4")] // From specification (similar pattern)
        [InlineData("-10 - -5", "-5")]
        [InlineData("-2 * -3", "6")]
        [InlineData("-20 / 4", "-5")]
        [InlineData("10 + -5", "5")]
        public void Calculate_NegativeNumbers_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Calculate_LargeNumbers_ReturnsCorrectResult()
        {
            // Arrange - numbers that exceed long.MaxValue
            string expression = "999999999999999999999 + 1";
            string expected = "1000000000000000000000";

            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Calculate_LargeMultiplication_ReturnsCorrectResult()
        {
            // Arrange
            string expression = "123456789 * 2";
            string expected = "246913578";

            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("5 / 0")]
        [InlineData("10 + 5 / 0")]
        public void Calculate_DivisionByZero_ReturnsError(string expression)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.StartsWith("Error - Division by zero", result);
        }

        [Theory]
        [InlineData("a + 1")]
        [InlineData("5 + b")]
        [InlineData("abc")]
        public void Calculate_InvalidCharacters_ReturnsError(string expression)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.StartsWith("Error - Invalid character", result);
        }

        [Theory]
        [InlineData("2.1 * 2", "4.2")]
        [InlineData("5.5 + 3", "8.5")]
        [InlineData("10 / 2.5", "4")]
        public void Calculate_DecimalNumbers_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Calculate_EmptyExpression_ReturnsError(string expression)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.StartsWith("Error", result);
        }

        [Theory]
        [InlineData("5 +")]
        [InlineData("* 3")]
        public void Calculate_InvalidSyntax_ReturnsError(string expression)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.StartsWith("Error", result);
        }

        [Theory]
        [InlineData("+5", "5")]
        [InlineData("10 + + 5", "15")]
        [InlineData("+ 7", "7")]
        public void Calculate_UnaryPlus_Supported_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("  5  +  3  ", "8")]
        [InlineData("10-4", "6")]
        [InlineData("  2 + 3 * 2  ", "8")]
        public void Calculate_WhitespaceHandling_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("5 + 3", "8")]
        [InlineData("invalid", "Error")]
        public void IsValidExpression_ValidatesCorrectly(string expression, string expectedPrefix)
        {
            // Act
            bool isValid = _calculator.IsValidExpression(expression, out string errorMessage);

            // Assert
            if (expectedPrefix == "Error")
            {
                Assert.False(isValid);
                Assert.NotEmpty(errorMessage);
            }
            else
            {
                Assert.True(isValid);
                Assert.Empty(errorMessage);
            }
        }

        [Theory]
        [InlineData("100 / 3", "33.333333333333333333333333333")]
        [InlineData("7 / 2", "3.5")]
        [InlineData("2 / 3", "0.6666666666666666666666666667")]
        [InlineData("-7 / 2", "-3.5")]
        public void Calculate_DecimalDivision_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1 + 2 + 3 + 4 + 5", "15")]
        [InlineData("100 - 10 - 5 - 3", "82")]
        [InlineData("2 * 3 * 4 * 5", "120")]
        [InlineData("1000 / 10 / 5 / 2", "10")]
        public void Calculate_MultipleOperations_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1 + 2 * 3 + 4 * 5", "27")]
        [InlineData("10 + 20 * 30 - 40 / 2", "590")]
        [InlineData("5 * 6 - 7 * 8 + 9", "-17")]
        public void Calculate_ComplexExpressions_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            string result = _calculator.Calculate(expression);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
