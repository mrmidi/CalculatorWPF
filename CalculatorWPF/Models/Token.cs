using System.Numerics;

namespace CalculatorWPF.Models
{
    /// <summary>
    /// Represents a token in a mathematical expression
    /// </summary>
    public class Token
    {
        public TokenType Type { get; set; }
        public BigInteger? Value { get; set; }
        public string? Operator { get; set; }
        public int Position { get; set; }

        public Token(TokenType type, int position, BigInteger? value = null, string? operatorSymbol = null)
        {
            Type = type;
            Position = position;
            Value = value;
            Operator = operatorSymbol;
        }
    }

    /// <summary>
    /// Types of tokens that can appear in an expression
    /// </summary>
    public enum TokenType
    {
        Number,
        Operator,
        End
    }
}
