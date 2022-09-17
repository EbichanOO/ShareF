using UnityEngine;
using UnityEngine.Video;

namespace PretiaArCloud.RecordingPlayback.Editor
{
    public class VideoTimeProvider : IPlaybackTimer
    {
        public double Time { get; private set; }
        public float MinTime { get; set; }
        public float MaxTime { get; set; }
        private System.Action<double> OnTimeUpdated { get; set; }
        private VideoPlayer VideoPlayer { get; set; }

        public VideoTimeProvider(VideoPlayer videoPlayer)
        {
            if (videoPlayer == null)
            {
                throw new System.NullReferenceException();
            }
            VideoPlayer = videoPlayer;
            VideoPlayer.playOnAwake = false;
            VideoPlayer.waitForFirstFrame = false;
            VideoPlayer.skipOnDrop = true;
            VideoPlayer.isLooping = true;
            VideoPlayer.sendFrameReadyEvents = true;
            VideoPlayer.frameReady += OnFrameReady;
        }

        public void Enable(System.Action<double> onTimeUpdated)
        {
            OnTimeUpdated = onTimeUpdated;
            VideoPlayer.Pause();
            VideoPlayer.time = MinTime;
            VideoPlayer.prepareCompleted += OnPrepared;
            
            if (!VideoPlayer.isPrepared)
            {
                VideoPlayer.Prepare();
            }
            
            VideoPlayer.Play();
        }

        public void SetTime(double time)
        {
            if(Mathf.Abs((float)time - (float) VideoPlayer.time) < 0.01f)
                return;
            
            VideoPlayer.time = time;
            
            if (VideoPlayer.isPlaying) { return; }
            
            if (!VideoPlayer.isPrepared)
            {
                VideoPlayer.Prepare();
            }
            else
            {
                VideoPlayer.Play();
            }
        }

        private void OnPrepared(VideoPlayer source)
        {
            VideoPlayer.prepareCompleted -= OnPrepared;
            VideoPlayer.Play();
        }
        
        private void OnFrameReady(VideoPlayer source, long frame)
        {
            if (!VideoPlayer.isPlaying) { return; }
            
            Time = VideoPlayer.time;
            if (Time > MaxTime || Time < MinTime)
            {
                VideoPlayer.Pause();
                VideoPlayer.time = MinTime;
                VideoPlayer.Play();
                return;
            }
            
            Time = VideoPlayer.time;
            OnTimeUpdated?.Invoke(Time);
        }

        public void Disable()
        {
            VideoPlayer.prepareCompleted -= OnPrepared;
            VideoPlayer.Stop();
            OnTimeUpdated = null;
        }

        ~ VideoTimeProvider()
        {
            VideoPlayer.frameReady -= OnFrameReady;
            VideoPlayer.prepareCompleted -= OnPrepared;
        }
    }
}