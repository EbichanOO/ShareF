using System;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud
{
    internal interface IRelocalizer : IDisposable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task<(RelocState, RelocResult)> RelocalizeAsync(CancellationToken cancellationToken = default);
        void Cleanup();
        void Reset();
    }
}