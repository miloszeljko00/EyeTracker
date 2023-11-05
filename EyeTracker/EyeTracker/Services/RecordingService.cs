using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using ScreenRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EyeTracker.Services;

public class RecordingService
{
    private readonly ApplicationDbContext _context;
    private Recorder? _recorder;
    private Recording? CurrentRecording { get; set; }
    public Recording? SelectedRecording { get; set; }
    private int counter = 1;
    public RecordingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void StartRecordingScreen(Profile profile, ROIConfig config)
    {

        Screen primaryScreen = Screen.PrimaryScreen;
        int screenWidth = primaryScreen.Bounds.Width;
        int screenHeight = primaryScreen.Bounds.Height;
        if (_recorder != null) return;
        _recorder = new Recorder("out.avi", 100, 0, 0, screenWidth, screenHeight, true, -1, false);
        var id = Guid.NewGuid();
        CurrentRecording = new Recording()
        {
            Id = id,
            Profile = profile,
            ROIConfig = config,
            RecordingStart = DateTime.Now,
            RecordingEnd = DateTime.Now,
            VideoUrl = id.ToString() + ".avi",
            Points = new(),
        };
    }
    public void StopRecordingScreen()
    {
        if(_recorder == null) return;
        if(CurrentRecording == null) return;
        _recorder.Dispose();
        _recorder = null;
        CurrentRecording.RecordingEnd = DateTime.Now;
        CurrentRecording.Profile = _context.Profiles.Where(x => x.Id == CurrentRecording.Profile.Id).First();
        CurrentRecording.ROIConfig = _context.ROIConfigs.Where(x => x.Id == CurrentRecording.ROIConfig.Id).First();
        _context.Add(CurrentRecording);
        _context.SaveChanges();
    }
    public bool DrawROI(List<Models.ROI> rois)
    {
        if (CurrentRecording == null) return false;
        List<Contracts.ROI> roiContracts = FromROIsToContractROIs(rois);

        var result = Editor.AddROIsToRecording(roiContracts, "out.avi", CurrentRecording.VideoUrl);
        CurrentRecording = null;
        return result;
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

    public List<Recording> GetAllForConfig(Guid configId)
    {
        return _context.Recordings
            .Where(x => x.ROIConfig.Id == configId)
                .Include(x => x.ROIConfig)
                .Include(x => x.Profile)
            .AsNoTracking().ToList();
    }
}
