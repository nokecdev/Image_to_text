using Microsoft.Extensions.Logging;
using OpenCvSharp;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Tesseract;
using Document = Microsoft.Office.Interop.Word.Document;
using Word = Microsoft.Office.Interop.Word;

namespace Image_to_text
{
    public partial class ImageProcessing
    {
        private readonly UserDataContext _context;
        private readonly ILogger<ImageProcessing> _logger;
        private readonly string _logFilePath = "console_log.txt";
        private string _importPath = string.Empty;
        private const string FileName = "Image";

        public ImageProcessing(UserDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Logger setup
            var logFileWriter = new StreamWriter(_logFilePath, append: true);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new Logger(logFileWriter));
            });
            _logger = loggerFactory.CreateLogger<ImageProcessing>();
        }

        public void LoadImage()
        {
            _context.SettingsManager.Ofd.Filter = "Image Files (JPG,JPEG,PNG,TIFF)|*.JPG;*.JPEG;*.PNG;*.TIFF";

            if (_context.SettingsManager.Ofd.ShowDialog() == true)
            {
                _importPath = _context.SettingsManager.Ofd.FileName;
                _context.SettingsManager.FileSource = _importPath;
                _logger.LogInformation("Loaded image from: {path}", _importPath);
            }
        }

        public void ConvertImage(string exportType)
        {
            if (string.IsNullOrEmpty(_importPath))
            {
                MessageBox.Show("No image loaded.");
                return;
            }

            string lang = _context.ItemType.SelectedTranslateType.Language.ToLower();
            string tesseractPath = @".\tessdata";

            try
            {
                using var log = new StreamWriter("log.txt");
                log.WriteLine($"Started OCR with language: {lang}");

                using var preprocessedImage = PreprocessImage(_importPath, log);
                string text = RunTesseract(preprocessedImage, tesseractPath, lang, log);

                ExportText(exportType, text, log);
                MessageBox.Show("Document created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OCR processing.");
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private Mat PreprocessImage(string path, StreamWriter log)
        {
            log.WriteLine("Preprocessing image...");
            var image = Cv2.ImRead(path, ImreadModes.Grayscale);
            var processed = new Mat();

            // Adaptive threshold to handle varying illumination
            Cv2.AdaptiveThreshold(image, processed, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 2);

            

            // Resize to improve OCR accuracy
            Cv2.Resize(processed, processed, new OpenCvSharp.Size(), 2.0, 2.0, InterpolationFlags.Cubic);

            // Dilation for broken characters
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            Cv2.MorphologyEx(processed, processed, MorphTypes.Dilate, kernel);

            // Slight blur to remove noise
            Cv2.GaussianBlur(processed, processed, new OpenCvSharp.Size(3, 3), 0);
            Cv2.Threshold(image, processed, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            // Invert colors for Tesseract
            Cv2.BitwiseNot(processed, processed);

            string savedImagePath = Path.Combine(Path.GetTempPath(), "ocr_input.png");
            processed.SaveImage(savedImagePath);
            log.WriteLine($"Image preprocessed and saved to: {savedImagePath}");

            return processed;
        }

        private string RunTesseract(Mat image, string tesseractPath, string lang, StreamWriter log)
        {
            log.WriteLine("Running Tesseract OCR...");
            string savedImagePath = Path.Combine(Path.GetTempPath(), "ocr_input.png");

            using var engine = new TesseractEngine(tesseractPath, lang, EngineMode.Default);
            engine.SetVariable("user_defined_dpi", "300");
            engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,!?-' ");
            engine.DefaultPageSegMode = PageSegMode.Auto;

            using var pix = Pix.LoadFromFile(savedImagePath);
            pix.Deskew();
            using var page = engine.Process(pix);

            string text = page.GetText().Trim();
            log.WriteLine("OCR completed successfully.");

            try
            {
                // Remove image deleting to check if image noise is ideal and readable for processing. Check the temp folder for result
                File.Delete(savedImagePath);
                log.WriteLine("Temporary image deleted.");
            }
            catch (Exception e)
            {
                log.WriteLine($"Warning: Could not delete temp file: {e.Message}");
            }

            return text;
        }

        private void ExportText(string exportType, string text, StreamWriter log)
        {
            string savePath = _context.SettingsManager.SavePath ?? Environment.CurrentDirectory;
            string fullPath = Path.Combine(savePath, $"{FileName}.{exportType.ToLower()}");

            log.WriteLine($"Exporting text as {exportType} to {fullPath}");

            switch (exportType)
            {
                case "Txt":
                    File.WriteAllText(fullPath, text, System.Text.Encoding.UTF8);
                    break;

                case "Word":
                    fullPath = Path.Combine(savePath, $"{FileName}.docx");
                    CreateWordDoc(fullPath, text);
                    break;

                case "Pdf":
                    File.WriteAllText(fullPath, text);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported export type: {exportType}");
            }
        }

        private void CreateWordDoc(string filePath, string text)
        {
            var app = new Word.Application();
            object missing = Missing.Value;
            Document document = app.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            document.Content.SetRange(0, 0);
            document.Content.Text = text + Environment.NewLine;

            object filename = filePath;
            document.SaveAs2(ref filename);
            document.Close(ref missing, ref missing, ref missing);
            app.Quit(ref missing, ref missing, ref missing);
        }
    }
}
