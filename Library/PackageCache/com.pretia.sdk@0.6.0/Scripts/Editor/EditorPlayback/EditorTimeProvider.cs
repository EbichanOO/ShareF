using System;
using UnityEditor;

namespace PretiaArCloud.RecordingPlayback.Editor
{
    public class EditorTimeProvider : IPlaybackTimer
    {
        private float _minTime, _maxTime;
        public double Time { get; private set; }
        
        private Action<double> OnTimeUpdated { get; set; }

        public float MinTime
        {
            get => _minTime;
            set
            {
                _minTime = value < 0 ? 0 : value;
                
                if (Time < _minTime)
                {
                    Time = _minTime;
                    OnTimeUpdated?.Invoke(Time);
                }
            }
        }

        public float MaxTime
        {
            get => _maxTime;
            set
            {
                _maxTime = value < 0 ? 0 : value;
                
                if (Time > _maxTime)
                {
                    Time = _maxTime;
                    OnTimeUpdated?.Invoke(Time);
                }
            }
        }
        
        
        /// <summary>
        /// Stores the time since startup to obtain the delta time to update
        /// the time on the looping views.
        /// </summary>
        private double LastTimeSinceStartup { get; set; }

        public void SetTime(double time)
        {
            Time = time;
        }

        public void Enable(Action<double> onTimeUpdated)
        {
            OnTimeUpdated = onTimeUpdated;
            EditorApplication.update += Update;
            Time = 0;
        }

        public void Disable()
        {
            EditorApplication.update -= Update;
        }
        
        private void Update()
        {
            // Gets the delta time.
            var timeSinceStartUp = EditorApplication.timeSinceStartup;
            var deltaTime = timeSinceStartUp - LastTimeSinceStartup;
            
            // Updates the time on the looping views.
            Time += deltaTime;
            
            if (Time - MaxTime >= 0)
            {
                Time = MinTime;
            }
            
            OnTimeUpdated?.Invoke(Time);
            // Stores the time since start up.
            LastTimeSinceStartup = timeSinceStartUp;
        }
    }
}