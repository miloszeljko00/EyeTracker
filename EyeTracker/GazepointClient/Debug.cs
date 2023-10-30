﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient
{
    public class Debug
    {
        static public List<ROIPoint> RectangleROIPoints(int startX, int startY, int endX, int endY)
        {
            ROIPoint p1 = new ROIPoint
            {
                X = startX,
                Y = startY,
            };
            ROIPoint p2 = new ROIPoint
            {
                X = endX,
                Y = startY,
            };
            ROIPoint p3 = new ROIPoint
            {
                X = startX,
                Y = endY,
            };
            ROIPoint p4 = new ROIPoint
            {
                X = endX,
                Y = endY,
            };

            return new List<ROIPoint> { p1, p2, p3, p4 };
        }

        static public void Main(string[] args)
        {
            GPClient client = new();
            
            var roiPoints1 = RectangleROIPoints(10, 10, 100, 100);
            var roiPoints2 = RectangleROIPoints(1000, 1000, 1100, 1100);

            var roi1 = new ROI
            {
                Id = "roi1",
                Points = roiPoints1
            };
            var roi2 = new ROI
            {
                Id = "roi2",
                Points = roiPoints2
            };

            ROIConfig roiConfig = new ROIConfig
            {
                Id = "debug-config",
                ROIs = new List<ROI> { roi1, roi2 }
            };

            EyeTrackerConfig eyeTrackerConfig = new EyeTrackerConfig
            {
                Address = "172.21.208.1",
                Port = "4242",
                ScreenHeight = 1080,
                ScreenWidth = 1920,
            };

            client.StartRecording(roiConfig, eyeTrackerConfig);

            Console.WriteLine("Press any key to stop reading from the server...");
            Console.ReadKey();

            client.StopRecording();
        }
    }
}
