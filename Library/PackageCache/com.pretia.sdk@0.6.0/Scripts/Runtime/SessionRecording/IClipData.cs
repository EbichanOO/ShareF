using UnityEngine;

namespace PretiaArCloud.RecordingPlayback
{
    public interface IClipData
    {
        AnimationClip Clip { get; }
        RenderTexture Texture { get; }
        float Duration { get; }
        bool CompareClip(Object clip);
        void SetClip(AnimationClip clip);
        Vector3 GetPosition(float time);
        Quaternion GetRotation(float time);
        float GetOrientation(float time);
        void UpdateTexture(float time);
    }
}