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
    public static bool AddROIsToRecording(List<ROI> rois, string inputVideoPath, string outputVideoPath)
    {
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
            videoWriter = new VideoWriter(outputVideoPath, OpenCvSharp.FourCC.XVID, 30, new Size((int)videoCapture.FrameWidth, (int)videoCapture.FrameHeight));

            while (true)
            {
                Mat frame = new();
                if (!videoCapture.Read(frame)) break;
                if (frame.Empty()) break;

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
                    Cv2.Polylines(frame, new[] { points.ToArray() }, isClosed: true, color: new Scalar(0, 0, 255), thickness: 2);
                }

                videoWriter.Write(frame);
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
