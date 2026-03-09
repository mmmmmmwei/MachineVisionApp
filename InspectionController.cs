using System;
using System.Diagnostics;
using System.Threading;

public class InspectionController
{
    private CameraController camera;

    private Thread acquisitionThread;
    private bool running;

    private int durationMs;
    private Mode currentMode;

    private enum Mode
    {
        SingleShot,
        TimedRun,
        Manual
    }

    public InspectionController(CameraController cam)
    {
        camera = cam;
    }

    // MODE 1
    public void SingleShot()
    {
        currentMode = Mode.SingleShot;
        StartThread();
    }

    // MODE 2
    public void RunForDuration(int ms)
    {
        durationMs = ms;
        currentMode = Mode.TimedRun;
        StartThread();
    }

    // MODE 3
    public void StartManual()
    {
        currentMode = Mode.Manual;
        StartThread();
    }

    public void StopManual()
    {
        running = false;
    }

    private void StartThread()
    {
        running = true;

        acquisitionThread = new Thread(AcquisitionLoop);
        acquisitionThread.IsBackground = true;
        acquisitionThread.Start();
    }

    private void AcquisitionLoop()
    {
        camera.StartAcquisition();

        switch (currentMode)
        {
            case Mode.SingleShot:

                var frame = camera.GrabFrame();
                Inspect(frame);
                running = false;

                break;

            case Mode.TimedRun:

                var timer = Stopwatch.StartNew();

                while (running && timer.ElapsedMilliseconds < durationMs)
                {
                    frame = camera.GrabFrame();
                    Inspect(frame);
                }

                break;

            case Mode.Manual:

                while (running)
                {
                    frame = camera.GrabFrame();
                    Inspect(frame);
                }

                break;
        }

        camera.StopAcquisition();
    }

    private void Inspect(object frame)
    {
        // vision algorithm here
    }
}