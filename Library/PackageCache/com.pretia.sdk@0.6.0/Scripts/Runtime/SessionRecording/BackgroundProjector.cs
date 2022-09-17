using UnityEngine;

namespace PretiaArCloud.RecordingPlayback
{
    public class BackgroundProjector
    {
        
        /// <summary>
        /// Name of the shader used to project the video over objects.
        /// </summary>
        private const string ShaderName = "Hidden/PlaybackProjector";

        /// <summary>
        /// Name of the property on the shader used to project the video over
        /// objects.
        /// </summary>
        private const string TexturePropertyName = "_ShadowTex";
        
        /// <summary>
        /// ID of the property for the projection texture in the shader.
        /// </summary>
        private static readonly int ProjectionTex 
            = Shader.PropertyToID(TexturePropertyName);
        
        private Projector Projector { get; }
        
        /// <summary>
        /// Material for the projection.
        /// </summary>
        private Material ProjectionMaterial { get; set; }

        public BackgroundProjector(Projector projector, Camera camera)
        {
            if ((object)projector == null || (object) camera == null)
            {
                throw new System.NullReferenceException();
            }
            
            Projector = projector;
            Projector.nearClipPlane = camera.nearClipPlane;
            Projector.farClipPlane = camera.farClipPlane;
            Projector.fieldOfView = camera.fieldOfView;
            Projector.orthographic = camera.orthographic;
            Projector.orthographicSize = camera.orthographicSize;
            Projector.hideFlags =  HideFlags.HideInInspector;
        }

        public void AutoEnableByLayerMask(LayerMask layer)
        {
            if (layer.value == decimal.Zero && Projector.enabled)
            {
                Projector.enabled = false;
            }
            else if (layer.value != decimal.Zero)
            {
                Projector.enabled = true;
            }
        }

        public void SetBackgroundTexture(RenderTexture texture)
        {
            
            if ((object)ProjectionMaterial == null)
            {
                ProjectionMaterial = new Material(Shader.Find(ShaderName))
                {
                    hideFlags = HideFlags.HideInInspector
                };

                Projector.material = ProjectionMaterial;
            }
            
            ProjectionMaterial.SetTexture(ProjectionTex, texture);
        }
        
        public void SetProjectionLayer(LayerMask layer)
        {
            switch (layer.value)
            {
                case 0:
                    Projector.ignoreLayers = -1;
                    break;
                case -1:
                    Projector.ignoreLayers = 0;
                    break;
                default:
                {
                    var mask = -1;
                    for (var i = 0; i <= 31; i++)
                    {
                        if ((layer.value & (1 << i)) > 0)
                        {
                            mask &= ~(1 << i);
                        }
                    }
                    
                    Projector.ignoreLayers = mask;
                    break;
                }
            }
        }

        public void SetAspectRatio(float aspect)
        {
            Projector.aspectRatio = aspect;
        }
    }
}