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
        var recordingPoints = recording.Points.OrderBy(x => x.Timestamp).ToList();
        var recordingStartTimestamp = recording.Points.First().Timestamp;
        var recordingEndTimestamp = recording.Points.Last().Timestamp;
        var recordingDuration = recordingEndTimestamp - recordingStartTimestamp;
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

            var pointsToDraw = new List<Point>();
            var totalFrames = videoCapture.FrameCount;
            var totalPoints = recordingPoints.Count;
            var pointsPerFrame = totalPoints / totalFrames;
            var durationPerFrame = recordingDuration / totalFrames;

            for ( var i = 0; i < totalFrames; i++)
            {
                int startIndex = i * pointsPerFrame;
                int endIndex = Math.Min((i + 1) * pointsPerFrame, recordingPoints.Count);

                // Calculate the mean X and Y values for the subset of points
                double meanX = 0;
                double meanY = 0;

                for (int j = startIndex; j < endIndex; j++)
                {
                    meanX += recordingPoints[j].X;
                    meanY += recordingPoints[j].Y;
                }

                meanX /= (endIndex - startIndex);
                meanY /= (endIndex - startIndex);
                pointsToDraw.Add(new Point((int)meanX, (int)meanY));
            }



            var currentFrame = 0;
            while (true)
            {
                Mat frame = new();
                if (!videoCapture.Read(frame)) break;
                if (frame.Empty()) break;


                int trailLength = 5;
                // Draw a trail of circles for previous points
                for (int i = Math.Max(0, currentFrame - trailLength); i < currentFrame; i++)
                {
                    double opacity = 1.0 - ((double)(currentFrame - i) / trailLength);
                    Scalar color = new Scalar(0, 200, 0, opacity * 255);
                    Cv2.Circle(frame, new Point(pointsToDraw[i].X, pointsToDraw[i].Y), 5, color, -1);
                }

                // Draw the current point with a different color
                Cv2.Circle(frame, new Point(pointsToDraw[currentFrame].X, pointsToDraw[currentFrame].Y), 10, new Scalar(0, 255, 0), -1);


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
                    if (recordingPoints[currentFrame].Label == roi.Id)
                    {
                        Cv2.Polylines(frame, new[] { points.ToArray() }, isClosed: true, color: new Scalar(255, 0, 0), thickness: 2);
                    }
                    else
                    {
                        Cv2.Polylines(frame, new[] { points.ToArray() }, isClosed: true, color: new Scalar(0, 0, 255), thickness: 2);
                    }
                }

                videoWriter.Write(frame);
                currentFrame++;
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
