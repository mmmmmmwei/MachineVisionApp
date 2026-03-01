using Serilog;

namespace ConsoleApp1;

public class CameraManager
{
    private List<CameraController> _cameras = new List<CameraController>();

    public IReadOnlyList<CameraController> Cameras => _cameras;

    private readonly ILogger _logger;

    public CameraManager(ILogger logger)
    {
        _logger=logger;
    }

    public void AddCamera(CameraController camera)
    {
        _cameras.Add(camera);
    }

    // Start all cameras
    public void StartAll()
    {
        foreach (var cam in _cameras)
            cam.StartLive();
    }

    public void Start(int id)
    {
        if (id>=0 && id<_cameras.Count())
            _cameras[id].StartLive();
    }

    public void StopAll()
    {
        foreach (var cam in _cameras)
            cam.StopLive();
    }

    public void Stop(int id)
    {
        if (id>=0 && id<_cameras.Count())
            _cameras[id].StopLive();
    }

    // Manual inspection for all cameras
    public void InspectAll()
    {
        foreach (var cam in _cameras)
            cam.Inspect();
    }

    public void Inspect(int id)
    {
        if (id>=0 && id<_cameras.Count())
            _cameras[id].Inspect();
    }
}