using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud
{
    [System.Serializable]
    public abstract class AbstractMapSelection : ScriptableObject
    {
        public abstract Task<string> GetMapSelectionAsync(CancellationToken cancellationToken = default);
    }
}