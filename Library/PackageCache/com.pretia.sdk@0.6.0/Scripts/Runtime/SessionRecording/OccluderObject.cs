using System.Collections.Generic;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback
{
    /// <summary>
    /// Consider the object this is attached to as an occluder element.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class OccluderObject : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Renderer of this game object.
        /// </summary>
        [HideInInspector]
        [SerializeField] 
        private new Renderer renderer;

        /// <summary>
        /// List of occluder object to consider for the texture.
        /// </summary>
        internal static readonly List<OccluderObject> List 
            = new List<OccluderObject>();
        

        #endregion

        #region Properties
        
        /// <summary>
        /// Renderer of this game object.
        /// </summary>
        public Renderer Renderer => renderer;

        #endregion
        

        #region Event Methods

#if UNITY_EDITOR
        /// <summary>
        /// Called on reset by the editor.
        /// </summary>
        private void Reset()
        {
            renderer = GetComponent<Renderer>();
        }
#endif

        /// <summary>
        /// Called on awake.
        /// </summary>
        private void Awake()
        {
            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }
        }

        /// <summary>
        /// Called on enable.
        /// </summary>
        public void OnEnable()
        {
            if (List.Contains(this))
            {
                return;
            }

            List.Add(this);
            OccluderRenderer.RecalculateCommandBuffer();
        }

        /// <summary>
        /// Called on disable.
        /// </summary>
        public void OnDisable()
        {
            if (!List.Contains(this))
            {
                return;
            }
            
            List.Remove(this);
            OccluderRenderer.RecalculateCommandBuffer();
        }
        
        #endregion
    }
}