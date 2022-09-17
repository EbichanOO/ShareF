using System.Collections.Generic;
using UnityEngine;

namespace PretiaArCloud
{
    public class MapSelectionStrategy : ScriptableObject
    {
        public List<AbstractMapSelection> Selections = new List<AbstractMapSelection>();
    }
}