using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Editor.Importers
{
    /// <summary>
    /// Interface for the importers used to read the data on an AR Record file
    /// and convert it into an animation.
    /// </summary>
    internal interface IImporter
    {
        /// <summary>
        /// Reads the data from the given stream and put them into a key frames.
        /// </summary>
        /// <param name="stream">Stream where to read the data from.</param>
        /// <param name="options">Options for serialization.</param>
        /// <param name="cancelToken">Token to cancel the task.</param>
        /// <returns>Task of reading.</returns>
        Task CollectKeyframes(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default
        );

        /// <summary>
        /// Adds frames to the given clip.
        /// </summary>
        /// <param name="clip">Clip where to add the collected frames.</param>
        void AddKeyFrames(AnimationClip clip);
    }
}