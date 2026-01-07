using System.IO;
using CalculatorWPF.Services;
using Xunit;

namespace CalculatorWPF.Tests.Services
{
    public class FileProcessorTests : IDisposable
    {
        private readonly FileProcessor _processor;
        private readonly string _testDirectory;
        private readonly List<string> _filesToCleanup;

        public FileProcessorTests()
        {
            _processor = new FileProcessor();
            _testDirectory = Path.Combine(Path.GetTempPath(), "CalculatorTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _filesToCleanup = new List<string>();
        }

        [Fact]
        public async Task ProcessFileAsync_ValidInput_CreatesOutputFile()
        {
            // Arrange
            string inputFile = CreateTestFile("5 + 3\n10 - 4");
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            Assert.True(File.Exists(outputFile));
        }

        [Fact]
        public async Task ProcessFileAsync_ValidExpressions_ProducesCorrectOutput()
        {
            // Arrange
            string inputFile = CreateTestFile("2 + 3 * 2\n10 / 2\n-5 + 3");
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            string[] outputLines = await File.ReadAllLinesAsync(outputFile);
            Assert.Equal("8", outputLines[0]);
            Assert.Equal("5", outputLines[1]);
            Assert.Equal("-2", outputLines[2]);
        }

        [Fact]
        public async Task ProcessFileAsync_InvalidExpressions_ProducesErrorMessages()
        {
            // Arrange
            string inputFile = CreateTestFile("a + 1\n2 / 3\n2.1*2");
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            string[] outputLines = await File.ReadAllLinesAsync(outputFile);
            Assert.StartsWith("Error - Invalid character: 'a'", outputLines[0]);
            Assert.Equal("0.6666666666666666666666666667", outputLines[1]); // 2 / 3 with decimal support
            Assert.Equal("4.2", outputLines[2]); // 2.1 * 2 now works with decimal support
        }

        [Fact]
        public async Task ProcessFileAsync_EmptyLines_PreservesEmptyLines()
        {
            // Arrange
            string inputFile = CreateTestFile("5 + 3\n\n10 - 4\n\n");
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            string[] outputLines = await File.ReadAllLinesAsync(outputFile);
            Assert.Equal(5, outputLines.Length);
            Assert.Equal("8", outputLines[0]);
            Assert.Empty(outputLines[1]);
            Assert.Equal("6", outputLines[2]);
            Assert.Empty(outputLines[3]);
        }

        [Fact]
        public async Task ProcessFileAsync_NonExistentInputFile_ReturnsFailure()
        {
            // Arrange
            string inputFile = GetTestFilePath("nonexistent.txt");
            string outputFile = GetTestFilePath("output.txt");

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        [Fact]
        public async Task ProcessFileAsync_EmptyInputPath_ReturnsFailure()
        {
            // Arrange
            string outputFile = GetTestFilePath("output.txt");

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync("", outputFile);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("cannot be empty", result.Message);
        }

        [Fact]
        public async Task ProcessFileAsync_EmptyOutputPath_ReturnsFailure()
        {
            // Arrange
            string inputFile = CreateTestFile("5 + 3");

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, "");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("cannot be empty", result.Message);
        }

        [Fact]
        public async Task ProcessFileAsync_CreatesOutputDirectory_IfNotExists()
        {
            // Arrange
            string inputFile = CreateTestFile("5 + 3");
            string subDir = Path.Combine(_testDirectory, "newdir");
            string outputFile = Path.Combine(subDir, "output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            Assert.True(Directory.Exists(subDir));
            Assert.True(File.Exists(outputFile));
        }

        [Fact]
        public async Task ProcessFileAsync_ReportsProgress()
        {
            // Arrange
            string inputFile = CreateTestFile("1 + 1\n2 + 2\n3 + 3\n4 + 4\n5 + 5");
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);
            var progressReports = new List<int>();
            var progress = new Progress<int>(p => progressReports.Add(p));

            // Act
            await _processor.ProcessFileAsync(inputFile, outputFile, progress);

            // Assert
            Assert.NotEmpty(progressReports);
            Assert.Contains(100, progressReports); // Should reach 100%
        }

        [Fact]
        public async Task ProcessFileAsync_MixedValidAndInvalidLines_ProcessesAll()
        {
            // Arrange
            string inputContent = "2 + -3 * 2\na + 1\n2 / 3\n2.1*2";
            string inputFile = CreateTestFile(inputContent);
            string outputFile = GetTestFilePath("output.txt");
            _filesToCleanup.Add(outputFile);

            // Act
            ProcessingResult result = await _processor.ProcessFileAsync(inputFile, outputFile);

            // Assert
            Assert.True(result.Success);
            string[] outputLines = await File.ReadAllLinesAsync(outputFile);
            Assert.Equal(4, outputLines.Length);
            Assert.Equal("-4", outputLines[0]); // 2 + (-3 * 2) = 2 + (-6) = -4
            Assert.StartsWith("Error - Invalid character: 'a'", outputLines[1]);
            Assert.Equal("0.6666666666666666666666666667", outputLines[2]); // 2 / 3 with decimal support
            Assert.Equal("4.2", outputLines[3]); // 2.1 * 2 now works with decimal support
        }

        private string CreateTestFile(string content)
        {
            string filePath = GetTestFilePath($"input_{Guid.NewGuid()}.txt");
            File.WriteAllText(filePath, content);
            _filesToCleanup.Add(filePath);
            return filePath;
        }

        private string GetTestFilePath(string fileName)
        {
            return Path.Combine(_testDirectory, fileName);
        }

        public void Dispose()
        {
            try
            {
                foreach (string file in _filesToCleanup)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }

                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // Cleanup failed, but we don't want to fail the tests
            }
        }
    }
}
