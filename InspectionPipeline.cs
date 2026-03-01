using System.Security.Cryptography;
using Serilog;

namespace ConsoleApp1;

public class InspectionResult
{
    public bool IsPass { get; set; }
    public double AverageGray { get; set; }
    public string CameraName { get; set; }
}

public class InspectionPipeline
{
    public string CameraName { get; }

    private readonly ILogger _logger;

    public InspectionPipeline(string cameraName, ILogger logger)
    {
        CameraName = cameraName;
        _logger=logger;
    }

    // Example processing: compute average grayscale
    public InspectionResult Process(int image)
    {
        _logger.Information("Inspection start");
        // if (image == null) return null;

        // double total = 0;
        // int count = image.Width * image.Height;

        // for (int y = 0; y < image.Height; y++)
        //     for (int x = 0; x < image.Width; x++)
        //     {
        //         Color pixel = image.GetPixel(x, y);
        //         total += (pixel.R + pixel.G + pixel.B) / 3.0;
        //     }

        // double avg = total / count;
        // bool pass = avg > 100; // simple threshold

        _logger.Information("Inspection end");

        return new InspectionResult
        {
            CameraName = CameraName,
            AverageGray = 0,
            IsPass = true
        };
    }
}