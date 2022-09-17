using UnityEngine;

namespace PretiaArCloud
{
    public interface IScaledCameraManager
    {
        bool TryGetIntrinsics(out Vector2 focalLength, out Vector2 principalPoint, out Vector2Int resolution);
        bool TryGetLatestCameraTexture(out Texture2D cameraTexture, out double timestamp);
        System.IntPtr GetTextureUnsafePtr(Texture2D tex);
    }
}