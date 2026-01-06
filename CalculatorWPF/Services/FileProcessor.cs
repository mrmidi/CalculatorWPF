using System.IO;

namespace CalculatorWPF.Services
{
    /// <summary>
    /// Processes mathematical expressions from input files asynchronously
    /// </summary>
    public class FileProcessor
    {
        private readonly CalculatorEngine _calculator;

        public FileProcessor()
        {
            _calculator = new CalculatorEngine();
        }

        /// <summary>
        /// Processes an input file and writes results to an output file asynchronously
        /// </summary>
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

                // Ensure output directory exists
                string? outputDirectory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Read all lines asynchronously
                string[] lines = await File.ReadAllLinesAsync(inputFilePath);
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
                            // Ignore empty lines
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

                // Write results asynchronously
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

    /// <summary>
    /// Result of file processing operation
    /// </summary>
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
