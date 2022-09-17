using System.Threading;
using System.Threading.Tasks;
using static PretiaArCloud.NetworkRelocalization;

namespace PretiaArCloud
{
    internal interface INetworkRelocalizationClient
    {
        Task<InitializeResponse> InitializeAsync(string mapkey, byte[] config, CancellationToken cancellationToken = default);
        Task<RelocalizeResponse> RelocalizeAsync(byte[] frameData, CancellationToken cancellationToken = default);
        Task<IntrinsicsResponse> UpdateIntrinsicsAsync(double[] intrinsics, CancellationToken cancellationToken = default);
        Task<RelocalizationDataResponse> GetRelocalizationDataAsync(CancellationToken cancellationToken = default);
        void Terminate();
    }
}
