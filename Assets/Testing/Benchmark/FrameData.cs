using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrameData
{
    public float frameTime;
    public float fps => 1000f / frameTime;

    public bool advancedFrameTiming;
    public double cpuTime;
    public double cpuRenderTime;
    public double gpuTime;

    public double timeLineTime;

    public FrameData( float timeMS, float timelineTime = 0, bool captureAdvancedTimings = true )
    {
        frameTime = timeMS;
        this.timeLineTime = timelineTime;

        advancedFrameTiming = FrameTimingManager.IsFeatureEnabled() && captureAdvancedTimings;

        cpuTime = cpuRenderTime = gpuTime = timeMS;

        if (advancedFrameTiming)
        {
            FrameTimingManager.CaptureFrameTimings();
            FrameTiming[] timings = new FrameTiming[1];
            uint count = FrameTimingManager.GetLatestTimings(1, timings);
            if (count > 0)
            {
                cpuTime = timings[0].cpuFrameTime;
                cpuRenderTime = timings[0].cpuRenderThreadFrameTime;
                gpuTime = timings[0].gpuFrameTime;
            }
        }
    }

    public FrameData Min ( FrameData a, FrameData b )
    {
        FrameData o = new FrameData();

        o.frameTime = Mathf.Min(a.frameTime, b.frameTime);

        o.advancedFrameTiming = a.advancedFrameTiming && b.advancedFrameTiming;
        if (o.advancedFrameTiming)
        {
            o.cpuTime = DoubleMin(a.cpuTime , b.cpuTime);
            o.cpuRenderTime = DoubleMin(a.cpuRenderTime , b.cpuRenderTime);
            o.gpuTime = DoubleMin(a.gpuTime, b.gpuTime);
        }

        return o;
    }
    public void MinWith( FrameData other )
    {
        this = Min(this, other);
    }

    public FrameData Max(FrameData a, FrameData b)
    {
        FrameData o = new FrameData();

        o.frameTime = Mathf.Max(a.frameTime, b.frameTime);

        o.advancedFrameTiming = a.advancedFrameTiming && b.advancedFrameTiming;
        if (o.advancedFrameTiming)
        {
            o.cpuTime = DoubleMax(a.cpuTime, b.cpuTime);
            o.cpuRenderTime = DoubleMax(a.cpuRenderTime, b.cpuRenderTime);
            o.gpuTime = DoubleMax(a.gpuTime, b.gpuTime);
        }

        return o;
    }

    public void MaxWith( FrameData other )
    {
        this = Max(this, other);
    }

    private double DoubleMin(double a, double b)
    {
        return (a < b) ? a : b;
    }
    private double DoubleMax(double a, double b)
    {
        return (a > b) ? a : b;
    }

    override public string ToString()
    {
        return $"FrameData{{frameTime: {frameTime} ms, fps: {fps}, advancedFrameTiming: {advancedFrameTiming}, cpuTime: {cpuTime} ms, cpuRenderTime: {cpuRenderTime} ms, gpuTime: {gpuTime} ms}}";
    }
}
