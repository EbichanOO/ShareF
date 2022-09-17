using UnityEngine;

namespace PretiaArCloud.RecordingPlayback
{
    /// <summary>
    /// Manager of views.
    /// </summary>
    public class PlaybackViewManager : MonoBehaviour
    {
        /// <summary>
        /// List of the views to manage.
        /// </summary>
        [SerializeField] 
        private PlaybackView[] views;
    }
}