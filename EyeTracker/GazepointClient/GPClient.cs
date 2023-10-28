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

namespace GazepointClient
{
    public class GPClient: IGPClient
    {
        private CancellationTokenSource cancellationTokenSource;

        public GazepointReader GazepointReader { get; set; } = new GazepointReader();

        public string GetRecordingFilePath(string roiConfigId)
        {
            throw new NotImplementedException();
        }

        private void Record(ROIConfig roiConfig, EyeTrackerConfig eyeTrackerConfig, CancellationToken cancellationToken)
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
                gp3Client = new TcpClient(eyeTrackerConfig.Address, Int32.Parse(eyeTrackerConfig.Port));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect with error: {0}", e);
                return;
            }

            int startindex, endindex;
            String incoming_data = "";

            dataFeed = gp3Client.GetStream();
            dataWrite = new StreamWriter(dataFeed);

            dataWrite.Write(GazepointReader.WriteSignalXMLSignalConfiguration());
            dataWrite.Flush();

            while(!cancellationToken.IsCancellationRequested)
            {
                int ch = dataFeed.ReadByte();
                if (ch != -1)
                {
                    incoming_data += (char)ch;

                    if (incoming_data.IndexOf("\r\n") != -1)
                    {
                        if (incoming_data.IndexOf("<REC") != -1)
                        {
                            Console.WriteLine(incoming_data);
                            GazepointReader.ParseIncomingDataLine(incoming_data);

                            // TODO(@Vlodson): logic for labeling stuff as being in a user defined zone
                            // inside it also noise removal, has to work fast
                        }

                        incoming_data = "";
                    }
                }
            }

            dataWrite.Close();
            dataFeed.Close();
            gp3Client.Close();
        }

        public void StartRecording(ROIConfig roiConfig, EyeTrackerConfig eyeTrackerConfig)
        {
            cancellationTokenSource = new();
            Task.Run(() => Record(roiConfig, eyeTrackerConfig, cancellationTokenSource.Token));
        }

        public void StopRecording()
        {
            cancellationTokenSource.Cancel();

            // TODO(@Vlodson): logic for saving data in GazepointReader.SignalObjectsDict to a csv
        }
    }
}