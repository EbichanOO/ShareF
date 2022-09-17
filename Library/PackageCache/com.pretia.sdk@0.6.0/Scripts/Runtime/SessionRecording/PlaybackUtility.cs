using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Video;
#endif

namespace PretiaArCloud.RecordingPlayback
{
    public static class PlaybackUtility
    {
#region Definitions

        /// <summary>
        /// Path from the root of the game object to apply the animation.
        /// </summary>
        public const string RelativePath = "";
        
        /// <summary>
        /// Type of the component this importer is for.
        /// </summary>
        public static readonly System.Type Type = typeof(PlaybackBehaviour);

        /// <summary>
        /// Name of the property the texture value is for.
        /// </summary>
        public const string TexturePropertyName = "texture2D";
        
        /// <summary>
        /// Name of the property the texture value is for.
        /// </summary>
        public const string VideoPropertyName = "videoClip";

        /// <summary>
        /// Name of the property the orientation value is for.
        /// </summary>
        private const string OrientationPropertyName = "orientation";
        
        /// <summary>
        /// Name of the property the position values are for.
        /// </summary>
        private const string PositionPropertyNameFormat = "position.{0}";
        
        /// <summary>
        /// Name of the property the rotation values are for.
        /// </summary>
        private const string RotationPropertyNameFormat = "rotation.{0}";

#if UNITY_EDITOR
        
        /// <summary>
        /// Texture binding.
        /// </summary>
        private static readonly EditorCurveBinding TextureBinding =
            new EditorCurveBinding()
            {
                path = RelativePath,
                type = Type,
                propertyName = TexturePropertyName
            };

        /// <summary>
        /// Video clip binding.
        /// </summary>
        public static readonly EditorCurveBinding VideoClipBinding =
            new EditorCurveBinding()
            {
                path = RelativePath,
                type = Type,
                propertyName = VideoPropertyName
            };

        /// <summary>
        /// Orientation of the rotation of the screen binding.
        /// </summary>
        public static readonly EditorCurveBinding OrientationRotationBinding =
            new EditorCurveBinding()
            {
                path = RelativePath,
                type = Type,
                propertyName = OrientationPropertyName
            };

        /// <summary>
        /// Position bindings 
        /// </summary>
        private static readonly Dictionary<char, EditorCurveBinding>
            PositionBindings = new Dictionary<char, EditorCurveBinding>
            {
                {'x',GetPositionBinding('x')},
                {'y',GetPositionBinding('y')},
                {'z',GetPositionBinding('z')}
            };
        
        /// <summary>
        /// Rotation bindings.
        /// </summary>
        private static readonly Dictionary<char, EditorCurveBinding>
            RotationBindings = new Dictionary<char, EditorCurveBinding>
            {
                {'x',GetRotationBinding('x')},
                {'y',GetRotationBinding('y')},
                {'z',GetRotationBinding('z')},
                {'w',GetRotationBinding('w')}
            };
#endif
#endregion
        
#region Methods
#if UNITY_EDITOR
        
        /// <summary>
        /// Gets the Editor Curve Binding for position of the given axis.
        /// </summary>
        /// <param name="axis">Axis to return its binding.</param>
        /// <returns>Editor Curve Binding for the given axis.</returns>
        public static EditorCurveBinding GetPositionBinding(char axis)
        {
            var propertyName = string.Format(PositionPropertyNameFormat, axis); 
            return new EditorCurveBinding()
            {
                path = RelativePath,
                type = Type,
                propertyName = propertyName
            };
        }

        /// <summary>
        /// Gets the Editor Curve Binding for the given axis.
        /// </summary>
        /// <param name="axis">Axis to return its binding.</param>
        /// <returns>Editor Curve Binding for the given axis.</returns>
        public static EditorCurveBinding GetRotationBinding(char axis)
        {
            var propertyName = string.Format(RotationPropertyNameFormat, axis); 
            return new EditorCurveBinding()
            {
                path = RelativePath,
                type = Type,
                propertyName = propertyName
            };
        }
        
        /// <summary>
        /// Gets the playback texture curve for the given clip.
        /// </summary>
        /// <param name="clip">Clip that has Playback animation.</param>
        /// <returns>Array with the object reference.</returns>
        internal static ObjectReferenceKeyframe[] GetTextureCurve(
            AnimationClip clip
        ) => AnimationUtility.GetObjectReferenceCurve(clip, TextureBinding);
        
        /// <summary>
        /// Gets the playback texture curve for the given clip.
        /// </summary>
        /// <param name="clip">Clip that has Playback animation.</param>
        /// <returns>Array with the object reference.</returns>
        internal static VideoClip GetVideoCip(AnimationClip clip)
        {
            var curve = AnimationUtility.GetObjectReferenceCurve(
                clip, 
                VideoClipBinding
            );

            if (curve == null || curve.Length == 0) { return null; }
            return curve[0].value as VideoClip;
        }
        
        /// <summary>
        /// Gets the orientation curve for the given clip.
        /// </summary>
        /// <param name="clip">Clip that has Playback animation.</param>
        /// <returns>Animation curve.</returns>
        internal static AnimationCurve GetOrientationCurve(AnimationClip clip)
        {
            return AnimationUtility.GetEditorCurve(
                clip, 
                OrientationRotationBinding
            );
        }

        /// <summary>
        /// Gets the playback position curve for the given clip
        /// </summary>
        /// <param name="clip">Clip that has Playback animation.</param>
        /// <param name="axis">Axis of the animation</param>
        /// <returns>Animation curve.</returns>
        internal static AnimationCurve GetPositionCurve(
            AnimationClip clip, 
            char axis
        ) => !PositionBindings.ContainsKey(axis) 
            ? null
            : AnimationUtility.GetEditorCurve(clip, PositionBindings[axis]);

        /// <summary>
        /// Gets the playback rotation curve for the given clip
        /// </summary>
        /// <param name="clip">Clip that has Playback animation.</param>
        /// <param name="axis">Axis of the animation</param>
        /// <returns>Animation Curve.</returns>
        internal static AnimationCurve GetRotationCurve(
            AnimationClip clip,
            char axis
        ) => !RotationBindings.ContainsKey(axis) 
            ? null 
            : AnimationUtility.GetEditorCurve(clip, RotationBindings[axis]);

#endif
#endregion
    }
}