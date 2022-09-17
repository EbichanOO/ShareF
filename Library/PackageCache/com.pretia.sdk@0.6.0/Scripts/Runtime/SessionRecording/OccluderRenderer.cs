using UnityEngine;
using UnityEngine.Rendering;

namespace PretiaArCloud.RecordingPlayback
{
    /// <summary>
    /// Renders the occluder objects texture map.
    /// </summary>
    public static class OccluderRenderer
    {
#region Consts
        
        /// <summary>
        /// Event where the occluder objects render map should happen.
        /// </summary>
        private const CameraEvent CamEvent = CameraEvent.BeforeLighting;
        
        /// <summary>
        /// Name of the global texture where all the occluder objects should
        /// be draw.
        /// </summary>
        private const string TextureGlobalName = "_OccluderTex";
        
        /// <summary>
        /// Name of the shader used to draw occluder objects into a single
        /// texture.
        /// </summary>
        private const string ShaderName = "Hidden/PlaybackOccluder";

#endregion

#region Fields
        
        /// <summary>
        /// Command buffer that renders the occluder objects into a texture.
        /// </summary>
        private static CommandBuffer _commandBuffer;
        
        /// <summary>
        /// Material used to render the occluder objects with.
        /// </summary>
        private static Material _occluderMaterial;
        
#endregion

#region Properties
        
        /// <summary>
        /// Material used to render the occluder objects with.
        /// </summary>
        private static Material OccluderMaterial
        {
            get
            {
                if (_occluderMaterial != null) { return _occluderMaterial; }
                _occluderMaterial = new Material(Shader.Find(ShaderName));
                return _occluderMaterial;
            }
        }
        
        /// <summary>
        /// Command buffer that renders the occluder objects into a texture.
        /// </summary>
        private static CommandBuffer CommandBuffer
        {
            get
            {
                if (_commandBuffer != null) { return _commandBuffer;}
                _commandBuffer = CreateCommandBuffer();
                return _commandBuffer;
            }
        }
        
#endregion

#region Methods

        /// <summary>
        /// Adds a new command buffer to the camera.
        /// </summary>
        private static CommandBuffer CreateCommandBuffer()
        {
            // Creates a new command buffer.
            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "Occluder Texture Buffer.";

            // Creates a render texture occluder objects.
            var id = Shader.PropertyToID(TextureGlobalName);
            commandBuffer.GetTemporaryRT(id, -1, -1, 24, FilterMode.Bilinear);
            commandBuffer.SetRenderTarget(id);
            commandBuffer.ClearRenderTarget(true, true, Color.black);

            // Draws all objects to the texture.s
            for (var i = 0; i < OccluderObject.List.Count; i++)
            {
                commandBuffer.DrawRenderer(
                    OccluderObject.List[i].Renderer, 
                    OccluderMaterial
                );
            }

            // Sets the texture to be global.
            commandBuffer.SetGlobalTexture(TextureGlobalName, id);
            return commandBuffer;
        }

        /// <summary>
        /// Re calculates the command buffer.
        /// </summary>
        internal static void RecalculateCommandBuffer()
        {
            // Clears the target.
            CommandBuffer.ClearRenderTarget(true, true, Color.black);
            
            // Draws all objects to the texture.s
            for (var i = 0; i < OccluderObject.List.Count; i++)
            {
                CommandBuffer.DrawRenderer(
                    OccluderObject.List[i].Renderer, 
                    OccluderMaterial
                );
            }
        }

        /// <summary>
        /// Adds the command buffer to the given camera.
        /// </summary>
        /// <param name="camera">Camera where to add the command buffer.</param>
        internal static void AddCommandBuffer(Camera camera)
        {
            if (camera == null) return;
            camera.AddCommandBuffer(CamEvent, CommandBuffer);
        }

        /// <summary>
        /// Removes the command buffer from the camera given.
        /// </summary>
        /// <param name="camera">Camera where to remove the command buffer
        /// from.</param>
        internal static void RemoveCommandBuffer(Camera camera)
        {
            if (camera == null) return;
            camera.RemoveCommandBuffer(CamEvent, CommandBuffer);
        }

#endregion
    }
}