using System.IO;
using UnityEngine;

namespace PretiaArCloud
{
    public class MapContentsController : MonoBehaviour
    {
        public TextAsset MapContentsAsset;
        public string MapKey => Path.GetFileNameWithoutExtension(MapContentsAsset.name);
    }
}