using UnityEngine;

namespace PretiaArCloud.RecordingPlayback
{
    internal class BackgroundDisplay
    {
        /// <summary>
        /// Name of the shader.
        /// </summary>
        private const string BackgroundShaderName = "Hidden/PlaybackBackground";

        /// <summary>
        /// Name of the property on the shader used as input for the background
        /// texture.
        /// </summary>
        private const string BackgroundPropertyName = "_BackgroundTex";

        /// <summary>
        /// Name of the property on the shader used as input for the angle of
        /// the background.
        /// </summary>
        private const string BackgroundRotationAngleName = "_Angle";
        
        /// <summary>
        /// ID of the property for the background in the shader.
        /// </summary>
        private static readonly int BackgroundPropertyId 
            = Shader.PropertyToID(BackgroundPropertyName);

        /// <summary>
        /// ID of the property for the angle of the background in the shader.
        /// </summary>
        private static readonly int AnglePropertyId
            = Shader.PropertyToID(BackgroundRotationAngleName);

        /// <summary>
        /// Material to show off the background of the view.
        /// </summary>
        private Material BackgroundMaterial { get; set; }
        
        public Camera Camera { get; }
        
        public BackgroundDisplay(Camera camera)
        {
            if (camera == null)
            {
                throw new System.NullReferenceException();
            }
            
            Camera = camera;
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = Color.clear;
            Camera.depthTextureMode = DepthTextureMode.Depth;
            Camera.renderingPath = RenderingPath.DeferredShading;
            Camera.nearClipPlane = 0.001f;
            Camera.hideFlags = HideFlags.HideInInspector;
        }

        public void AddBuffer()
        {
            OccluderRenderer.AddCommandBuffer(Camera);
        }

        public void RemoveBuffer()
        {
            OccluderRenderer.RemoveCommandBuffer(Camera);
        }

        public void Blit(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, BackgroundMaterial);
        }

        public void SetRotationAngle(float degrees)
        {
            if ((object)BackgroundMaterial == null)
            {
                BackgroundMaterial = new Material(
                    Shader.Find(BackgroundShaderName)
                ); 
            }
            
            BackgroundMaterial.SetFloat(
                AnglePropertyId,
                degrees
            );
        }

        public void SetBackgroundTexture(RenderTexture texture)
        {
            if ((object)BackgroundMaterial == null)
            {
                BackgroundMaterial = new Material(
                    Shader.Find(BackgroundShaderName)
                ); 
            }
            
            BackgroundMaterial.SetTexture(
                BackgroundPropertyId, 
                texture
            );
        }

        public void SetAspectRatio(Rect pixelRect, float aspect)
        {
            Camera.pixelRect = pixelRect;
            Camera.aspect = aspect;
        }

    }
}