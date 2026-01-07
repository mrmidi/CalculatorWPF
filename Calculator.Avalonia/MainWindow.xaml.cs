using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CalculatorWPF.Services;

namespace Calculator.AvaloniaApp
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorEngine _engine = new CalculatorEngine();
        private readonly FileProcessor _fileProcessor = new FileProcessor();

        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("=== MainWindow Constructor ===");
            
            // Manually find controls since code generation may not be working
            var calcButton = this.FindControl<Button>("CalcButton");
            var expressionTextBox = this.FindControl<TextBox>("ExpressionTextBox");
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
            
            Console.WriteLine($"CalcButton found: {calcButton != null}");
            Console.WriteLine($"ExpressionTextBox found: {expressionTextBox != null}");
            Console.WriteLine($"ResultTextBlock found: {resultTextBlock != null}");

            if (calcButton != null)
            {
                Console.WriteLine("Attaching Click handler to CalcButton");
                calcButton.Click += OnCalculate;
            }
            else
            {
                Console.WriteLine("WARNING: CalcButton not found!");
            }
            
            var inputBrowseButton = this.FindControl<Button>("InputBrowseButton");
            var outputBrowseButton = this.FindControl<Button>("OutputBrowseButton");
            var processButton = this.FindControl<Button>("ProcessButton");
            
            if (inputBrowseButton != null) inputBrowseButton.Click += OnBrowseInput;
            if (outputBrowseButton != null) outputBrowseButton.Click += OnBrowseOutput;
            if (processButton != null) processButton.Click += OnProcessFile;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCalculate(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("=== OnCalculate FIRED ===");
            Console.WriteLine($"sender: {sender?.GetType().Name}");
            
            var expressionTextBox = this.FindControl<TextBox>("ExpressionTextBox");
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
            
            string expression = expressionTextBox?.Text ?? string.Empty;
            Console.WriteLine($"Expression from textbox: '{expression}'");
            
            string result = _engine.Calculate(expression);
            Console.WriteLine($"Calculation result: '{result}'");
            
            if (resultTextBlock != null)
            {
                resultTextBlock.Text = result;
                Console.WriteLine("Result updated in UI");
            }
            else
            {
                Console.WriteLine("WARNING: ResultTextBlock is null!");
            }
        }

        private async void OnBrowseInput(object? sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Select Input File",
                AllowMultiple = false
            });
            
            var inputFileBox = this.FindControl<TextBox>("InputFileBox");
            if (files.Count > 0 && inputFileBox != null)
            {
                inputFileBox.Text = files[0].Path.LocalPath;
            }
        }

        private async void OnBrowseOutput(object? sender, RoutedEventArgs e)
        {
            var file = await StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Select Output File"
            });
            
            var outputFileBox = this.FindControl<TextBox>("OutputFileBox");
            if (file != null && outputFileBox != null)
            {
                outputFileBox.Text = file.Path.LocalPath;
            }
        }

        private async void OnProcessFile(object? sender, RoutedEventArgs e)
        {
            var inputFileBox = this.FindControl<TextBox>("InputFileBox");
            var outputFileBox = this.FindControl<TextBox>("OutputFileBox");
            var processButton = this.FindControl<Button>("ProcessButton");
            var progressBar = this.FindControl<ProgressBar>("ProgressBar");
            var statusBlock = this.FindControl<TextBlock>("StatusBlock");
            
            if (inputFileBox == null || outputFileBox == null || processButton == null || progressBar == null || statusBlock == null)
                return;

            string input = inputFileBox.Text ?? string.Empty;
            string output = outputFileBox.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input) || !File.Exists(input))
            {
                statusBlock.Text = "Input file not found.";
                return;
            }

            processButton.IsEnabled = false;
            progressBar.Value = 0;
            statusBlock.Text = "Processing...";

            var progress = new Progress<int>(p => { if (progressBar != null) progressBar.Value = p; });

            var result = await _fileProcessor.ProcessFileAsync(input, output, progress);

            statusBlock.Text = result.Message;
            progressBar.Value = 100;
            processButton.IsEnabled = true;
        }
    }
}