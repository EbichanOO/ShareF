using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace PretiaArCloud.RecordingPlayback
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(Projector))]
    public class PlaybackView : MonoBehaviour
    {
#if UNITY_EDITOR
        
#region Fields

        /// <summary>
        /// Clip to play back.
        /// </summary>
        [SerializeField]
        private AnimationClip clip;

        /// <summary>
        /// Whether to loop the clip.
        /// </summary>
        [SerializeField] 
        private bool loopTime;

        /// <summary>
        /// Current time of the view.
        /// </summary>
        [SerializeField]
        private double time;

        /// <summary>
        /// Min time of the loop.
        /// </summary>
        [SerializeField] 
        private float minTime;
        
        /// <summary>
        /// Max time of the loop.
        /// </summary>
        [SerializeField]
        private float maxTime;

        /// <summary>
        /// Video player.
        /// </summary>
        [SerializeField] 
        private VideoPlayer videoPlayer;

        /// <summary>
        /// Color of the frustum.
        /// </summary>
        [SerializeField] 
        private Color gizmoColor = Color.white;

        /// <summary>
        /// Whether to draw the frustum.
        /// </summary>
        [SerializeField] 
        private bool drawFrustum;
        
        /// <summary>
        /// Camera where to show the playback.
        /// </summary>
        [SerializeField] 
        private new Camera camera;

        /// <summary>
        /// Projector for materials.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private Projector projector;

        /// <summary>
        /// Layer where to project.
        /// </summary>
        [SerializeField]
        private LayerMask projectionLayer;

#endregion

#region Properties

        /// <summary>
        /// List of available views.
        /// </summary>
        public static List<PlaybackView> Views { get; } = 
            new List<PlaybackView>();
        
        /// <summary>
        /// Clip to play back.
        /// </summary>
        public AnimationClip Clip
        {
            get => clip;
            set => clip = value;
        }
        
        /// <summary>
        /// Current time of the view.
        /// </summary>
        public double Time
        {
            get => time;
            private set => time = value;
        }

        public int TargetDisplay
        {
            get => camera ? camera.targetDisplay : 0;
            set
            {
                if (!camera || value < 0)
                {
                    return;
                }

                camera.targetDisplay = value;
            }
        }

        /// <summary>
        /// Whether to loop the clip.
        /// </summary>
        public bool LoopTime
        {
            get => loopTime;
            set
            {
                loopTime = value;
            }
        }

        /// <summary>
        /// Whether to draw the frustum.
        /// </summary>
        public bool DrawFrustum
        {
            get => drawFrustum;
            set => drawFrustum = value;
        }

        /// <summary>
        /// Color of the frustum.
        /// </summary>
        public Color GizmoColor
        {
            get => gizmoColor;
            set => gizmoColor = value;
        }

        public VideoPlayer VideoPlayer => videoPlayer;

        /// <summary>
        /// Current position of the view.
        /// </summary>
        private Vector3 Position { get; set; }
        
        /// <summary>
        /// Current rotation of the view.
        /// </summary>
        private Quaternion Rotation { get; set; }
        
        /// <summary>
        /// Current rotation of the view.
        /// </summary>
        private float Orientation { get; set; }
        
        private Coroutine SetOrientationCoroutine { get; set; }
        
        private BackgroundDisplay BackgroundDisplay { get; set; }
        private BackgroundProjector BackgroundProjector { get; set; }
        private IClipData ClipData { get; set; }
        
        public IPlaybackTimer Timer { get; set; }

        /// <summary>
        /// Min time of the loop.
        /// </summary>
        public float MinTime
        {
            get => minTime;
            set => minTime = value;
        }

        /// <summary>
        /// Max time of the loop.
        /// </summary>
        public float MaxTime
        {
            get => maxTime;
            set => maxTime = value;
        }

        #endregion

#region Methods

        /// <summary>
        /// Called on reset.
        /// </summary>
        private void Reset()
        {
            camera = GetComponent<Camera>();
            videoPlayer = GetComponent<VideoPlayer>();
            projector = GetComponent<Projector>();
        }
        
        /// <summary>
        /// Called on awake.
        /// </summary>
        private void Awake()
        {
            if (camera == null) { camera = GetComponent<Camera>(); }
            if (projector == null) { projector = GetComponent<Projector>(); }
            if (videoPlayer == null) { videoPlayer = GetComponent<VideoPlayer>(); }
            transform.hideFlags = HideFlags.NotEditable;
        }

        /// <summary>
        /// Called on enable.
        /// </summary>
        private void OnEnable()
        {
            if (ClipData == null) ClipData = new ClipDataVideo(videoPlayer);
            if (BackgroundDisplay == null) BackgroundDisplay = new BackgroundDisplay(camera);
            if (BackgroundProjector == null) BackgroundProjector = new BackgroundProjector(projector, camera);
            
            BackgroundDisplay.AddBuffer();
            RecalculateProjectionLayer();
            OnClipChanged();
            UpdateData();
            
            if (!Views.Contains(this)) { Views.Add(this); }
        }

        /// <summary>
        /// Called on disable.
        /// </summary>
        private void OnDisable()
        {
            BackgroundDisplay.RemoveBuffer();
        }

        /// <summary>
        /// Called on destroy.
        /// </summary>
        private void OnDestroy()
        {
            if (Views.Contains(this)) { Views.Remove(this); }
        }
        
        /// <summary>
        /// Emulates an update MonoBehaviour method. Makes the loop animation.
        /// </summary>
        public void SetTime(double time)
        {
            Time = time;
            UpdateData();
            Update();
        }

        /// <summary>
        /// Called on update.
        /// </summary>
        public void Update()
        {
            BackgroundProjector.AutoEnableByLayerMask(projectionLayer);

            var t = transform;
            t.position = Position;
            t.rotation = Rotation;
            t.Rotate(Vector3.forward, Orientation, Space.Self );
        }

        /// <summary>
        /// Called on draw gizmos.
        /// </summary>
        public void OnDrawGizmos()
        {
            if (!DrawFrustum) { return; }
            
            var t = transform;
            var lastColor = Gizmos.color;
            Gizmos.color = GizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(
                t.position, 
                t.rotation, 
                new Vector3(camera.aspect, 1.0f, 1.0f) 
            );
            
            Gizmos.DrawFrustum( 
                Vector3.zero, 
                camera.fieldOfView, 
                camera.farClipPlane, 
                camera.nearClipPlane,  
                1.0f
            );
            Gizmos.color = lastColor;
        }

        /// <summary>
        /// Event function that Unity calls after a Camera has finished
        /// rendering, that allows you to modify the Camera's final image.
        /// </summary>
        /// <param name="src">A RenderTexture containing the source
        /// image.</param>
        /// <param name="dest">The RenderTexture to update with the
        /// modified image.</param>
        // TODO: Add support to Scriptable Render Pipelines.
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // Just passes the render.
            if (ClipData.Texture == null)
            {
                Graphics.Blit(src, dest);
                return;
            }
            
            BackgroundDisplay.Blit(src, dest);
        }

        /// <summary>
        /// Evaluates the clip by the current time and
        /// detect changes on it.
        /// </summary>
        public void UpdateData()
        {
            if (!ClipData.CompareClip(Clip))
            {
                OnClipChanged();
                return;
            }

            if ((object) clip == null)
            {
                return;
            }

            var t = (float)Time;
            var fixedTime = FixTime(t);
            
            Position = ClipData.GetPosition(fixedTime);
            Rotation = ClipData.GetRotation(fixedTime);
            var orientation = ClipData.GetOrientation(fixedTime);

            if (Mathf.Abs(Orientation - orientation) >= 1f)
            {
                Orientation = orientation;
                if (SetOrientationCoroutine != null)
                {
                    StopCoroutine(SetOrientationCoroutine);
                }
                SetOrientationCoroutine = StartCoroutine(
                    SetAspectRatio(orientation)
                );
            }
            BackgroundDisplay.SetRotationAngle(Orientation);
            
            ClipData.UpdateTexture(t);
        }

        public void OnClipChanged()
        {
            ClipData.SetClip(Clip);
            Time = 0;
            BackgroundProjector.SetBackgroundTexture(ClipData.Texture);
            BackgroundDisplay.SetBackgroundTexture(ClipData.Texture);
            
            if (SetOrientationCoroutine != null)
            {
                StopCoroutine(SetOrientationCoroutine);
            }
            SetOrientationCoroutine = StartCoroutine(
                SetAspectRatio(ClipData.GetOrientation(0))
            );
        }

        public void RecalculateProjectionLayer()
        {
            BackgroundProjector.SetProjectionLayer(projectionLayer);
        }
        
        private IEnumerator SetAspectRatio(float orientation)
        {
            yield return null;
            yield return null;
            var texture = ClipData?.Texture;
            if ((object)texture == null) { yield break; }
            var textureAspect = texture.width / (float) texture.height;
            var targetAspect = Mathf.Round(orientation) % 180 != 0
                ? 1 / textureAspect
                : textureAspect;

            float targetHeight, targetWidth, offsetX = 0, offsetY = 0;
            
            if (Screen.width / (float) Screen.height > targetAspect)
            {
                targetHeight = Screen.height;
                targetWidth = targetHeight * targetAspect; 
                offsetX = (Screen.width - targetWidth) / 2f; 
            }
            else
            {
                targetWidth = Screen.width;
                targetHeight = targetWidth / targetAspect;
                offsetY = (Screen.height - targetHeight) / 2f; 
            }
            
            BackgroundProjector.SetAspectRatio(textureAspect);
            BackgroundDisplay.SetAspectRatio(
                new Rect(offsetX, offsetY, targetWidth, targetHeight), 
                textureAspect
            );
        }
        
        public float FixTime(float time)
        {
            if (clip == null || videoPlayer == null || videoPlayer.length <= 0)
            {
                return 0;
            }

            var percentage = time / (float) videoPlayer.length;
            return percentage * clip.length;
        }

#endregion

#endif
    }
}
