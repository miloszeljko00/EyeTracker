using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Runtime.InteropServices;

namespace ScreenRecorder;

public class RecorderParams
{

    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    public RecorderParams(string filename, int FrameRate, FourCC Encoder, int Quality)
    {
        FileName = filename;
        FramesPerSecond = FrameRate;
        Codec = Encoder;
        this.Quality = Quality;

        Height = GetSystemMetrics(SM_CYSCREEN);
        Width = GetSystemMetrics(SM_CXSCREEN);
    }

    string FileName;
    public int FramesPerSecond, Quality;
    FourCC Codec;

    public int Height { get; private set; }
    public int Width { get; private set; }

    public AviWriter CreateAviWriter()
    {
        return new AviWriter(FileName)
        {
            FramesPerSecond = FramesPerSecond,
            EmitIndex1 = true,
        };
    }

    public IAviVideoStream CreateVideoStream(AviWriter writer)
    {
        // Select encoder type based on FOURCC of codec
        if (Codec == KnownFourCCs.Codecs.Uncompressed)
            return writer.AddUncompressedVideoStream(Width, Height);
        else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
            return writer.AddMotionJpegVideoStream(Width, Height, Quality);
        else
        {
            return writer.AddMpeg4VideoStream(Width, Height, (double)writer.FramesPerSecond,
                // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                quality: Quality,
                codec: Codec,
                // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                forceSingleThreadedAccess: true);
        }
    }
}