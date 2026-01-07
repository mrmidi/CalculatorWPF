using CalculatorWPF.Models;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class TokenizerTests
    {
        [Fact]
        public void Tokenize_SimpleAddition_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("5 + 3");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(4, tokens.Count); // 5, +, 3, End
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(5, (int)tokens[0].Value!.Value);
            Assert.Equal(TokenType.Operator, tokens[1].Type);
            Assert.Equal("+", tokens[1].Operator);
            Assert.Equal(TokenType.Number, tokens[2].Type);
            Assert.Equal(3, (int)tokens[2].Value!.Value);
            Assert.Equal(TokenType.End, tokens[3].Type);
        }

        [Fact]
        public void Tokenize_NegativeNumber_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("-5 + 3");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(-5, (int)tokens[0].Value!.Value);
            Assert.Equal(TokenType.Operator, tokens[1].Type);
            Assert.Equal("+", tokens[1].Operator);
        }

        [Fact]
        public void Tokenize_AllOperators_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("1 + 2 - 3 * 4 / 5");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(10, tokens.Count); // 1, +, 2, -, 3, *, 4, /, 5, End
            Assert.All(new[] { 0, 2, 4, 6, 8 }, i => Assert.Equal(TokenType.Number, tokens[i].Type));
            Assert.All(new[] { 1, 3, 5, 7 }, i => Assert.Equal(TokenType.Operator, tokens[i].Type));
        }

        [Fact]
        public void Tokenize_InvalidCharacter_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("5 + a");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => tokenizer.Tokenize());
            Assert.Contains("Invalid character", exception.Message);
            Assert.Contains("'a'", exception.Message);
        }

        [Fact]
        public void Tokenize_DecimalNumber_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("2.5 + 3");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(4, tokens.Count); // 2.5, +, 3, EOF
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(2.5m, tokens[0].Value);
        }

        [Fact]
        public void Tokenize_LargeNumber_ReturnsCorrectToken()
        {
            // Arrange
            var tokenizer = new Tokenizer("999999999999999999999");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(2, tokens.Count); // Number, End
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal("999999999999999999999", tokens[0].Value!.Value.ToString());
        }

        [Fact]
        public void Tokenize_WhitespaceHandling_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("  5  +   3  ");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(4, tokens.Count);
            Assert.Equal(5, (int)tokens[0].Value!.Value);
            Assert.Equal(3, (int)tokens[2].Value!.Value);
        }

        [Fact]
        public void Tokenize_PowerOperator_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("2 ^ 3");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(4, tokens.Count); // 2, ^, 3, End
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(2, (int)tokens[0].Value!.Value);
            Assert.Equal(TokenType.Operator, tokens[1].Type);
            Assert.Equal("^", tokens[1].Operator);
            Assert.Equal(TokenType.Number, tokens[2].Type);
            Assert.Equal(3, (int)tokens[2].Value!.Value);
            Assert.Equal(TokenType.End, tokens[3].Type);
        }

        [Fact]
        public void Tokenize_Parentheses_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("(2 + 3)");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(6, tokens.Count); // (, 2, +, 3, ), End
            Assert.Equal(TokenType.LeftParenthesis, tokens[0].Type);
            Assert.Equal(TokenType.Number, tokens[1].Type);
            Assert.Equal(2, (int)tokens[1].Value!.Value);
            Assert.Equal(TokenType.Operator, tokens[2].Type);
            Assert.Equal("+", tokens[2].Operator);
            Assert.Equal(TokenType.Number, tokens[3].Type);
            Assert.Equal(3, (int)tokens[3].Value!.Value);
            Assert.Equal(TokenType.RightParenthesis, tokens[4].Type);
            Assert.Equal(TokenType.End, tokens[5].Type);
        }

        [Fact]
        public void Tokenize_NestedParentheses_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("((2))");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(6, tokens.Count); // (, (, 2, ), ), End
            Assert.Equal(TokenType.LeftParenthesis, tokens[0].Type);
            Assert.Equal(TokenType.LeftParenthesis, tokens[1].Type);
            Assert.Equal(TokenType.Number, tokens[2].Type);
            Assert.Equal(TokenType.RightParenthesis, tokens[3].Type);
            Assert.Equal(TokenType.RightParenthesis, tokens[4].Type);
            Assert.Equal(TokenType.End, tokens[5].Type);
        }

        [Fact]
        public void Tokenize_NegativeNumberAfterLeftParenthesis_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("(-5 + 3)");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(6, tokens.Count); // (, -5, +, 3, ), End
            Assert.Equal(TokenType.LeftParenthesis, tokens[0].Type);
            Assert.Equal(TokenType.Number, tokens[1].Type);
            Assert.Equal(-5, (int)tokens[1].Value!.Value);
            Assert.Equal(TokenType.Operator, tokens[2].Type);
            Assert.Equal("+", tokens[2].Operator);
        }

        [Fact]
        public void Tokenize_AllOperatorsWithPower_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("1 + 2 - 3 * 4 / 5 ^ 6");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(12, tokens.Count); // 1, +, 2, -, 3, *, 4, /, 5, ^, 6, End
            Assert.All(new[] { 0, 2, 4, 6, 8, 10 }, i => Assert.Equal(TokenType.Number, tokens[i].Type));
            Assert.All(new[] { 1, 3, 5, 7, 9 }, i => Assert.Equal(TokenType.Operator, tokens[i].Type));
            Assert.Equal("^", tokens[9].Operator);
        }

        [Fact]
        public void Tokenize_ComplexExpression_ReturnsCorrectTokens()
        {
            // Arrange
            var tokenizer = new Tokenizer("(2 + 3) * 4 ^ 2");

            // Act
            var tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equal(10, tokens.Count); // (, 2, +, 3, ), *, 4, ^, 2, End
            Assert.Equal(TokenType.LeftParenthesis, tokens[0].Type);
            Assert.Equal(TokenType.Number, tokens[1].Type);
            Assert.Equal(TokenType.Operator, tokens[2].Type);
            Assert.Equal(TokenType.Number, tokens[3].Type);
            Assert.Equal(TokenType.RightParenthesis, tokens[4].Type);
            Assert.Equal(TokenType.Operator, tokens[5].Type);
            Assert.Equal("*", tokens[5].Operator);
            Assert.Equal(TokenType.Number, tokens[6].Type);
            Assert.Equal(TokenType.Operator, tokens[7].Type);
            Assert.Equal("^", tokens[7].Operator);
            Assert.Equal(TokenType.Number, tokens[8].Type);
        }
    }
}
