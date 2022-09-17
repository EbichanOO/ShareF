using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud
{
    [CreateAssetMenu(fileName = "DirectMapSelection", menuName = "Pretia ArCloud/Map Selection/Direct Map Selection")]
    public class DirectMapSelection : AbstractMapSelection
    {
        [SerializeField]
        private string _mapKey;

        public override Task<string> GetMapSelectionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_mapKey);
        }
    }
}