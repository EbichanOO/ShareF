using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PretiaArCloud
{
    internal class InternalAnchorManager
    {
        private ARSessionOrigin _arSessionOrigin;
        private ARAnchorManager _arAnchorManager;

        public InternalAnchorManager(ARSessionOrigin sessionOrigin, ARAnchorManager anchorManager)
        {
            _arSessionOrigin = sessionOrigin;
            _arAnchorManager = anchorManager;
        }

        public ARAnchor CreateAnchor(Pose pose, GameObject customPrefab = null)
        {
            _arAnchorManager.enabled = true;

            GameObject obj;
            if (customPrefab != null)
            {
                obj = UnityEngine.Object.Instantiate(customPrefab);
            }
            else if (_arAnchorManager.anchorPrefab != null)
            {
                obj = UnityEngine.Object.Instantiate(_arAnchorManager.anchorPrefab);
            }
            else
            {
                obj = new GameObject();
            }

            // Setting the parent of the anchor as ARSessionOrigin.trackablesParent
            // allows us to use transform.localPosition and transform.localRotation
            // to get the anchor pose in AR session coordinate system.
            obj.transform.SetParent(_arSessionOrigin.trackablesParent);
            obj.transform.localPosition = pose.position;
            obj.transform.localRotation = pose.rotation;
            return obj.AddComponent<ARAnchor>();
        }

        public void CreateAnchor(ref ARAnchor anchor, Pose pose)
        {
            RemoveAnchor(ref anchor);
            anchor = CreateAnchor(pose);
        }

        public void RemoveAnchor(ref ARAnchor anchor)
        {
            RemoveAnchor(anchor);
            anchor = null;
        }

        public void RemoveAnchor(ARAnchor anchor)
        {
            if (anchor != null)
            {
                UnityEngine.Object.Destroy(anchor.gameObject);
            }
        }

        public static Vector3 GetAnchorPosition(ARAnchor anchor)
        {
            return anchor.transform.localPosition;
        }

        public static Quaternion GetAnchorRotation(ARAnchor anchor)
        {
            return anchor.transform.localRotation;
        }

        public static Pose GetAnchorPose(ARAnchor anchor)
        {
            return new Pose(GetAnchorPosition(anchor), GetAnchorRotation(anchor));
        }
    }
}
