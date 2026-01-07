using CalculatorWPF.Models;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class RpnConverterTests
    {
        private readonly RpnConverter _converter;

        public RpnConverterTests()
        {
            _converter = new RpnConverter();
        }

        [Fact]
        public void ConvertToRpn_SimpleAddition_ReturnsCorrectRpn()
        {
            // Arrange: "2 + 3"
            var tokenizer = new Tokenizer("2 + 3");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 +"
            Assert.Equal(3, rpn.Count);
            Assert.Equal(TokenType.Number, rpn[0].Type);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(TokenType.Number, rpn[1].Type);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal(TokenType.Operator, rpn[2].Type);
            Assert.Equal("+", rpn[2].Operator);
        }

        [Fact]
        public void ConvertToRpn_WithPrecedence_ReturnsCorrectRpn()
        {
            // Arrange: "2 + 3 * 4"
            var tokenizer = new Tokenizer("2 + 3 * 4");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 4 * +"
            Assert.Equal(5, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal(4, (int)rpn[2].Value!.Value);
            Assert.Equal("*", rpn[3].Operator);
            Assert.Equal("+", rpn[4].Operator);
        }

        [Fact]
        public void ConvertToRpn_WithParentheses_ReturnsCorrectRpn()
        {
            // Arrange: "(2 + 3) * 4"
            var tokenizer = new Tokenizer("(2 + 3) * 4");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 + 4 *"
            Assert.Equal(5, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal("+", rpn[2].Operator);
            Assert.Equal(4, (int)rpn[3].Value!.Value);
            Assert.Equal("*", rpn[4].Operator);
        }

        [Fact]
        public void ConvertToRpn_WithPowerOperator_ReturnsCorrectRpn()
        {
            // Arrange: "2 ^ 3"
            var tokenizer = new Tokenizer("2 ^ 3");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 ^"
            Assert.Equal(3, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal("^", rpn[2].Operator);
        }

        [Fact]
        public void ConvertToRpn_PowerRightAssociative_ReturnsCorrectRpn()
        {
            // Arrange: "2 ^ 3 ^ 2" should be "2 ^ (3 ^ 2)"
            var tokenizer = new Tokenizer("2 ^ 3 ^ 2");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 2 ^ ^"
            Assert.Equal(5, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal(2, (int)rpn[2].Value!.Value);
            Assert.Equal("^", rpn[3].Operator);
            Assert.Equal("^", rpn[4].Operator);
        }

        [Fact]
        public void ConvertToRpn_PowerWithMultiplication_ReturnsCorrectRpn()
        {
            // Arrange: "2 * 3 ^ 2" (power has higher precedence)
            var tokenizer = new Tokenizer("2 * 3 ^ 2");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 2 ^ *"
            Assert.Equal(5, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal(2, (int)rpn[2].Value!.Value);
            Assert.Equal("^", rpn[3].Operator);
            Assert.Equal("*", rpn[4].Operator);
        }

        [Fact]
        public void ConvertToRpn_ComplexExpression_ReturnsCorrectRpn()
        {
            // Arrange: "3 + 4 * 2 / (1 - 5) ^ 2"
            var tokenizer = new Tokenizer("3 + 4 * 2 / (1 - 5) ^ 2");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "3 4 2 * 1 5 - 2 ^ / +"
            // Verify the sequence
            var expectedSequence = new[] { "3", "4", "2", "*", "1", "5", "-", "2", "^", "/", "+" };
            Assert.Equal(expectedSequence.Length, rpn.Count);
            
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                if (rpn[i].Type == TokenType.Number)
                {
                    Assert.Equal(expectedSequence[i], rpn[i].Value!.Value.ToString());
                }
                else if (rpn[i].Type == TokenType.Operator)
                {
                    Assert.Equal(expectedSequence[i], rpn[i].Operator);
                }
            }
        }

        [Fact]
        public void ConvertToRpn_NestedParentheses_ReturnsCorrectRpn()
        {
            // Arrange: "((2 + 3) * 4)"
            var tokenizer = new Tokenizer("((2 + 3) * 4)");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 + 4 *"
            Assert.Equal(5, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal("+", rpn[2].Operator);
            Assert.Equal(4, (int)rpn[3].Value!.Value);
            Assert.Equal("*", rpn[4].Operator);
        }

        [Fact]
        public void ConvertToRpn_AllOperators_ReturnsCorrectRpn()
        {
            // Arrange: "1 + 2 - 3 * 4 / 5 ^ 2"
            var tokenizer = new Tokenizer("1 + 2 - 3 * 4 / 5 ^ 2");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "1 2 + 3 4 * 5 2 ^ / -"
            // Power highest, then * /, then + -
            Assert.Equal(11, rpn.Count);
            Assert.Equal(1, (int)rpn[0].Value!.Value);
            Assert.Equal(2, (int)rpn[1].Value!.Value);
            Assert.Equal("+", rpn[2].Operator);
            Assert.Equal(3, (int)rpn[3].Value!.Value);
            Assert.Equal(4, (int)rpn[4].Value!.Value);
            Assert.Equal("*", rpn[5].Operator);
            Assert.Equal(5, (int)rpn[6].Value!.Value);
            Assert.Equal(2, (int)rpn[7].Value!.Value);
            Assert.Equal("^", rpn[8].Operator);
            Assert.Equal("/", rpn[9].Operator);
            Assert.Equal("-", rpn[10].Operator);
        }

        [Fact]
        public void ConvertToRpn_MismatchedParentheses_UnclosedOpen_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("(2 + 3");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ConvertToRpn(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("unclosed opening parenthesis", exception.Message);
        }

        [Fact]
        public void ConvertToRpn_MismatchedParentheses_ExtraClosing_ThrowsException()
        {
            // Arrange
            var tokenizer = new Tokenizer("2 + 3)");
            var tokens = tokenizer.Tokenize();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _converter.ConvertToRpn(tokens));
            Assert.Contains("Mismatched parentheses", exception.Message);
            Assert.Contains("no opening parenthesis", exception.Message);
        }

        [Fact]
        public void ConvertToRpn_MultipleParenthesesGroups_ReturnsCorrectRpn()
        {
            // Arrange: "(2 + 3) * (4 + 5)"
            var tokenizer = new Tokenizer("(2 + 3) * (4 + 5)");
            var tokens = tokenizer.Tokenize();

            // Act
            var rpn = _converter.ConvertToRpn(tokens);

            // Assert: should be "2 3 + 4 5 + *"
            Assert.Equal(7, rpn.Count);
            Assert.Equal(2, (int)rpn[0].Value!.Value);
            Assert.Equal(3, (int)rpn[1].Value!.Value);
            Assert.Equal("+", rpn[2].Operator);
            Assert.Equal(4, (int)rpn[3].Value!.Value);
            Assert.Equal(5, (int)rpn[4].Value!.Value);
            Assert.Equal("+", rpn[5].Operator);
            Assert.Equal("*", rpn[6].Operator);
        }
    }
}
