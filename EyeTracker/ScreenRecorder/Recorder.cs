using SharpAvi.Output;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ScreenRecorder
{
    public class Recorder : IDisposable
    {
        #region Fields
        AviWriter writer;
        RecorderParams Params;
        IAviVideoStream videoStream;
        Thread screenThread;
        ManualResetEvent stopThread = new ManualResetEvent(false);
        #endregion

        public Recorder(RecorderParams Params)
        {
            this.Params = Params;

            // Create AVI writer and specify FPS
            writer = Params.CreateAviWriter();

            // Create video stream
            videoStream = Params.CreateVideoStream(writer);
            // Set only name. Other properties were set when creating the stream, 
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Recorder";

            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };

            screenThread.Start();
        }

        public void Dispose()
        {
            stopThread.Set();
            screenThread.Join();

            // Close writer: the remaining data is written to a file, and the file is closed
            writer.Close();

            stopThread.Dispose();
        }

        void RecordScreen()
        {
            var buffer = new byte[Params.Width * Params.Height * 4];
            Stopwatch stopwatch = new Stopwatch();

            while (!stopThread.WaitOne(1)) // Use a small wait to avoid busy-waiting
            {
                stopwatch.Restart();

                Screenshot(buffer);

                // Wait for the previous frame to be written
                videoStream.WriteFrame(true, buffer, 0, buffer.Length);

                stopwatch.Stop();

                // Calculate the time to wait until the next frame
                var frameInterval = TimeSpan.FromSeconds(Decimal.ToDouble((1.0M / writer.FramesPerSecond)));
                var timeToWait = frameInterval - stopwatch.Elapsed;

                if (timeToWait > TimeSpan.Zero)
                {
                    Thread.Sleep(timeToWait);
                }
            }
        }

        public void Screenshot(byte[] buffer)
        {
            using var bmp = new Bitmap(Params.Width, Params.Height, PixelFormat.Format32bppRgb);
            using var g = Graphics.FromImage(bmp);
            // Capture the screen directly to the Bitmap
            g.CopyFromScreen(0, 0, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            // Lock the bits of the Bitmap for direct access
            var bits = bmp.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            try
            {
                // Copy the pixel data to the buffer
                Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
            }
            finally
            {
                // Unlock the bits
                bmp.UnlockBits(bits);
            }
        }
    }
}
