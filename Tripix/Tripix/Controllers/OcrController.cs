using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using Tesseract;

[Route("api/[controller]")]
[ApiController]
public class OcrController : ControllerBase
{
    private readonly ILogger<OcrController> _logger;

    public OcrController ( ILogger<OcrController> logger )
    {
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage ( IFormFile file )
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("يرجى تحميل صورة صحيحة.");
        }

        try
        {
            // حفظ الصورة في ملف مؤقت
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // تحسين الصورة باستخدام OpenCV
            var processedImagePath = PreprocessImage(tempPath);

            // استخراج النص باستخدام Tesseract
            var extractedText = ExtractTextFromImage(processedImagePath);

            // حذف الملفات المؤقتة
            System.IO.File.Delete(tempPath);
            System.IO.File.Delete(processedImagePath);

            return Ok(new { data = extractedText });
        }
        catch (Exception ex)
        {
            _logger.LogError($"حدث خطأ أثناء معالجة الصورة: {ex.Message}");
            return StatusCode(500, "حدث خطأ أثناء معالجة الصورة: " + ex.Message);
        }
    }

    // 🔹 تحسين جودة الصورة باستخدام OpenCV
    private string PreprocessImage ( string imagePath )
    {
        Mat image = Cv2.ImRead(imagePath, ImreadModes.Grayscale); // تحويل إلى أبيض وأسود
        Cv2.GaussianBlur(image, image, new Size(5, 5), 0); // تقليل الضوضاء
        Cv2.Threshold(image, image, 120, 255, ThresholdTypes.Binary); // زيادة التباين

        // حفظ الصورة المحسنة
        var processedImagePath = imagePath.Replace(".tmp", "_processed.jpg");
        Cv2.ImWrite(processedImagePath, image);

        return processedImagePath;
    }

    // 🔹 استخراج النص باستخدام Tesseract OCR
    private List<Dictionary<string, string>> ExtractTextFromImage ( string imagePath )
    {
        var tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

        using var engine = new TesseractEngine(tessDataPath, "ara+eng", EngineMode.Default);
        using var img = Pix.LoadFromFile(imagePath);
        using var page = engine.Process(img);

        string extractedText = page.GetText();

        return new List<Dictionary<string, string>>
        {
            new()
            {
                { "label", "نص مستخرج" },
                { "value", extractedText.Trim() }
            }
        };
    }
}
