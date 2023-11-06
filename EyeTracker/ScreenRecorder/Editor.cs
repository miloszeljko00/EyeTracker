using System;
using System.Collections.Generic;
using Contracts;
using OpenCvSharp;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace ScreenRecorder;


public class Editor
{
    public static bool DrawRecording(Recording recording, string inputVideoPath, string outputVideoPath)
    {
        var rois = recording.ROIConfig.ROIs;
        var recordingPoints = recording.Points.OrderBy(x => x.Order).ToList();

        VideoCapture? videoCapture = null;
        VideoWriter? videoWriter = null;
        try
        {
            videoCapture = new VideoCapture(inputVideoPath);
            if (!videoCapture.IsOpened())
            {
                Console.WriteLine("Error: Video file could not be opened.");
                return false; // Exit the function or handle the error appropriately
            }
            videoWriter = new VideoWriter(outputVideoPath, OpenCvSharp.FourCC.XVID, 15, new Size((int)videoCapture.FrameWidth, (int)videoCapture.FrameHeight));

            var totalFrames = videoCapture.FrameCount;
            var pointsToDrawCount = recordingPoints.Count / totalFrames;
            int step = Math.Max(1, recordingPoints.Count / pointsToDrawCount);
            var pointsToDraw = new List<RecordingPoint>();
            for (int i = 0; i < recordingPoints.Count; i += step)
            {
                pointsToDraw.Add(recordingPoints[i]);
            }
            var frameCounter = 0;
            var currentPointIndex = 0;
            var framesPerPoint = totalFrames / pointsToDrawCount;
            while (true)
            {
                Mat frame = new();
                if (!videoCapture.Read(frame)) break;
                if (frame.Empty()) break;
                if(frameCounter < framesPerPoint)
                {
                    currentPointIndex++;
                    if (currentPointIndex >= pointsToDraw.Count) currentPointIndex = pointsToDraw.Count - 1;
                }
                Cv2.Circle(frame, new Point(pointsToDraw[currentPointIndex].X, pointsToDraw[currentPointIndex].Y), 5, new Scalar(0, 0, 255), -1);
                
                foreach (ROI roi in rois)
                {
                    var points = new List<OpenCvSharp.Point>();
                    foreach (var roiPoint in roi.Points)
                    {
                        points.Add(new OpenCvSharp.Point()
                        {
                            X = int.Parse(Math.Round(roiPoint.X).ToString()),
                            Y = int.Parse(Math.Round(roiPoint.Y).ToString()),
                        });
                    }
                    if (pointsToDraw[currentPointIndex].Label == roi.Id)
                    {
                        Cv2.Polylines(frame, new[] { points.ToArray() }, isClosed: true, color: new Scalar(255, 0, 0), thickness: 2);
                    }
                    else
                    {
                        Cv2.Polylines(frame, new[] { points.ToArray() }, isClosed: true, color: new Scalar(0, 0, 255), thickness: 2);
                    }
                }

                videoWriter.Write(frame);
                frameCounter++;
            }
        }
        finally
        {
            if(videoCapture != null) videoCapture.Release();
            if(videoWriter != null) videoWriter.Release();
        }
        return true;
    }
}
