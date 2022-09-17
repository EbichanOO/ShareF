using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ARSessionOrigin))]
    [RequireComponent(typeof(ARAnchorManager))]
    public class ARSharedAnchorManager
        : SubsystemLifecycleManager<SharedAnchorSubsystem>
    {
        [SerializeField]
        private bool _runRelocalizationOnStart = true;

        /// <summary>
        /// Indicates the mode of relocalization type.
        /// </summary>
        public enum RelocalizationTypeChoice
        {
            Image,
            CloudMap,
        }

        [SerializeField]
        [Tooltip("Will re-run relocalization automatically when encountering an error that stops the previous relocalization. Will only run when RunRelocalizationOnStart is enabled")]
        private bool _restartAutomaticallyOnError = true;

        [SerializeField]
        [Tooltip("There are two types of relocalization available with PretiaSDK, which is image-based relocalization and map-based relocalization. The RelocalizationType decides which is used when running relocalization automatically with RunRelocalizationOnStart")]
        /// <summary>
        /// Decides which relocalization type is used when running automatically with RunRelocalizationOnStart.
        /// </summary>
        public RelocalizationTypeChoice RelocalizationType = RelocalizationTypeChoice.CloudMap;

        [SerializeField]
        [Tooltip("Timeout used when running relocalization automatically with RunRelocalizationOnStart. Setting this value to 0 will let relocalization run without timeout")]
        private uint _timeoutInMilliseconds = 0;

        [SerializeReference]
        private AbstractMapSelection _mapSelection;

        [SerializeField]
        private ARTrackedImageManager _trackedImageManager;

        [SerializeField]
        private int _selectedReferenceImageIndex;
        private ARCameraManager _cameraManager;

        private TaskCompletionSource<bool> _requestingLocationPermission;
        private Transform _unityOriginTransform;
        private ARSessionOrigin _arSessionOrigin;
        private InverseDisplayMatrix _displayMatInv;
        private CancellationTokenSource _getMapSelectionCts;

        /// <summary>
        /// Indicates how close the current frame is to successful relocalization. Ranges from 0f until 1f. Value of 1f means that it is close to succesful relocalization.
        /// </summary>
        public float RelocScore => _subsystem.RelocScore;

        /// <summary>
        /// Indicates the current state of relocalization.
        /// </summary>
        public SharedAnchorState CurrentState => _subsystem.CurrentState;

        /// <summary>
        /// Emits an event everytime the state of the shared anchor changes.
        /// </summary>
        public event SharedAnchorStateChanged OnSharedAnchorStateChanged;

        public delegate void RelocalizedEvent();
        public delegate void ImageRelocalizedEvent(XRReferenceImage referenceImage);
        public delegate void MapRelocalizedEvent(string mapKey);
        public delegate void RequestPermissionEvent();

        /// This is a duplicated event for convenience
        /// <summary>
        /// An event that indicates that a successful relocalization has occured.
        /// </summary>
        public event RelocalizedEvent OnRelocalized;

        /// <summary>
        /// An event that indicates that a successful image relocalization has occured.
        /// </summary>
        public event ImageRelocalizedEvent OnImageRelocalized;

        /// <summary>
        /// An event that indicates that a successful cloud map relocalization has occured.
        /// </summary>
        public event MapRelocalizedEvent OnMapRelocalized;

        /// <summary>
        /// An event that indicates that a the location permission has been granted from the user.
        /// </summary>
        public event RequestPermissionEvent OnLocationPermissionGranted;

        /// <summary>
        /// An event that indicates that a the location permission has been denied from the user.
        /// </summary>
        public event RequestPermissionEvent OnLocationPermissionDenied;

        /// <summary>
        /// An event that indicates an unexpected error had occured in the shared anchor subsystem.
        /// </summary>
        public event SharedAnchorSubsystemException OnException;

        /// <summary>
        /// An event that is emitted every time the score changed, which is after every frame that has finished evaluation.
        /// </summary>
        public event SharedAnchorScoreUpdated OnScoreUpdated;

        /// <summary>
        /// An event that is emitted every time the keypoints changed, which is after every frame that has finished evaluation.
        /// </summary>
        public event SharedAnchorKeypointsUpdated OnKeypointsUpdated;

        protected override void Awake()
        {
            base.Awake();

            var unityOriginObject = new GameObject("Unity Origin");
            _unityOriginTransform = unityOriginObject.transform;

            _arSessionOrigin = GetComponent<ARSessionOrigin>();
            if (_arSessionOrigin == null)
            {
                throw new Exception($"{nameof(ARSharedAnchorManager)} requires to be in the same GameObject as {nameof(ARSessionOrigin)}");
            }

            var arCameras = GetComponentsInChildren<ARCameraManager>();
            _cameraManager = arCameras.FirstOrDefault(c => c.GetComponent<ARPoseDriver>() != null);

            if (_cameraManager == null)
            {
                throw new Exception("There is no camera with ARPoseDriver found in the child objects of ARSharedAnchorManager");
            }

            _subsystem.SetTrackedImageManager(_trackedImageManager);
            _subsystem.SetARPoseDriver(_cameraManager.GetComponent<ARPoseDriver>());
            _subsystem.SetInternalAnchorManager(new InternalAnchorManager(_arSessionOrigin, GetComponent<ARAnchorManager>()));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _subsystem.OnSharedAnchorStateChanged += ProcessSharedAnchorStateChanged;
            _subsystem.OnSharedAnchorPoseUpdated += UpdateSharedAnchorPose;
            _subsystem.OnException += ProcessExceptionEvent;
            _subsystem.OnSharedAnchorScoreUpdated += ProcessScore;
            _subsystem.OnSharedAnchorKeypointsUpdated += ProcessKeypoints;
        }

        private void OnFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            if (_displayMatInv == null)
            {
                _displayMatInv = new InverseDisplayMatrix();
                _displayMatInv.SetImageResolution(640, 480); // TODO
            }

            _displayMatInv.UpdateMatrix(eventArgs.displayMatrix);
        }

        private void Start()
        {
            if (_runRelocalizationOnStart)
            {
                switch(RelocalizationType)
                {
                    case RelocalizationTypeChoice.Image:
                        StartImageRelocalization((int)_timeoutInMilliseconds);
                    break;

                    case RelocalizationTypeChoice.CloudMap:
                        StartCloudMapRelocalization((int)_timeoutInMilliseconds);
                    break;
                }
            }
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                if (_requestingLocationPermission != null)
                {
#if PLATFORM_ANDROID
                    bool locationPermissionAuthorized = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
                    _requestingLocationPermission.SetResult(locationPermissionAuthorized);
#elif !UNITY_EDITOR && UNITY_IOS
                    var authorizationStatus = IOSLocationPermission.GetAuthorizationStatus();
                    bool locationPermissionAuthorized = authorizationStatus == IOSLocationPermission.Status.AuthorizedAlways || authorizationStatus == IOSLocationPermission.Status.AuthorizedWhenInUse;
                    _requestingLocationPermission.SetResult(locationPermissionAuthorized);
#endif
                }
            }
        }

        private void ProcessExceptionEvent(Exception e)
        {
            OnException?.Invoke(e);

            if (_runRelocalizationOnStart && _restartAutomaticallyOnError)
            {
                Start();
            }
        }

        private void UpdateSharedAnchorPose(Pose sharedAnchorPose)
        {
            // Shared Anchor pose relative to the session origin
            var sessionRelativePose = _arSessionOrigin.trackablesParent.TransformPose(sharedAnchorPose);

            // Use MakeContentAppearAt to update the session origin tranlation (because it uses an intermediate game object)
            _arSessionOrigin.MakeContentAppearAt(_unityOriginTransform, sessionRelativePose.position);

            // Update the session origin rotation directly
            _arSessionOrigin.transform.rotation = Quaternion.Inverse(sharedAnchorPose.rotation);
        }

        private void ProcessSharedAnchorStateChanged(SharedAnchorState newState)
        {
            if (newState == SharedAnchorState.Initializing)
            {
                _cameraManager.frameReceived += OnFrameReceived;
            }
            else if (newState == SharedAnchorState.Stopped)
            {
                _cameraManager.frameReceived -= OnFrameReceived;
            }

            OnSharedAnchorStateChanged?.Invoke(newState);

            if (newState == SharedAnchorState.Relocalized)
            {
                var relocalizer = _subsystem.GetActiveRelocalizer();

                if (relocalizer is CloudMapRelocalizer)
                    OnMapRelocalized?.Invoke(_subsystem.GetMapKey());

                else if (relocalizer is ImageRelocalizer)
                    OnImageRelocalized?.Invoke(_subsystem.GetReferenceImage());

                OnRelocalized?.Invoke();
            }
        }

        private void ProcessScore(float score)
        {
            OnScoreUpdated?.Invoke(score);
        }

        private void ProcessKeypoints(float[] keypoints)
        {
            if (keypoints != null && _displayMatInv != null)
            {
                // Transform the keypoints from image coordinates to screen coordinates
                // Note: Most of the time, there are not many keypoints, so this should be fast,
                //       but when approaching reloc there might be more points,
                //       so we may need to move this to a shader if that's how we display the points eventually.
                float[] screenKeypoints = new float[keypoints.Length];
                for (int i = 0; i < keypoints.Length; i+=2)
                {
                    _displayMatInv.ComputeScreenPoint(
                        keypoints[i], keypoints[i+1],
                        out screenKeypoints[i], out screenKeypoints[i+1]);
                }

                OnKeypointsUpdated?.Invoke(screenKeypoints);
            }
            else
            {
                OnKeypointsUpdated?.Invoke(null);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subsystem.OnSharedAnchorStateChanged -= ProcessSharedAnchorStateChanged;
            _subsystem.OnSharedAnchorPoseUpdated -= UpdateSharedAnchorPose;
            _subsystem.OnException -= ProcessExceptionEvent;
            _subsystem.OnSharedAnchorScoreUpdated -= ProcessScore;
            _subsystem.OnSharedAnchorKeypointsUpdated -= ProcessKeypoints;
        }

        /// <summary>
        /// Starts an image-based relocalization with the default reference image set in the inspector of this component.
        /// </summary>
        /// <param name="timeoutInMilliseconds">The relocalization execution will be stopped if it exceeds the timeout limit. Value of 0 will run the execution without any timeout. Defaults to 0 if not specified.</param>
        public void StartImageRelocalization(int timeoutInMilliseconds = 0)
        {
            StartImageRelocalization(default, timeoutInMilliseconds);
        } 

        /// <summary>
        /// Starts an image-based relocalization with the specified reference image.
        /// </summary>
        /// <param name="referenceImage">The reference image to use as the shared anchor</param>
        /// <param name="timeoutInMilliseconds">The relocalization execution will be stopped if it exceeds the timeout limit. Value of 0 will run the execution without any timeout. Defaults to 0 if not specified.</param>
        public void StartImageRelocalization(XRReferenceImage referenceImage, int timeoutInMilliseconds = 0)
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if (referenceImage == default)
            {
                if (_trackedImageManager == null)
                {
                    throw new Exception("Reference image library is not set for the ARTrackedImageManager used in ARSharedAnchorManager");
                }

                referenceImage = _trackedImageManager.referenceLibrary[_selectedReferenceImageIndex];
            }

            _subsystem.SetReferenceImage(referenceImage);
            _subsystem.StartRelocalization(SharedAnchorSubsystem.SharedAnchorTypeChoice.Image, timeoutInMilliseconds);
#else
            OnImageRelocalized?.Invoke(referenceImage);
            OnRelocalized?.Invoke();
#endif
        }

        /// <summary>
        /// Starts an cloud map-based relocalization using the default map selection implementation in the inspector of this component.
        /// </summary>
        /// <param name="timeoutInMilliseconds">The relocalization execution will be stopped if it exceeds the timeout limit. Value of 0 will run the execution without any timeout. Defaults to 0 if not specified.</param>
        public void StartCloudMapRelocalization(int timeoutInMilliseconds = 0) => StartCloudMapRelocalization(default, timeoutInMilliseconds);

        /// <summary>
        /// Starts an cloud map-based relocalization with the specified map key.
        /// </summary>
        /// <param name="mapKey">The map key to use for the cloud relocalization</param>
        /// <param name="timeoutInMilliseconds">The relocalization execution will be stopped if it exceeds the timeout limit. Value of 0 will run the execution without any timeout. Defaults to 0 if not specified.</param>
        public async void StartCloudMapRelocalization(string mapKey, int timeoutInMilliseconds = 0)
        {
            if (string.IsNullOrEmpty(mapKey))
            {
                if (_mapSelection == null)
                {
                    throw new Exception("MapSelectionStrategy is not set for ARSharedAnchorManager");
                }

                if (_mapSelection is CriteriaBasedMapSelection)
                {
#if PLATFORM_ANDROID
                    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                    {
                        _requestingLocationPermission = new TaskCompletionSource<bool>();
                        Permission.RequestUserPermission(Permission.FineLocation);

                        bool locationPermissionGranted = await _requestingLocationPermission.Task;
                        _requestingLocationPermission = null;
                        if (locationPermissionGranted)
                        {
                            OnLocationPermissionGranted?.Invoke();
                        }
                        else
                        {
                            OnLocationPermissionDenied?.Invoke();
                            return;
                        }
                    }
#elif !UNITY_EDITOR && UNITY_IOS
                    var authorizationStatus = IOSLocationPermission.GetAuthorizationStatus();
                    switch(authorizationStatus)
                    {
                        case IOSLocationPermission.Status.NotDetermined:
                            _requestingLocationPermission = new TaskCompletionSource<bool>();
                            Input.location.Start(desiredAccuracyInMeters: 1f, updateDistanceInMeters: 5f);

                            bool locationPermissionGranted = await _requestingLocationPermission.Task;
                            _requestingLocationPermission = null;
                            if (locationPermissionGranted)
                            {
                                OnLocationPermissionGranted?.Invoke();
                            }
                            else
                            {
                                OnLocationPermissionDenied?.Invoke();
                                return;
                            }
                        break;

                        case IOSLocationPermission.Status.Restricted:
                        case IOSLocationPermission.Status.Denied:
                            OnLocationPermissionDenied?.Invoke();
                            return;
                    }
#endif
                }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                try
                {
                    _getMapSelectionCts = new CancellationTokenSource(); 
                    mapKey = await _mapSelection.GetMapSelectionAsync(_getMapSelectionCts.Token);
                }
                catch (Exception e)
                {
                    OnException?.Invoke(e);
                }
#endif
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            _subsystem.SetMapKey(mapKey);
            _subsystem.StartRelocalization(SharedAnchorSubsystem.SharedAnchorTypeChoice.CloudMap, timeoutInMilliseconds);
#else
            OnMapRelocalized?.Invoke(mapKey);
            OnRelocalized?.Invoke();
#endif
        }

        public void _internal_StartLocalRelocalization(string mapPath = "", int timeoutInMilliseconds = 0)
        {
            _subsystem._internal_StartLocalRelocalization(mapPath, timeoutInMilliseconds);
        }

        /// <summary>
        /// Resets the shared anchor subsystem. Will stop the ongoing relocalization execution if it is running.
        /// </summary>
        public void ResetSharedAnchor()
        {
            if (_getMapSelectionCts != null && !_getMapSelectionCts.Token.IsCancellationRequested)
            {
                _getMapSelectionCts.Cancel();
            }

            _subsystem.Reset();
        }
    }
}