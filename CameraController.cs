using Serilog;

namespace ConsoleApp1;

public class CameraController
{
    public string Name { get; }
    public CameraDevice Camera { get; }
    public ImageBuffer Buffer { get; }
    public InspectionPipeline Pipeline { get; }

    // Event fired when inspection is completed
    public event Action<InspectionResult> InspectionCompleted;

    private readonly ILogger _logger;

    public CameraController(string name, ILogger logger)
    {
        Name = name;
        _logger=logger;

        Camera = new CameraDevice(name, _logger);
        Buffer = new ImageBuffer();
        Pipeline = new InspectionPipeline(name, _logger);

        Camera.ImageCaptured += OnImageCaptured;
    }

    // Called automatically whenever camera produces an image
    private void OnImageCaptured(int image)
    {
        // Store in buffer
        Buffer.SetImage(image);

        // Optional: auto inspection
        //Inspect(); // Uncomment for auto mode
    }

    // Manual inspection
    public void Inspect()
    {
        if (!Buffer.HasImage) return;

        int img = Buffer.GetImage();
        var result = Pipeline.Process(img);
        // img.Dispose();

        InspectionCompleted?.Invoke(result);
    }

    public void StartLive() => Camera.StartLiveCapture();
    public void StopLive() => Camera.StopLiveCapture();

    public void LoadOfflineImage(string path) => Camera.LoadImage(path);
}