using System.IO;

namespace CalculatorWPF.Services
{
    // Processes expressions from file line by line
    public class FileProcessor
    {
        private readonly CalculatorEngine _calculator;

        public FileProcessor()
        {
            _calculator = new CalculatorEngine();
        }

        // Reads input file, calculates each line, writes to output file
        public async Task<ProcessingResult> ProcessFileAsync(string inputFilePath, string outputFilePath, IProgress<int>? progress = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputFilePath))
                {
                    return new ProcessingResult(false, "Input file path cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    return new ProcessingResult(false, "Output file path cannot be empty");
                }

                if (!File.Exists(inputFilePath))
                {
                    return new ProcessingResult(false, $"Input file not found: {inputFilePath}");
                }

                // Create output directory if needed
                string? outputDirectory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                string fileContent = await File.ReadAllTextAsync(inputFilePath);
                string[] lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                var results = new List<string>();
                int totalLines = lines.Length;
                int processedLines = 0;

                // Process each line
                foreach (string line in lines)
                {
                    await Task.Run(() =>
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            results.Add(string.Empty);
                        }
                        else
                        {
                            string result = _calculator.Calculate(line.Trim());
                            results.Add(result);
                        }
                    });

                    processedLines++;
                    progress?.Report((int)((double)processedLines / totalLines * 100));
                }

                await File.WriteAllLinesAsync(outputFilePath, results);

                return new ProcessingResult(true, $"Successfully processed {totalLines} line(s)", totalLines, processedLines);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ProcessingResult(false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return new ProcessingResult(false, $"IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new ProcessingResult(false, $"Unexpected error: {ex.Message}");
            }
        }
    }

    // Result of file processing operation
    public class ProcessingResult
    {
        public bool Success { get; }
        public string Message { get; }
        public int TotalLines { get; }
        public int ProcessedLines { get; }

        public ProcessingResult(bool success, string message, int totalLines = 0, int processedLines = 0)
        {
            Success = success;
            Message = message;
            TotalLines = totalLines;
            ProcessedLines = processedLines;
        }
    }
}
