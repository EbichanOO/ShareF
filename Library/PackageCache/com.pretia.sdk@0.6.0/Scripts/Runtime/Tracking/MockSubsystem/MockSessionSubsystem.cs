using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    public class MockSessionSubsystem : XRSessionSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider()
        {
            return new MockProvider();
        }

        private class MockProvider : Provider { }
#endif
    }
}