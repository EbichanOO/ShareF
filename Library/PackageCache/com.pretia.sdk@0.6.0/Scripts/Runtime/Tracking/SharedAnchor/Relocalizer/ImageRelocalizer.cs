using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    internal class ImageRelocalizer : IRelocalizer
    {
        private ARTrackedImageManager _trackedImageManager;

        private XRReferenceImage _referenceImage;

        public TrackableId ImageTrackableId { get; private set; } = TrackableId.invalidId;

        public ImageRelocalizer()
        {
        }

        public void SetTrackedImageManager(ARTrackedImageManager trackedImageManager)
        {
            _trackedImageManager = trackedImageManager;
        }

        public void SetReferenceImage(XRReferenceImage referenceImage)
        {
            _referenceImage = referenceImage;
        }

        public XRReferenceImage GetReferenceImage() => _referenceImage;

        public void Cleanup()
        {
        }

        public void Dispose()
        {
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_trackedImageManager == null)
            {
                throw new Exception("AR Tracked Image Manager not set");
            }

            if (_referenceImage == default)
            {
                throw new Exception("Reference image to use as shared anchor not set");
            }

            if (_trackedImageManager.referenceLibrary == null)
            {
                throw new Exception("Reference image library not set");
            }

            return Task.CompletedTask;
        }

        public Task<(RelocState, RelocResult)> RelocalizeAsync(CancellationToken cancellationToken = default)
        {
            RelocState trackingState = RelocState.Lost;
            RelocResult result;
            result.Pose = default;
            result.Keypoints = default;
            result.Score = 0.0f;

            foreach (var image in _trackedImageManager.trackables)
            {
                if (image.referenceImage.guid != _referenceImage.guid)
                {
                    continue;
                }

                if (image.trackingState == TrackingState.Limited)
                {
                    result.Score = 0.5f;
                    break;
                }
                else if (image.trackingState == TrackingState.Tracking)
                {
                    ImageTrackableId = image.trackableId;
                    result.Score = 1.0f;
                    // Get the image pose in the AR coordinate system (localPosition / localRotation)
                    result.Pose.position = image.transform.localPosition;
                    result.Pose.rotation = image.transform.localRotation;
                    trackingState = RelocState.Tracking;
                    break;
                }
            }

            return Task.FromResult((trackingState, result));
        }

        public void Reset()
        {
            ImageTrackableId = TrackableId.invalidId;
        }
    }
}