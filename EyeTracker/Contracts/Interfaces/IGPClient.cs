using Contracts;

namespace GazepointClient.Interfaces;

public interface IGPClient
{
    void StartRecording(ROIConfig roiConfig, EyeTrackerConfig eyeTrackerConfig, string recordingId);
    void StopRecording();
    string GetRecordingFilePath(string roiConfigId);
}
