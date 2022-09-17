using System;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Editor.CustomEditors
{
    /// <summary>
    /// Custom editor for a PlaybackView.
    /// </summary>
    [CustomEditor(typeof(PlaybackView))]
    public class PlaybackViewEditor : UnityEditor.Editor
    {
#region Fields

        /// <summary>
        /// Serialized property reference to drawFrustum.
        /// </summary>
        private SerializedProperty _drawFrustumProperty;

        /// <summary>
        /// Serialized property reference to the projection layer.
        /// </summary>
        private SerializedProperty _projectionLayerProperty;
        
        /// <summary>
        /// Serialized property reference to minTime.
        /// </summary>
        private SerializedProperty _timeProperty;
        
        /// <summary>
        /// Serialized property reference to minTime.
        /// </summary>
        private SerializedProperty _minTimeProperty;
        
        /// <summary>
        /// Serialized property reference to maxTime.
        /// </summary>
        private SerializedProperty _maxTimeProperty;
#endregion

#region Methods

        /// <summary>
        /// Called on enable.
        /// </summary>
        private void OnEnable()
        {
            _drawFrustumProperty = serializedObject.FindProperty("drawFrustum");
            _projectionLayerProperty = serializedObject.FindProperty("projectionLayer");
            _minTimeProperty = serializedObject.FindProperty("minTime");
            _maxTimeProperty = serializedObject.FindProperty("maxTime");
            _timeProperty = serializedObject.FindProperty("time");
            
            // Emulates an update MonoBehaviour method.
            var view = target as PlaybackView;
            if (view.Timer == null) view.Timer = new VideoTimeProvider(view.VideoPlayer);
            if (view.LoopTime)
                view.Timer.Enable(view.SetTime);
        }

        /// <summary>
        /// Called on disable.
        /// </summary>
        private void OnDisable()
        {
            var view = target as PlaybackView;
            if (view != null && view.Timer != null)
            {
                view.Timer.Disable();
            }
        }

        /// <summary>
        /// Called on inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var view = target as PlaybackView;
            if ((object)view == null) { return; }

            if (!view.LoopTime && view.VideoPlayer.isPlaying)
            {
                view.VideoPlayer.Pause();
            }
            
            // Draws the clip.
            var oldClip = view.Clip;
            view.Clip = EditorGUILayout.ObjectField(
                UI.ClipLabel, 
                oldClip,
                typeof(AnimationClip), 
                allowSceneObjects: false
            ) as AnimationClip;

            if (view.Clip != oldClip)
            {
                try
                {
                    view.OnClipChanged();
                    view.UpdateData();
                }
                catch (Exception)
                {
                    view.Clip = null;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            if (view.Clip == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Draws loop an frustum options.
            EditorGUI.BeginChangeCheck();
            view.LoopTime = EditorGUILayout.Toggle(UI.LoopLabel, view.LoopTime);
            if (EditorGUI.EndChangeCheck())
            {
                if (view.Timer == null)
                {
                    view.Timer = new VideoTimeProvider(view.VideoPlayer);
                }
                
                view.Timer.Disable();
                if (view.LoopTime)
                {
                    view.Timer.Enable(view.SetTime);
                }
                else
                {
                    view.Timer.Disable();
                }
            }

            EditorGUILayout.PropertyField(_drawFrustumProperty);
            
            EditorGUI.BeginChangeCheck();

            view.TargetDisplay = EditorGUILayout.Popup(
                UI.DisplayPopUp,
                view.TargetDisplay,
                UI.DisplaysLabels
            );
            
            EditorGUILayout.PropertyField(_projectionLayerProperty);
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                view.RecalculateProjectionLayer();
            }

            if (view == null || (object) view.Clip == null) { return; }
            
            var time = System.TimeSpan.FromSeconds((float) view.VideoPlayer.length);
            
            // Draw the clip lenght.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                UI.ClipLenghtLabel, 
                new GUIContent(time.ToString("g")),
                EditorStyles.whiteLabel
            );

            // Draw the sliders.
            DrawTimeSlider(view);
            if (view.LoopTime)
            {
                DrawLoopMinMaxSlider(view);
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Draws a controllable time slider.
        /// </summary>
        /// <param name="view">Current view.</param>
        private void DrawTimeSlider(PlaybackView view)
        {
            var currentTime = _timeProperty.floatValue;
            var changedTime = EditorGUILayout.Slider(
                currentTime, 
                0, 
                (float) view.VideoPlayer.length
            );
            if (currentTime - changedTime != 0)
            {
                _timeProperty.floatValue = changedTime;
                view.Timer.SetTime(changedTime);
            }
            
            view.SetTime(currentTime);
        }

        /// <summary>
        /// Draws the slider to control the min and max time of the loop
        /// animation.
        /// </summary>
        /// <param name="view">Current view.</param>
        private void DrawLoopMinMaxSlider(PlaybackView view)
        {
            float minTime = _minTimeProperty.floatValue, 
                maxTime = _maxTimeProperty.floatValue;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider(
                ref minTime, 
                ref maxTime, 
                0, 
                (float) view.VideoPlayer.length
            );
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(52));
            EditorGUILayout.EndHorizontal();
            
            _minTimeProperty.floatValue = minTime;
            _maxTimeProperty.floatValue = maxTime;
            view.Timer.MinTime = minTime;
            view.Timer.MaxTime = maxTime;

            serializedObject.ApplyModifiedProperties();
            
        }

        /// <summary>
        /// Creates a new Game object with a Playback View.
        /// </summary>
        [MenuItem("GameObject/XR/Playback View")]
        public static void CreateView()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new GameObject("Playback View", typeof(PlaybackView));
        }
        
#endregion

#region Nested
        
        /// <summary>
        /// Class that defines the UI elements.
        /// </summary>
        private static class UI
        {
            public static readonly GUIContent ClipLabel 
                = new GUIContent("Clip");
            public static readonly GUIContent ClipLenghtLabel 
                = new GUIContent("Clip Duration:");
            public static readonly GUIContent LoopLabel
                = new GUIContent("Loop");
            public static readonly GUIContent[] DisplaysLabels = {
                new GUIContent("Display 1"),
                new GUIContent("Display 2"),
                new GUIContent("Display 3"),
                new GUIContent("Display 4"),
                new GUIContent("Display 5"),
                new GUIContent("Display 6"),
                new GUIContent("Display 7"),
                new GUIContent("Display 8")
            };
            public static readonly GUIContent DisplayPopUp = new GUIContent(
                "Display",
                "Display where to show up the view."
            );
        }

#endregion
    }
}