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
    public RecordingService()
    {
    }

    public void StartRecordingScreen()
    {
        if (_recorder != null) return;
        _recorder = new Recorder(new RecorderParams("REC.avi", 24, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 100));
    }
    public void StopRecordingScreen()
    {
        if(_recorder == null) return;
        _recorder.Dispose();
        _recorder = null;
    }
}
