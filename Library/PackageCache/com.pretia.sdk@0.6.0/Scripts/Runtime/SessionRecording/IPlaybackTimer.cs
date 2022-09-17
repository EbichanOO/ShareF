using System;

namespace PretiaArCloud.RecordingPlayback
{
    public interface IPlaybackTimer
    {
        double Time { get; }
        float MinTime { get; set; }
        float MaxTime { get; set; }
        void SetTime(double time);
        void Enable(Action<double> onTimeUpdated);
        void Disable();
    }
}