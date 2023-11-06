using EyeTracker.Models;
using GazepointClient.Interfaces;
using Microsoft.EntityFrameworkCore;
using ScreenRecorder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace EyeTracker.Services;

public class RecordingService
{
    private readonly ApplicationDbContext _context;
    private IGPClient _gpClient { get; set; }
    private Recorder? _recorder;
    private Recording? CurrentRecording { get; set; }
    public Recording? SelectedRecording { get; set; }
    private int counter = 1;
    public RecordingService(ApplicationDbContext context, IGPClient pgClient)
    {
        _context = context;
        _gpClient = pgClient;
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
        var etConfig = _context.EyeTrackerConfigs.First();
        etConfig.ScreenWidth = screenWidth;
        etConfig.ScreenHeight = screenHeight;

        var roiConfig = new Contracts.ROIConfig() 
        { 
            Id = config.Id.ToString(),
            ROIs = FromROIsToContractROIs(config.ROIs),
        };
        var eyeTrackerConfig = new Contracts.EyeTrackerConfig()
        {
            Address = etConfig.Address,
            Port = etConfig.Port,
            ScreenWidth = etConfig.ScreenWidth,
            ScreenHeight = etConfig.ScreenHeight,
        };
        _gpClient.StartRecording(roiConfig, eyeTrackerConfig);
    }
    public Recording? StopRecordingScreen()
    {
        if(_recorder == null) return null;
        if(CurrentRecording == null) return null;
        _recorder.Dispose();
        _recorder = null;
        _gpClient.StopRecording();
        var recordingFilePath = _gpClient.GetRecordingFilePath(CurrentRecording.ROIConfig.Id.ToString());
        List<RecordingPoint> recordingPoints = new();
        try
        {
            using (var reader = new StreamReader(@recordingFilePath))
            {
            
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(',');

                    var xPercentage = double.Parse(values[2]) / 100000D;
                    var yPercentage = double.Parse(values[3]) / 100000D;
                    Screen primaryScreen = Screen.PrimaryScreen;
                    int screenWidth = primaryScreen.Bounds.Width;
                    int screenHeight = primaryScreen.Bounds.Height;

                    var recordingPoint = new RecordingPoint()
                    {
                        Id = Guid.NewGuid(),
                        Order = int.Parse(values[0]),
                        Timestamp = long.Parse(values[1]),
                        X = xPercentage * screenWidth,
                        Y = yPercentage * screenHeight,
                        Label = values[11],
                    };
                    recordingPoints.Add(recordingPoint);
                } 
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        CurrentRecording.Points = recordingPoints;
        CurrentRecording.RecordingEnd = DateTime.Now;
        CurrentRecording.Profile = _context.Profiles.Where(x => x.Id == CurrentRecording.Profile.Id).First();
        CurrentRecording.ROIConfig = _context.ROIConfigs.Where(x => x.Id == CurrentRecording.ROIConfig.Id).Include(x => x.ROIs).First();
        _context.Add(CurrentRecording);
        _context.SaveChanges();
        return CurrentRecording;
    }
    public bool DrawRecording(Recording recording)
    {
        recording = _context.Recordings.Where(x => x.Id == recording.Id)
                .Include(x => x.ROIConfig)
                .Include(x => x.Points)
            .First();
        if (CurrentRecording == null) return false;

        var result = Editor.DrawRecording(FromRecordingToContractRecording(recording), "out.avi", CurrentRecording.VideoUrl);
        CurrentRecording = null;
        return result;
    }
    private Contracts.Recording FromRecordingToContractRecording(Models.Recording recording)
    {
        return new Contracts.Recording()
        {
            Points = FromRecordingPointsToContractRecordingPoints(recording.Points),
            ROIConfig = FromROIConfigToContractROIConfig(recording.ROIConfig),
            RecordingEnd = recording.RecordingEnd,
            RecordingStart = recording.RecordingStart,
        };
    }

    private Contracts.ROIConfig FromROIConfigToContractROIConfig(ROIConfig rOIConfig)
    {
        return new Contracts.ROIConfig()
        {
            Id = rOIConfig.Id.ToString(),
            ROIs = FromROIsToContractROIs(rOIConfig.ROIs),
        };
    }

    private List<Contracts.RecordingPoint> FromRecordingPointsToContractRecordingPoints(List<RecordingPoint> points)
    {
        var recordingPoints = new List<Contracts.RecordingPoint>();
        foreach (var point in points)
        {
            recordingPoints.Add(new Contracts.RecordingPoint()
            {
                Order = point.Order,
                Timestamp = point.Timestamp,
                X = point.X,
                Y = point.Y,
                Label = point.Label,
            });
        }
        return recordingPoints;
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
