using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace PretiaArCloud
{
    public delegate void SharedAnchorStateChanged(SharedAnchorState newState);
    internal delegate void SharedAnchorPoseUpdated(Pose sharedAnchorPose);
    public delegate void SharedAnchorSubsystemException(Exception e);
    public delegate void SharedAnchorScoreUpdated(float score);
    public delegate void SharedAnchorKeypointsUpdated(float[] keypoints);

    public sealed partial class SharedAnchorSubsystem : ISubsystem
    {
        internal class Factory
        {
            public static SharedAnchorSubsystem Create(PretiaSDKProjectSettings sdkSettings)
            {
                XRSessionSubsystem sessionSubsystem = null;
                XRCameraSubsystem cameraSubsystem = null;
                if (Utility.IsEditor() || Utility.IsStandalone())
                {
                    sessionSubsystem = new MockSessionSubsystem();
                    cameraSubsystem = new MockCameraSubsystem();
                }
                else
                {
                    if (!XRGeneralSettings.Instance.TryGetSubsystem<XRSessionSubsystem>(out sessionSubsystem))
                    {
                        throw new NullReferenceException(nameof(sessionSubsystem));
                    }
                    if (!XRGeneralSettings.Instance.TryGetSubsystem<XRCameraSubsystem>(out cameraSubsystem))
                    {
                        throw new NullReferenceException(nameof(cameraSubsystem));
                    }
                }

                var cameraManager = new SyncedCameraManager(cameraSubsystem);
                var nativeApi = new NativeApi();
                return new SharedAnchorSubsystem(
                    sessionSubsystem: sessionSubsystem,

                    localMapRelocalizer: new LocalMapRelocalizer(
                        cameraManager,
                        nativeApi),

                    cloudMapRelocalizer: new CloudMapRelocalizer(
                        sdkSettings.RelocServerAddress,
                        sdkSettings.RelocServerPort,
                        sdkSettings.AppKey,
                        cameraManager,
                        nativeApi),

                    imageRelocalizer: new ImageRelocalizer(),

                    syncedCameraManager: cameraManager
                );
            }
        }

        private const int CONSECUTIVE_SUCCESSFUL_RELOC_TARGET = 3;
        private const int FAILED_ATTEMPT_BEFORE_COOLDOWN = 10;
        private const float COOLDOWN_TIME_IN_SECOND = 1f;
        private const float MAX_DISTANCE_DELTA = .2f;
        private const float MAX_ANGLE_DELTA = 30f;

        private XRSessionSubsystem _sessionSubsystem;
        private LocalMapRelocalizer _localMapRelocalizer;
        private CloudMapRelocalizer _cloudMapRelocalizer;
        private ImageRelocalizer _imageRelocalizer;
        private InternalAnchorManager _anchorManager;
        private SyncedCameraManager _syncedCameraManager;

        private ARAnchor _cameraAnchor;
        private ARAnchor _sharedAnchor;
        private List<ARAnchor> _relocAnchors = new List<ARAnchor>(CONSECUTIVE_SUCCESSFUL_RELOC_TARGET);
        private IRelocalizer _activeRelocalizer;

        private bool _isRunning = false;
        private Task _previousRelocalizationTask;
        private Task _relocalizationTask;
        private CancellationTokenSource _relocalizationCancelSource;

        public float RelocScore { get; private set; } = 0.0f;

        public SharedAnchorState CurrentState { get; private set; } = SharedAnchorState.Stopped;

        public event SharedAnchorStateChanged OnSharedAnchorStateChanged;
        internal event SharedAnchorPoseUpdated OnSharedAnchorPoseUpdated;
        public event SharedAnchorSubsystemException OnException;
        public event SharedAnchorScoreUpdated OnSharedAnchorScoreUpdated;
        public event SharedAnchorKeypointsUpdated OnSharedAnchorKeypointsUpdated;

        internal enum SharedAnchorTypeChoice
        {
            Image,
            CloudMap,
            LocalMap,
        }

        internal SharedAnchorSubsystem(
            XRSessionSubsystem sessionSubsystem,
            LocalMapRelocalizer localMapRelocalizer,
            CloudMapRelocalizer cloudMapRelocalizer,
            ImageRelocalizer imageRelocalizer,
            SyncedCameraManager syncedCameraManager) : base()
        {
            _sessionSubsystem = sessionSubsystem;
            _localMapRelocalizer = localMapRelocalizer;
            _cloudMapRelocalizer = cloudMapRelocalizer;
            _imageRelocalizer = imageRelocalizer;
            _syncedCameraManager = syncedCameraManager;
        }

        internal void SetARPoseDriver(ARPoseDriver arPoseDriver)
        {
            _syncedCameraManager.SetARPoseDriver(arPoseDriver);
        }

        internal void SetInternalAnchorManager(InternalAnchorManager anchorManager)
        {
            _anchorManager = anchorManager;
        }

        internal void SetMapKey(string mapKey)
        {
            _cloudMapRelocalizer.SetMapKey(mapKey);
        }

        internal string GetMapKey() => _cloudMapRelocalizer.GetMapKey();

        internal void SetTrackedImageManager(ARTrackedImageManager trackedImageManager)
        {
            _imageRelocalizer.SetTrackedImageManager(trackedImageManager);
        }

        internal void SetReferenceImage(XRReferenceImage referenceImage)
        {
            _imageRelocalizer.SetReferenceImage(referenceImage);
        }

        internal XRReferenceImage GetReferenceImage() => _imageRelocalizer.GetReferenceImage();

        internal IRelocalizer GetActiveRelocalizer() => _activeRelocalizer;

        public void Initialize()
        {
            
        }

        public void Resume()
        {
            _isRunning = true;
        }

        internal void StartRelocalization(SharedAnchorTypeChoice sharedAnchorType, int timeoutInMilliseconds)
        {
            // Reset if already started
            if (CurrentState != SharedAnchorState.Stopped)
            {
                Reset();
            }
            
            // Prepare the task
            if (timeoutInMilliseconds == 0)
            {
                _relocalizationCancelSource = new CancellationTokenSource();
            }
            else
            {
                _relocalizationCancelSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutInMilliseconds));
            }
            _relocalizationTask = Run(sharedAnchorType, _relocalizationCancelSource.Token);

            _relocalizationTask.ContinueWith(
                _ => ResetInternal(),
                cancellationToken: default,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            _relocalizationTask.ContinueWith(
                t => {
                    // Debug.LogException(t.Exception);
                    OnException?.Invoke(t.Exception.InnerException);
                },
                cancellationToken: default,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        internal void _internal_StartLocalRelocalization(string mapPath = "", int timeoutInMilliseconds = 0)
        {
            _localMapRelocalizer.SetMapPath(mapPath);
            StartRelocalization(SharedAnchorTypeChoice.LocalMap, timeoutInMilliseconds);
        }

        private async Task Run(
            SharedAnchorTypeChoice sharedAnchorType,
            CancellationToken cancellationToken = default)
        {
            if (_previousRelocalizationTask != null && _previousRelocalizationTask.Status == TaskStatus.Running)
            {
                await _previousRelocalizationTask;
            }

            switch (sharedAnchorType)
            {
                case SharedAnchorTypeChoice.CloudMap:
                    _activeRelocalizer = _cloudMapRelocalizer;
                    await RunMapRelocalizationAsync(cancellationToken);
                break;

                case SharedAnchorTypeChoice.LocalMap:
                    _activeRelocalizer = _localMapRelocalizer;
                    await RunMapRelocalizationAsync(cancellationToken);
                break;

                case SharedAnchorTypeChoice.Image:
                    _activeRelocalizer = _imageRelocalizer;
                    await RunImageRelocalizationAsync(cancellationToken);
                break;
            }

            await MonitorSharedAnchor(cancellationToken);
        }

        private async Task RunMapRelocalizationAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Initialize relocalization (including map loading)

                TransitionToState(SharedAnchorState.Initializing);

                _syncedCameraManager.Enable();
                await _activeRelocalizer.InitializeAsync(cancellationToken);
                await WaitForAnchorManagerAsync(cancellationToken);

                // Start relocalizing

                TransitionToState(SharedAnchorState.Relocalizing);
                int consecutiveSuccessfulReloc = 0;
                int failedAttempt = 0;
                Pose cameraPose = default;
                Queue<float> scores = new Queue<float>(CONSECUTIVE_SUCCESSFUL_RELOC_TARGET);
                scores.Enqueue(0.0f);

                // Check for success criteria
                while (consecutiveSuccessfulReloc < CONSECUTIVE_SUCCESSFUL_RELOC_TARGET)
                {
                    await WaitWhilePausedAsync(cancellationToken);

                    // Check that the AR session is tracking correctly this frame
                    if (_sessionSubsystem.trackingState != TrackingState.Tracking)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Yield();
                        continue;
                    }

                    // Get the camera pose in the AR coordinate system
                    if (!_syncedCameraManager.TryGetCameraPose(out cameraPose, out var screenOrientation))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Yield();
                        continue;
                    }

                    // Save the camera position as an anchor
                    _anchorManager.CreateAnchor(ref _cameraAnchor, cameraPose);

                    // If failure criteria has been triggered
                    if (failedAttempt >= FAILED_ATTEMPT_BEFORE_COOLDOWN)
                    {
                        await WaitForCooldownAsync(cancellationToken);
                        failedAttempt = 0;
                        continue;
                    }

                    // Try to relocalize
                    var (status, result) = await _activeRelocalizer.RelocalizeAsync(cancellationToken);

                    // Compute score (moving average)
                    scores.Enqueue(result.Score);
                    if (scores.Count > CONSECUTIVE_SUCCESSFUL_RELOC_TARGET)
                    {
                        scores.Dequeue();
                    }
                    float scoreAverage = 0.0f;
                    foreach (var s in scores) { scoreAverage += s; }
                    scoreAverage /= scores.Count;
                    RelocScore = scoreAverage;

                    // Compute the shared anchor pose
                    Pose sharedAnchorPose = Utility.EstimateSharedAnchor(
                        result.Pose, InternalAnchorManager.GetAnchorPose(_cameraAnchor),
                        screenOrientation);

                    // Create an anchor at the shared anchor pose
                    _relocAnchors.Add(_anchorManager.CreateAnchor(sharedAnchorPose));

                    // Save the result
                    if (status == RelocState.Tracking)
                    {
                        consecutiveSuccessfulReloc++;
                    }
                    else
                    {
                        CleanupRelocAnchors();
                        consecutiveSuccessfulReloc = 0;
                        failedAttempt++;
                    }

                    OnSharedAnchorScoreUpdated?.Invoke(RelocScore);
                    OnSharedAnchorKeypointsUpdated?.Invoke(result.Keypoints);

                } // while not enough consecutive successful reloc

                {
                    // Compute the final shared anchor pose
                    Pose sharedAnchorPose = Utility.GetAveragePose(_relocAnchors);
                    sharedAnchorPose = Utility.SetPoseUpward(sharedAnchorPose);

                    // Create an anchor to represent the shared anchor position
                    _anchorManager.CreateAnchor(ref _sharedAnchor, sharedAnchorPose);
                }

                OnSharedAnchorPoseUpdated?.Invoke(InternalAnchorManager.GetAnchorPose(_sharedAnchor));
                TransitionToState(SharedAnchorState.Relocalized);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                MapRelocalizationCleanup();
            }
        }

        private void CleanupRelocAnchors()
        {
            foreach (var anchor in _relocAnchors)
            {
                _anchorManager.RemoveAnchor(anchor);
            }
            _relocAnchors.Clear();
        }

        private void MapRelocalizationCleanup()
        {
            _anchorManager.RemoveAnchor(ref _cameraAnchor);
            CleanupRelocAnchors();
            _activeRelocalizer.Cleanup();
            _syncedCameraManager.Disable();
            _syncedCameraManager.Clear();
        }

        private async Task WaitForAnchorManagerAsync(CancellationToken cancellationToken = default)
        {
            while (_anchorManager == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        private async Task WaitWhilePausedAsync(CancellationToken cancellationToken = default)
        {
            while (!_isRunning)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        private async Task WaitForCooldownAsync(CancellationToken cancellationToken = default)
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < COOLDOWN_TIME_IN_SECOND)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var cameraInitialPose = InternalAnchorManager.GetAnchorPose(_cameraAnchor);

                if (_syncedCameraManager.TryGetCameraPoseUnsynced(out Pose cameraCurrPose))
                {
                    float distance = Vector3.Distance(cameraInitialPose.position, cameraCurrPose.position);
                    if (distance >= MAX_DISTANCE_DELTA)
                        return;

                    float angle = Quaternion.Angle(cameraInitialPose.rotation, cameraCurrPose.rotation);
                    if (angle >= MAX_ANGLE_DELTA)
                        return;
                }

                await Task.Yield();
            }
        }

        private async Task RunImageRelocalizationAsync(CancellationToken cancellationToken = default)
        {
            // Initialize
            TransitionToState(SharedAnchorState.Initializing);

            await _activeRelocalizer.InitializeAsync(cancellationToken);

            // Run relocalization
            TransitionToState(SharedAnchorState.Relocalizing);

            RelocState status = RelocState.Lost;
            RelocResult result = default;

            while (status != RelocState.Tracking)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await WaitWhilePausedAsync(cancellationToken);

                // Check that the AR session is tracking correctly this frame
                if (_sessionSubsystem.trackingState != TrackingState.Tracking)
                {
                    await Task.Yield();
                    continue;
                }

                (status, result) = await _activeRelocalizer.RelocalizeAsync(cancellationToken);
                RelocScore = result.Score;
                
                OnSharedAnchorScoreUpdated?.Invoke(RelocScore);
                OnSharedAnchorKeypointsUpdated?.Invoke(null);

                // Create the shared anchor
                if (status == RelocState.Tracking)
                {
                    _anchorManager.CreateAnchor(ref _sharedAnchor, Utility.SetPoseUpward(result.Pose));
                }
                
                if (status != RelocState.Tracking)
                {
                    await Task.Yield();
                }
            }

            OnSharedAnchorPoseUpdated?.Invoke(InternalAnchorManager.GetAnchorPose(_sharedAnchor));
            TransitionToState(SharedAnchorState.Relocalized);
        }

        private async Task MonitorSharedAnchor(CancellationToken cancellationToken = default)
        {
            while (CurrentState == SharedAnchorState.Relocalized)
            {
                await WaitWhilePausedAsync(cancellationToken);

                // Wait for n seconds
                float startTime = Time.unscaledTime;
                while (Time.unscaledTime - startTime < 1.0f)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                if (_sessionSubsystem.trackingState != TrackingState.Tracking)
                {
                    continue;
                }

                if (_sharedAnchor.trackingState == TrackingState.Tracking)
                {
                    // Signal to update the shared anchor
                    OnSharedAnchorPoseUpdated?.Invoke(InternalAnchorManager.GetAnchorPose(_sharedAnchor));
                }
                else
                {
                    // The shared anchor is not tracking, back to relocalizing until it's tracked again
                    TransitionToState(SharedAnchorState.Relocalizing);
                    while (_sharedAnchor.trackingState != TrackingState.Tracking)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Yield();
                    }
                    OnSharedAnchorPoseUpdated?.Invoke(InternalAnchorManager.GetAnchorPose(_sharedAnchor));
                    TransitionToState(SharedAnchorState.Relocalized);
                }
            }
        }

        private void TransitionToState(SharedAnchorState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            OnSharedAnchorStateChanged?.Invoke(CurrentState);
        }

        public void Pause()
        {
            _isRunning = false;
        }

        private void CancelRelocalization()
        {
            if (_relocalizationCancelSource != null &&
                !_relocalizationCancelSource.IsCancellationRequested)
            {
                _relocalizationCancelSource.Cancel();
            }
        }

        private void ResetInternal()
        {
            if (_activeRelocalizer != null)
            {
                _activeRelocalizer.Reset();
            }

            if (_anchorManager != null)
            {
                _anchorManager.RemoveAnchor(ref _sharedAnchor);
            }

            RelocScore = 0.0f;
            TransitionToState(SharedAnchorState.Stopped);
        }

        public void Reset()
        {
            if (_relocalizationTask == null)
            {
                ResetInternal();
            }
            else
            {
                _previousRelocalizationTask = _relocalizationTask;
                CancelRelocalization();
            }
        }

        public void Destroy()
        {
            Reset();

            _localMapRelocalizer.Dispose();
            _cloudMapRelocalizer.Dispose();
            _imageRelocalizer.Dispose();
        }
    }
}