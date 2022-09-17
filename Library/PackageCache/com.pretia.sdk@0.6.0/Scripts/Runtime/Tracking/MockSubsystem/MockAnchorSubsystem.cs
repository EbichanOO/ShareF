using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    internal class MockAnchorSubsystem : XRAnchorSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MockProvider();

        private class MockProvider : Provider
        {
            public override TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
            {
                return default;
            }
        }
#endif
    }
}