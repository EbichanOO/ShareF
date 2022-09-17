using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NetworkRelocalization
    {
        public enum Protocol : System.Int32
        {
            Initialize,
            Relocalize,
            Terminate,
            InitializeNetOnly,
            RelocalizeNetOnly,
            RelocalizationData,
            UpdateIntrinsics,
            End,
        }
    }
}