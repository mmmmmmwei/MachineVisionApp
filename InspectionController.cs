using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

public class Frame
{
    // placeholder for your image
}

public class CameraController
{
    public void Initialize()
    {
        // open camera and configure FreeRun
    }

    public void StartAcquisition()
    {
        // SDK call to start acquisition
    }

    public void StopAcquisition()
    {
        // SDK call to stop acquisition
    }

    public Frame GrabFrame()
    {
        // SDK call to get frame
        return new Frame();
    }
}

public class InspectionController
{
    private CameraController camera;

    // threads
    private Thread cameraThread;
    private Thread inspectionThread;

    // running flags
    private volatile bool running = false;
    private volatile bool exitThreads = false;

    // mode control
    private int durationMs;
    private enum Mode { SingleShot, TimedRun, Manual }
    private Mode currentMode;

    // thread-safe queue
    private ConcurrentQueue<Frame> frameQueue = new ConcurrentQueue<Frame>();

    public InspectionController(CameraController cam)
    {
        camera = cam;

        // create persistent threads
        cameraThread = new Thread(CameraLoop) { IsBackground = true };
        inspectionThread = new Thread(InspectionLoop) { IsBackground = true };

        cameraThread.Start();
        inspectionThread.Start();
    }

    // ====== PUBLIC METHODS FOR MODES ======
    public void SingleShot()
    {
        if (running) return;
        currentMode = Mode.SingleShot;
        running = true;
    }

    public void RunForDuration(int ms)
    {
        if (running) return;
        durationMs = ms;
        currentMode = Mode.TimedRun;
        running = true;
    }

    public void StartManual()
    {
        if (running) return;
        currentMode = Mode.Manual;
        running = true;
    }

    public void StopManual()
    {
        running = false;
    }

    // ====== SHUTDOWN ======
    public void Shutdown()
    {
        running = false;
        exitThreads = true;

        cameraThread.Join();
        inspectionThread.Join();
    }

    // ====== CAMERA THREAD ======
    private void CameraLoop()
    {
        camera.StartAcquisition();

        while (!exitThreads)
        {
            if (!running)
            {
                Thread.Sleep(10);
                continue;
            }

            // Grab frame
            Frame frame = camera.GrabFrame();
            frameQueue.Enqueue(frame);

            // If single shot, stop after 1 frame
            if (currentMode == Mode.SingleShot)
            {
                running = false;
            }

            // If timed run, check duration
            if (currentMode == Mode.TimedRun)
            {
                // Use Stopwatch to measure elapsed time
                Stopwatch sw = Stopwatch.StartNew();
                if (sw.ElapsedMilliseconds >= durationMs)
                {
                    running = false;
                }
            }
        }

        camera.StopAcquisition();
    }

    // ====== INSPECTION THREAD ======
    private void InspectionLoop()
    {
        while (!exitThreads)
        {
            if (!running && frameQueue.IsEmpty)
            {
                Thread.Sleep(5);
                continue;
            }

            if (frameQueue.TryDequeue(out Frame frame))
            {
                Inspect(frame);
            }
        }
    }

    private void Inspect(Frame frame)
    {
        // Vision algorithm here
        Console.WriteLine("Frame inspected at " + DateTime.Now.ToString("HH:mm:ss.fff"));
    }
}