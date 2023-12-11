using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GazepointClient.Interfaces;
using Contracts;
using GazepointClient.Model;
using System.Security.Cryptography;

namespace GazepointClient.Services
{
    public class GPClient : IGPClient
    {
        private CancellationTokenSource cancellationTokenSource;

        private string sessionName = string.Empty;

        public GazepointReader GazepointReader { get; set; } = new GazepointReader();

        private string GetGPClientProjectPath()
        {
            string workingDirectory = Directory.GetCurrentDirectory();  // debug or release dir
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;  // project dir with app.xaml file

            return Path.Join(projectDirectory, "..", "GazepointClient");
        }

        public string GetRecordingFilePath(string roiConfigId)
        {
            string clientProject = GetGPClientProjectPath();
            return Path.Join(clientProject, "recording_data", roiConfigId + ".csv");
        }

        private async Task Record(PointLabeler pointLabeler, EyeTrackerConfig eyeTrackerConfig, CancellationToken cancellationToken)
        {
            TcpClient gp3Client;
            NetworkStream dataFeed;
            StreamWriter dataWrite;

            if (!GazepointReader.SignalConfiguration.InputSignals.Contains("ENABLE_SEND_POG_BEST"))
            {
                throw new Exception("ENABLE_SEND_POG_BEST not in input signal list. Can't label without the (X,Y) coordinates from it");
            }

            try
            {
                gp3Client = new TcpClient(eyeTrackerConfig.Address, int.Parse(eyeTrackerConfig.Port));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect with error: {0}", e);
                return;
            }

            int startindex, endindex;
            string incoming_data = "";

            dataFeed = gp3Client.GetStream();
            dataWrite = new StreamWriter(dataFeed);

            dataWrite.Write(GazepointReader.WriteSignalXMLSignalConfiguration());
            dataWrite.Flush();

            while (!cancellationToken.IsCancellationRequested)
            {
                byte[] byteCh = new byte[1];  // one byte/char at a time

                int readBytes = await dataFeed.ReadAsync(byteCh, 0, 1, cancellationToken);
                if (readBytes == 0)
                {
                    Console.WriteLine("Server connection lost");
                    break;
                }

                int ch = byteCh[0];
                if (ch != -1)
                {
                    incoming_data += (char)ch;

                    if (incoming_data.IndexOf("\r\n") != -1)
                    {
                        if (incoming_data.IndexOf("<REC") != -1)
                        {
                            Console.WriteLine(incoming_data);
                            GazepointReader.ParseIncomingDataLine(incoming_data);

                            POG_Best pogBest = (POG_Best)GazepointReader.SignalObjectsDict["ENABLE_SEND_POG_BEST"].Last();
                            Point eyeCoordinates = Point.FractionCoordinatesToAbsoluteCoordinates(new Point(pogBest.BPOGX / 100000D, pogBest.BPOGY / 100000D), eyeTrackerConfig.ScreenWidth, eyeTrackerConfig.ScreenHeight);
                            GazepointReader.SignalObjectsDict["ROI_LABEL"].Add(new ROI_Label { Label = pointLabeler.LabelSignalObjectsData(eyeCoordinates, eyeTrackerConfig.ScreenHeight, eyeTrackerConfig.ScreenWidth) });
                        }

                        incoming_data = "";
                    }
                }
            }

            dataWrite.Close();
            dataFeed.Close();
            gp3Client.Close();
        }

        public void StartRecording(ROIConfig roiConfig, EyeTrackerConfig eyeTrackerConfig, string recordingId)
        {
            sessionName = recordingId;

            PointLabeler pointLabeler = new PointLabeler(roiConfig);

            cancellationTokenSource = new();
            Task.Run(async () => await Record(pointLabeler, eyeTrackerConfig, cancellationTokenSource.Token));
        }

        public void StopRecording()
        {
            cancellationTokenSource.Cancel();
            CSVExporter csvExporter = new();
            csvExporter.ExportData(GazepointReader.SignalObjectsDict, GetRecordingFilePath(sessionName));
        }
    }
}