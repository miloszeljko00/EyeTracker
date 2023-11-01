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
using Contracts;

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
    public bool DrawROI(List<Models.ROI> rois)
    {
        List<Contracts.ROI> roiContracts = FromROIsToContractROIs(rois);

        return Editor.AddROIsToRecording(roiContracts, "AAA" + counter++ + ".avi", "AAA" + counter++ + ".avi");
    }

    private List<Contracts.ROI> FromROIsToContractROIs(List<Models.ROI> rois)
    {
        var roiContracts = new List<Contracts.ROI>();
        foreach (var roisItem in rois)
        {
            roiContracts.Add(new Contracts.ROI()
            {
                Id = roisItem.Id.ToString(),
                Points = FromROIPointToContractPoints(roisItem.Points),
            });
        }

        return roiContracts;
    }

    private List<Contracts.ROIPoint> FromROIPointToContractPoints(List<Models.ROIPoint> roiPoints)
    {
        List<Contracts.ROIPoint> points = new();
        foreach (var roiPoint in roiPoints)
        {
            points.Add(new Contracts.ROIPoint()
            {
                X = (int)Math.Round(roiPoint.X),
                Y = (int)Math.Round(roiPoint.Y),
            });
        }
        return points;
    }
}
