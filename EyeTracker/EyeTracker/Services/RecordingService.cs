using EyeTracker.Models;
using OpenCvSharp;
using ScreenRecorder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EyeTracker.Services;

public class RecordingService
{
    private Recorder? _recorder;
    private int counter = 1;
    public RecordingService()
    {
    }

    public void StartRecordingScreen()
    {
        if (_recorder != null) return;
        _recorder = new Recorder(new RecorderParams("AAA" + counter + ".avi", 30, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 100));
    }
    public void StopRecordingScreen()
    {
        if(_recorder == null) return;
        _recorder.Dispose();
        _recorder = null;
    }
    public bool DrawROI(ROI roi)
    {
        var points = FromROIPointToDrawablePoints(roi.Points);
        return Editor.AddPointsAndPolylineToVideo(points, "AAA" + counter++ + ".avi", "AAA" + counter++ + ".avi");
    }

    private List<OpenCvSharp.Point> FromROIPointToDrawablePoints(List<ROIPoint> roiPoints)
    {
        List<OpenCvSharp.Point> points = new();
        foreach (var roiPoint in roiPoints)
        {
            points.Add(new OpenCvSharp.Point()
            {
                X = (int)Math.Round(roiPoint.X),
                Y = (int)Math.Round(roiPoint.Y),
            });
        }
        return points;
    }
}
