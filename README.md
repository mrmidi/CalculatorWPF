# Calculator WPF

Integer calculator with WPF interface. Supports basic math operations and batch file processing.

## Features

- Basic operations: +, -, *, /
- Power/exponentiation: ^
- Parentheses for grouping: ( )
- Proper operator precedence using RPN (Reverse Polish Notation)
  - Parentheses (highest)
  - Exponentiation (^) - right associative
  - Multiplication and Division
  - Addition and Subtraction (lowest)
- Negative numbers
- Large number support (BigInteger)
- Batch file processing
- Async file operations

## Requirements

- .NET 10 SDK
- Windows (WPF requires Windows)

## Building

```bash
dotnet build
```

## Running

```bash
dotnet run --project CalculatorWPF/CalculatorWPF.csproj
```

Or open in Visual Studio and press F5.

## Testing

```bash
dotnet test
```

## Usage

### Calculator Tab

Enter an expression and click Calculate or press Enter.

Examples:
- `2 + 3 * 2` = 8
- `(2 + 3) * 2` = 10
- `2 ^ 3` = 8
- `2 ^ 3 ^ 2` = 512 (right associative: 2^(3^2))
- `(2 + 3) ^ 2` = 25
- `10 - 5` = 5
- `-3 * 4` = -12
- `((2 + 3) * 4) ^ 2 - 10` = 390

### Batch Processing Tab

1. Select input file (one expression per line)
2. Choose output file path
3. Click Process File

Input file example:
```
2 + 3 * 2
(2 + 3) * 2
2 ^ 3
10 - 5
-3 * 4
```

Output file:
```
8
10
8
5
-12
```

## Error Messages

- Division by zero: `Error - Division by zero`
- Invalid characters: `Error - Invalid character: 'x'`
- Decimals not supported: `Error - Decimal numbers are not supported`
- Empty expression: `Error - Expression cannot be empty`
- Mismatched parentheses: `Error - Mismatched parentheses: unclosed opening parenthesis`
- Negative exponents: `Error - Negative exponents are not supported`

## Project Structure

```
CalculatorWPF/
├── CalculatorWPF/
│   ├── Models/
│   │   └── Token.cs
│   ├── Services/
│   │   ├── Tokenizer.cs
│   │   ├── RpnConverter.cs
│   │   ├── ExpressionEvaluator.cs
│   │   ├── CalculatorEngine.cs
│   │   └── FileProcessor.cs
│   └── MainWindow.xaml
└── CalculatorWPF.Tests/
    └── Services/
        ├── TokenizerTests.cs
        ├── RpnConverterTests.cs
        ├── ExpressionEvaluatorTests.cs
        ├── ParenthesesTests.cs
        ├── ParenthesesValidationTests.cs
        ├── PowerOperatorTests.cs
        ├── ComplexExpressionTests.cs
        └── (other test files)
```

## Implementation Details

The calculator uses **Reverse Polish Notation (RPN)** with the **Shunting Yard algorithm** for proper order of operations:

1. **Tokenizer**: Converts input string into tokens (numbers, operators, parentheses)
2. **RpnConverter**: Converts infix notation to RPN using Shunting Yard algorithm
3. **ExpressionEvaluator**: Evaluates RPN expression using a stack-based approach

This approach ensures:
- Correct operator precedence
- Proper handling of parentheses
- Right-associativity for exponentiation
- Efficient evaluation
