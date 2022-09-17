using UnityEngine;
using UnityEngine.Video;

namespace PretiaArCloud.RecordingPlayback
{
    [ExecuteInEditMode]
    public class PlaybackBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Current position of the playback.
        /// </summary>
        [HideInInspector]
        [SerializeField] 
        private Vector3 position;
        
        /// <summary>
        /// Current rotation on the playback.
        /// </summary>
        [HideInInspector]
        [SerializeField] 
        private Quaternion rotation;

        /// <summary>
        /// Orientation in which the video was recorded.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private float orientation;

        /// <summary>
        /// Current playback texture.
        /// </summary>
        [HideInInspector]
        [SerializeField] 
        private Texture2D texture2D;

        /// <summary>
        /// Current playback video clip.
        /// </summary>
        [HideInInspector]
        [SerializeField] 
        private VideoClip videoClip;

        /// <summary>
        /// Current position of the playback.
        /// </summary>
        public Vector2 Position => position;

        /// <summary>
        /// Current rotation on the playback.
        /// </summary>
        public Quaternion Rotation => rotation;

        /// <summary>
        /// Current playback texture.
        /// </summary>
        public Texture2D Texture2D => texture2D;

        /// <summary>
        /// Current playback video clip.
        /// </summary>
        public VideoClip VideoClip => videoClip;
    }
}