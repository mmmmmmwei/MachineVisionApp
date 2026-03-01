using System.Threading;
using Serilog;

namespace ConsoleApp1;

public class CameraDevice
{
    public string Name { get; }
    public event Action<int> ImageCaptured;

    private Thread _grabThread;
    private bool _running = false;
    private Random _rnd = new Random();

    private readonly ILogger _logger;

    public CameraDevice(string name, ILogger logger)
    {
        Name = name;
        _logger=logger;
    }

    // Simulate live capture
    public void StartLiveCapture()
    {
        _logger.Information("Start live");

        _running = true;
        _grabThread = new Thread(LiveLoop) { IsBackground = true };
        _grabThread.Start();
    }

    public void StopLiveCapture()
    {
        _logger.Information("Stop live");

        _running = false;
    }

    private void LiveLoop()
    {
        while (_running)
        {
            // Generate a dummy image
            int bmp=1;
            // int bmp = new int(320, 240);
            // using (Graphics g = Graphics.FromImage(bmp))
            // {
            //     g.Clear(Color.FromArgb(_rnd.Next(50, 200),
            //                            _rnd.Next(50, 200),
            //                            _rnd.Next(50, 200)));
            // }

            ImageCaptured?.Invoke(bmp);

            Thread.Sleep(500); // simulate 2 FPS
        }
    }

    // Load offline image manually
    public void LoadImage(string path)
    {
        _logger.Information("Load image start");

        // int bmp = new int(path);
        int bmp=1;
        ImageCaptured?.Invoke(bmp);

        _logger.Information("Load image end");
    }
}