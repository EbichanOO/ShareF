using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace PretiaArCloud
{
    [CreateAssetMenu(fileName = "CriteriaBasedMapSelection", menuName = "Pretia ArCloud/Map Selection/Criteria Based Map Selection")]
    public class CriteriaBasedMapSelection : AbstractMapSelection
    {
        [SerializeField]
        private MapSelectionCriteria _criteria;

        public override async Task<string> GetMapSelectionAsync(CancellationToken cancellationToken = default)
        {
            await LocationProvider.StartLocationServiceAsync(cancellationToken);
            var selections = await MapSelection.SelectMapsAsync(_criteria, cancellationToken);
            return selections.SelectedMaps.FirstOrDefault().MapKey;
        }
    }
}