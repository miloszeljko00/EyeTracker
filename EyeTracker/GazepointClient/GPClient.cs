using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GazepointClient
{
    class GPClient
    {
        static void Main(string[] args)
        {
            GazepointReader configReader = new();

            int ServerPort = configReader.Configuration.ServerPort;
            string ServerIP = configReader.Configuration.ServerIp;

            bool exit_state = false;
            int startindex, endindex;
            TcpClient gp3_client;
            NetworkStream data_feed;
            StreamWriter data_write;
            String incoming_data = "";

            ConsoleKeyInfo keybinput;

            // Try to create client object, return if no server found
            try
            {
                gp3_client = new TcpClient(ServerIP, ServerPort);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect with error: {0}", e);
                return;
            }

            // Load the read and write streams
            data_feed = gp3_client.GetStream();
            data_write = new StreamWriter(data_feed);

            // Setup the data records
            data_write.Write(configReader.WriteSignalXMLConfiguration());

            // Flush the buffer out the socket
            data_write.Flush();

            do
            {
                int ch = data_feed.ReadByte();
                if (ch != -1)
                {
                    incoming_data += (char)ch;

                    // find string terminator ("\r\n") 
                    if (incoming_data.IndexOf("\r\n") != -1)
                    {
                        // only process DATA RECORDS, ie <REC .... />
                        if (incoming_data.IndexOf("<REC") != -1)
                        {
                            Console.WriteLine(incoming_data);
                            configReader.ParseIncomingDataLine(incoming_data);

                            // TODO(@Vlodson): logic for labeling stuff as being in a user defined zone
                            // inside it also noise removal, has to work fast
                        }

                        incoming_data = "";
                    }
                }

                if (Console.KeyAvailable == true)
                {
                    keybinput = Console.ReadKey(true);
                    if (keybinput.Key != ConsoleKey.Q)
                    {
                        exit_state = true;
                    }
                }
            }
            while (exit_state == false);

            data_write.Close();
            data_feed.Close();
            gp3_client.Close();

            // TODO(@Vlodson): logic for saving data in configReader.SignalObjectsDict to a csv
        }
    }
}