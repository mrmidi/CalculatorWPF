namespace CalculatorWPF.Models
{
    // Represents a single element in an expression (number, operator, or end marker)
    public class Token
    {
        public TokenType Type { get; set; }
        public decimal? Value { get; set; }
        public string? Operator { get; set; }
        public int Position { get; set; }

        public Token(TokenType type, int position, decimal? value = null, string? operatorSymbol = null)
        {
            Type = type;
            Position = position;
            Value = value;
            Operator = operatorSymbol;
        }
    }

    // Token types
    public enum TokenType
    {
        Number,
        Operator,
        LeftParenthesis,
        RightParenthesis,
        End
    }
}
