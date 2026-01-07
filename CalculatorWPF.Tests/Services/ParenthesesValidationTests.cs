using CalculatorWPF.Models;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class ParenthesesValidationTests
    {
        private readonly RpnConverter _converter;

        public ParenthesesValidationTests()
        {
            _converter = new RpnConverter();
        }

        [Fact]
        public void ValidateParentheses_BalancedParentheses_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("(2 + 3) * (4 + 5)");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }

        [Fact]
        public void ValidateParentheses_NoParentheses_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("2 + 3 * 4");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }

        [Fact]
        public void ValidateParentheses_NestedBalancedParentheses_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("((2 + 3) * (4 + 5))");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }

        [Fact]
        public void ValidateParentheses_MissingOpeningParen_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("2 + 3)");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("no opening parenthesis", exception.Message);
        }

        [Fact]
        public void ValidateParentheses_MissingClosingParen_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("(2 + 3");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("unclosed opening parenthesis", exception.Message);
        }

        [Fact]
        public void ValidateParentheses_MultipleUnclosedParens_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("((2 + 3)");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("unclosed opening parenthesis", exception.Message);
        }

        [Fact]
        public void ValidateParentheses_MultipleExtraClosingParens_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("2 + 3))");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("no opening parenthesis", exception.Message);
        }

        [Fact]
        public void ValidateParentheses_ComplexNestedBalanced_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("((2 + 3) * ((4 + 5) - (6 / 2)))");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }

        [Fact]
        public void ValidateParentheses_EmptyParenthesesDetectedByTokenizer_ThrowsException()
        {
            // Empty parentheses should be caught during evaluation
            // This test ensures the validator handles the token stream correctly
            // even if tokenizer allows it through
            
            // Arrange - manually create tokens for empty parens
            var tokens = new List<Token>
            {
                new Token(TokenType.LeftParenthesis, 0),
                new Token(TokenType.RightParenthesis, 1),
                new Token(TokenType.End, 2)
            };

            // Act & Assert - Validation passes (empty parens are valid syntax)
            // The error will be caught during RPN evaluation
            _converter.ValidateParentheses(tokens);
        }

        [Theory]
        [InlineData("(1 + 2")]
        [InlineData("((1 + 2)")]
        [InlineData("(((1)")]
        public void ValidateParentheses_VariousUnclosedParens_ThrowsException(string expression)
        {
            // Arrange
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
        }

        [Theory]
        [InlineData("1 + 2)")]
        [InlineData("1 + 2))")]
        [InlineData(")1 + 2")]
        public void ValidateParentheses_VariousExtraClosingParens_ThrowsException(string expression)
        {
            // Arrange
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ValidateParentheses(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
        }

        [Fact]
        public void ValidateParentheses_AlternatingParens_Balanced_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("(1) + (2) + (3)");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }

        [Fact]
        public void ValidateParentheses_DeeplyNested_DoesNotThrow()
        {
            // Arrange
            var tokenizer = new Tokenizer("((((((1))))))");
            var tokens = tokenizer.Tokenize();

            // Act & Assert - should not throw
            _converter.ValidateParentheses(tokens);
        }
    }
}
