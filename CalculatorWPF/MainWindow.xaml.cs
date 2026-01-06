using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CalculatorWPF.Services;

namespace CalculatorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CalculatorEngine _calculator;
        private readonly FileProcessor _fileProcessor;
        private bool _isProcessing;

        public MainWindow()
        {
            InitializeComponent();
            _calculator = new CalculatorEngine();
            _fileProcessor = new FileProcessor();
            _isProcessing = false;

            // Enable Enter key to calculate
            ExpressionTextBox.KeyDown += ExpressionTextBox_KeyDown;
            
            // Set default output file path
            OutputFileTextBox.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                "output.txt");
        }

        private void ExpressionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CalculateButton_Click(sender, e);
            }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string expression = ExpressionTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(expression))
                {
                    ResultTextBlock.Text = "Please enter an expression";
                    return;
                }

                // Disable button during calculation
                CalculateButton.IsEnabled = false;
                ResultTextBlock.Text = "Calculating...";

                // Perform calculation asynchronously
                string result = await Task.Run(() => _calculator.Calculate(expression));

                // Update UI on UI thread
                ResultTextBlock.Text = result;
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error - {ex.Message}";
            }
            finally
            {
                CalculateButton.IsEnabled = true;
            }
        }

        private void BrowseInputButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select Input File",
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    InputFileTextBox.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting file: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Select Output File",
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = "output.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    OutputFileTextBox.Text = saveFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting file: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing)
            {
                MessageBox.Show("Processing is already in progress", "Information", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string inputFile = InputFileTextBox.Text.Trim();
                string outputFile = OutputFileTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(inputFile))
                {
                    MessageBox.Show("Please select an input file", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(outputFile))
                {
                    MessageBox.Show("Please select an output file", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!File.Exists(inputFile))
                {
                    MessageBox.Show($"Input file not found: {inputFile}", "File Not Found", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Disable controls during processing
                _isProcessing = true;
                ProcessButton.IsEnabled = false;
                BrowseInputButton.IsEnabled = false;
                BrowseOutputButton.IsEnabled = false;
                InputFileTextBox.IsEnabled = false;
                OutputFileTextBox.IsEnabled = false;
                ProcessingProgressBar.Value = 0;
                ProgressTextBlock.Text = "Processing...";

                // Create progress reporter
                var progress = new Progress<int>(percent =>
                {
                    ProcessingProgressBar.Value = percent;
                    ProgressTextBlock.Text = $"Processing... {percent}%";
                });

                // Process file asynchronously
                ProcessingResult result = await _fileProcessor.ProcessFileAsync(inputFile, outputFile, progress);

                // Show result
                if (result.Success)
                {
                    ProgressTextBlock.Text = $"Complete - {result.ProcessedLines} line(s) processed";
                    MessageBox.Show(result.Message, "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ProgressTextBlock.Text = "Processing failed";
                    MessageBox.Show(result.Message, "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ProgressTextBlock.Text = "Error occurred";
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Re-enable controls
                _isProcessing = false;
                ProcessButton.IsEnabled = true;
                BrowseInputButton.IsEnabled = true;
                BrowseOutputButton.IsEnabled = true;
                InputFileTextBox.IsEnabled = true;
                OutputFileTextBox.IsEnabled = true;
            }
        }
    }
}