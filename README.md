# Overview
This is a simple WPF desktop application that extracts text from image files (JPG, PNG, TIFF, etc.) using Tesseract OCR.
![Basic features and simple usage.](/docs/main.png "This is the main view")


### The user can:

Choose the source language (English or Hungarian)

Select an image file

Set the export location and file format (TXT, DOCX, or PDF)

Start the conversion with a single click

The goal is to create an easy-to-use, offline OCR tool that runs locally without requiring installation.


## ⚙️ Main Features

🖼️ Load image files (JPG, JPEG, PNG, TIFF)

🌐 Language selection (English / Hungarian)

📄 Export results to TXT, Word, or PDF

🔍 Integrated Tesseract OCR engine

🧠 Automatic image preprocessing (thresholding, denoising, contrast, sharpening)

🧾 Logging support (log.txt, console_log.txt)

![Final result of an image processing.](/docs/result.png "Final result of an image processing")

### Prerequisites

* Windows 10 / 11
* .NET 8.0 Runtime or SDK
* Tesseract OCR Engine (Windows build)
    * Tesseract is a C++-based OCR engine that the C# app calls via wrapper libraries — it must be installed separately.

# Installing Tesseract on Windows

1. Download the official Windows build:
👉 UB Mannheim – Tesseract Windows installer

2. After installation, locate the tessdata folder, for example:

C:\Program Files\Tesseract-OCR\tessdata

```
Image_to_text\
└── tessdata\
    ├── eng.traineddata
    └── hun.traineddata
```

4. Make sure the tessdata folder is in the same directory as your executable.

1. 
* Development Environment (if you build from source)
* Visual Studio 2022+
* .NET 8.0 SDK
* NuGet Packages:
* Tesseract
* OpenCvSharp4
* OpenCvSharp4.runtime.win
* Microsoft.Extensions.Logging
* Microsoft.Office.Interop.Word

# Usage
* Run the application
* Select the language (English / Hungarian)
* Choose an image
* Select output folder and export format (TXT, Word, PDF)
* Click Convert
* Once finished, you’ll see a message:
“Document created successfully!”

📁 Output Files
Exported documents will appear in the chosen save location, e.g.:
C:\Users\<user>\Documents\Image.txt

Log files are generated in the project directory:
log.txt — processing log
console_log.txt — detailed debug log


OCR Quality Tips
Use sharp, high-resolution images (≥ 300 DPI).
The selected language must match the text in the image.
For blurry images, increase image size before conversion:
Cv2.Resize(image, image, new Size(), 2.0, 2.0);

To improve accuracy, try:
engine.DefaultPageSegMode = PageSegMode.AutoOsd;
This allows Tesseract to automatically detect layout and text orientation.



Possible Future Improvements
Automatic language detection
Drag & drop support
Screenshot OCR integration
AI-based post-processing for text correction
PDF output with embedded text layer



Purpose: A lightweight offline OCR tool built for personal use — and to make life easier for my sister