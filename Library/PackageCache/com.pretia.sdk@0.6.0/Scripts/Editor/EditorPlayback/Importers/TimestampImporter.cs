using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PretiaArCloud.RecordingPlayback.Serialization;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Editor.Importers
{
    /// <summary>
    /// Imports timestamps.
    /// </summary>
    internal class TimestampImporter : IImporter
    {
#region Properties

        /// <summary>
        /// Current accumulated time the importer is on.
        /// </summary>
        public float Time { get; private set; }
        
        /// <summary>
        /// Current last read.
        /// </summary>
        private DateTime CurrentRead { get; set; }

#endregion

#region Methods

        /// <inheritdoc cref="IImporter.CollectKeyframes"/>
        public Task CollectKeyframes(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            var date = DateSerialization.Read(
                stream, 
                options, 
                cancelToken
            );

            if (CurrentRead != default)
            {
                Time += (float) (date - CurrentRead).TotalSeconds;
            }

            CurrentRead = date;

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IImporter.AddKeyFrames"/> 
        public void AddKeyFrames(AnimationClip _) { }

#endregion
    }
}