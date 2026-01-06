# Integer Calculator - WPF Application

A modern, feature-rich integer calculator built with WPF and .NET 8, supporting both interactive calculations and batch file processing.

## Features

### Core Functionality
- ✅ **Integer Arithmetic Operations**: Addition (+), Subtraction (-), Multiplication (*), Division (/)
- ✅ **Operator Precedence**: Multiplication and division evaluated before addition and subtraction
  - Example: `2 + 3 * 2 = 8`
- ✅ **Negative Numbers**: Full support for negative integers
  - Example: `2 + -3 * 2 = -4`
- ✅ **Large Numbers**: Support for arbitrarily large integers using BigInteger
  - Can handle numbers far exceeding `long.MaxValue`

### User Interface
- 🎨 **Modern Design**: Clean, intuitive interface with Material Design-inspired styling
- 📊 **Dual Modes**: 
  - Interactive calculator for single expressions
  - Batch file processor for multiple expressions
- 🔄 **Async Operations**: Non-blocking UI during file processing
- 📈 **Progress Tracking**: Real-time progress bar for batch operations

### Batch Processing
- 📁 **File Input/Output**: Process multiple expressions from text files
- 🔍 **Error Handling**: Invalid expressions produce descriptive error messages
- 📝 **Empty Line Handling**: Empty lines in input are preserved in output
- 💾 **Flexible Output**: User-specified output directory and filename

## Technical Implementation

### Object-Oriented Design

The application follows solid OOP principles with clear separation of concerns:

#### Models
- **Token**: Represents individual elements in mathematical expressions
- **TokenType**: Enum defining token types (Number, Operator, End)

#### Services
- **Tokenizer**: Converts expression strings into token sequences
- **ExpressionEvaluator**: Evaluates tokenized expressions with proper precedence
- **CalculatorEngine**: High-level interface for calculations
- **FileProcessor**: Asynchronous batch file processing

### Key Technologies
- **Framework**: .NET 8 with WPF
- **Language**: C# with nullable reference types enabled
- **Async/Await**: All long-running operations are asynchronous
- **BigInteger**: Support for arbitrarily large numbers
- **Error Handling**: Comprehensive exception handling throughout

### Architecture Highlights

1. **Recursive Descent Parser**: Implements operator precedence through grammar rules
   - `Expression` → handles addition/subtraction
   - `Term` → handles multiplication/division  
   - `Factor` → handles numbers

2. **Tokenization**: Separates lexical analysis from parsing for better maintainability

3. **Async Processing**: File operations use `async/await` to keep UI responsive

## Usage

### Interactive Calculator

1. Launch the application
2. Navigate to the "Calculator" tab
3. Enter an expression (e.g., `2 + 3 * 2`)
4. Click "Calculate" or press Enter
5. View the result

### Batch File Processing

1. Navigate to the "Batch File Processing" tab
2. Select an input file containing expressions (one per line)
3. Specify an output file path
4. Click "Process File"
5. Monitor progress via the progress bar
6. Review results in the output file

### Input File Format

```text
2 + 3 * 2
10 - 5
-3 * 4
100 / 3
invalid expression
```

### Output File Format

```text
8
5
-12
33
Error - Invalid character: 'i'
```

## Error Handling

The application handles various error scenarios gracefully:

- **Division by Zero**: `Error - Division by zero`
- **Invalid Characters**: `Error - Invalid character: 'a'`
- **Decimal Numbers**: `Error - Decimal numbers are not supported`
- **Invalid Syntax**: `Error - Expected digit at position X`
- **Empty Expressions**: `Error - Expression cannot be empty`

## Testing

Comprehensive unit tests are included covering:

- ✅ Basic arithmetic operations
- ✅ Operator precedence rules
- ✅ Negative number handling
- ✅ Large number support
- ✅ Error conditions
- ✅ Edge cases
- ✅ File processing
- ✅ Tokenization
- ✅ Expression evaluation

### Running Tests

```bash
cd CalculatorWPF
dotnet test
```

## Building and Running

### Prerequisites
- .NET 8 SDK
- Windows OS (for WPF)

### Build
```bash
cd CalculatorWPF
dotnet build
```

### Run
```bash
cd CalculatorWPF
dotnet run --project CalculatorWPF/CalculatorWPF.csproj
```

Or simply open the solution in Visual Studio 2022 and press F5.

## Project Structure

```
CalculatorWPF/
├── CalculatorWPF/              # Main application
│   ├── Models/                 # Data models
│   │   └── Token.cs
│   ├── Services/               # Business logic
│   │   ├── Tokenizer.cs
│   │   ├── ExpressionEvaluator.cs
│   │   ├── CalculatorEngine.cs
│   │   └── FileProcessor.cs
│   ├── MainWindow.xaml         # UI definition
│   └── MainWindow.xaml.cs      # UI code-behind
├── CalculatorWPF.Tests/        # Unit tests
│   └── Services/
│       ├── CalculatorEngineTests.cs
│       ├── TokenizerTests.cs
│       ├── ExpressionEvaluatorTests.cs
│       └── FileProcessorTests.cs
└── README.md
```

## Design Decisions

### Why BigInteger?
The specification includes a bonus for handling large numbers. `BigInteger` allows the calculator to work with numbers of any size, limited only by available memory.

### Why Async/Await?
File processing can take time for large files. Async operations ensure the UI remains responsive, meeting the requirement that "UI should never freeze."

### Why Recursive Descent Parser?
A recursive descent parser naturally implements operator precedence through its grammar structure, making the code readable and maintainable.

### Error Messages
All error messages are descriptive and include relevant context (character positions, invalid characters) to help users identify issues quickly.

## Requirements Met

✅ **OOP**: Clear class hierarchy with single responsibilities  
✅ **C# / WPF / .NET 8**: Built with specified technologies  
✅ **async/await**: All long operations are asynchronous  
✅ **No 3rd party libraries**: Uses only .NET framework libraries  
✅ **Integer Calculator**: Supports +, -, *, / operations  
✅ **Operator Precedence**: Correctly evaluates expressions  
✅ **File Processing**: Reads input files, writes output files  
✅ **Error Handling**: Descriptive error messages for invalid input  
✅ **Empty Lines**: Properly ignored in input files  
✅ **UI Never Freezes**: All blocking operations are async  
✅ **Application Never Crashes**: Comprehensive error handling  

### Bonus Requirements
✅ **Large Numbers**: BigInteger support for arbitrary precision  
✅ **Unit Tests**: Comprehensive test coverage  

## License

This is a technical assessment project.
