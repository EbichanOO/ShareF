using UnityEngine;
using UnityEngine.Video;

namespace PretiaArCloud.RecordingPlayback
{
    /// <summary>
    /// Helper class to store the reference to the animation curves of a
    /// clip and evaluate its value by a given time.
    /// </summary>
    public class ClipDataVideo : IClipData
    {
        private VideoPlayer _videoPlayer;
        private AnimationCurve _positionX;
        private AnimationCurve _positionY;
        private AnimationCurve _positionZ;
        private AnimationCurve _rotationX;
        private AnimationCurve _rotationY;
        private AnimationCurve _rotationZ;
        private AnimationCurve _rotationW;
        private AnimationCurve _orientation;
        private VideoClip _videoClip;
        
        public AnimationClip Clip { get; private set; }
        public RenderTexture Texture { get; private set; }
        public float Duration => Clip ? Clip.length : 0;

        public ClipDataVideo(VideoPlayer videoPlayer)
        {
            if ((object) videoPlayer == null)
            {
                throw new System.NullReferenceException();
            }
            
            _videoPlayer = videoPlayer;
            _videoPlayer.Stop();
            _videoPlayer.source = VideoSource.VideoClip;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.aspectRatio = VideoAspectRatio.NoScaling;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            _videoPlayer.hideFlags = HideFlags.HideInInspector;
        }
        
        public bool CompareClip(Object clip) { return Clip == clip; }

        public void SetClip(AnimationClip clip)
        {
            _videoPlayer.Stop();
            
            Clip = clip;
            if (clip == null)
            {
                Texture = null;
                return;
            }

#if UNITY_EDITOR
            _positionX = PlaybackUtility.GetPositionCurve(clip, 'x');
            _positionY = PlaybackUtility.GetPositionCurve(clip, 'y');
            _positionZ = PlaybackUtility.GetPositionCurve(clip, 'z');
            _rotationX = PlaybackUtility.GetRotationCurve(clip, 'x');
            _rotationY = PlaybackUtility.GetRotationCurve(clip, 'y');
            _rotationZ = PlaybackUtility.GetRotationCurve(clip, 'z');
            _rotationW = PlaybackUtility.GetRotationCurve(clip, 'w');
            _orientation = PlaybackUtility.GetOrientationCurve(clip);
            _videoClip = PlaybackUtility.GetVideoCip(clip);
#endif
            
            Texture = new RenderTexture(
                (int) _videoClip.width,
                (int) _videoClip.height,
                1000
            )
            {
                useMipMap = false,
                autoGenerateMips = false,
                wrapMode = TextureWrapMode.Clamp
            };
            
            _videoPlayer.clip = _videoClip;
            _videoPlayer.targetTexture = Texture;
        }

        public Vector3 GetPosition(float time)
        {
            time = Mathf.Clamp(time, 0, Clip.length);
            return new Vector3(
                _positionX.Evaluate(time), 
                _positionY.Evaluate(time), 
                _positionZ.Evaluate(time)
            );
        }

        public Quaternion GetRotation(float time)
        {
            time = Mathf.Clamp(time, 0, Clip.length);
            return new Quaternion(
                _rotationX.Evaluate(time), 
                _rotationY.Evaluate(time), 
                _rotationZ.Evaluate(time), 
                _rotationW.Evaluate(time)
            );
        }

        public float GetOrientation(float time)
        {
            if (Clip == null) { return 0; }
            time = Mathf.Clamp(time, 0, Clip.length);
            return _orientation.Evaluate(time);
        }

        public void UpdateTexture(float time) { }
    }
}