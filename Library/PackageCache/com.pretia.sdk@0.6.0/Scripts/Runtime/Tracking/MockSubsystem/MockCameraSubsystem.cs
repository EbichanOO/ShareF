using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    internal class MockCameraSubsystem : XRCameraSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MockProvider();

        private class MockProvider : Provider { }
#endif
    }
}