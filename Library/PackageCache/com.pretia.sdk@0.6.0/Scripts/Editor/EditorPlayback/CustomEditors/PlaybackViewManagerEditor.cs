using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Editor.CustomEditors
{
    /// <summary>
    /// Custom editor for the manager of playback views.
    /// </summary>
    [CustomEditor(typeof(PlaybackViewManager))]
    public class PlaybackViewManagerEditor : UnityEditor.Editor
    {
#region Fields
        
        /// <summary>
        /// Serialized reference to the PlaybackViews on PlaybackViewManager.
        /// </summary>
        private SerializedProperty _viewsProperty;

        /// <summary>
        /// List class to draw the PlaybackViews.
        /// </summary>
        private ReorderableList _viewList;

        /// <summary>
        /// Reference dictionary that stores the foldout status of the
        /// PlaybackViews status.
        /// </summary>
        private readonly Dictionary<PlaybackView, bool> _foldout 
            = new Dictionary<PlaybackView, bool>();

#endregion

#region Methods

        /// <summary>
        /// Event called on enable.
        /// </summary>
        public void OnEnable()
        {
            // Initializes the views property.
            _viewsProperty = serializedObject.FindProperty("views");
            
            // Initializes the list.
            _viewList = new ReorderableList(serializedObject, _viewsProperty);
            _viewList.drawElementCallback += DrawElementCallback;
            _viewList.onAddCallback += OnAddCallback;
            _viewList.onRemoveCallback += OnRemoveCallback;
            _viewList.drawHeaderCallback += OnDrawHeaderCallback;
            _viewList.elementHeightCallback += ElementHeightCallback;
            _viewList.draggable = false;

            foreach (var view in PlaybackView.Views)
            {
                if (view.Timer == null) view.Timer = new VideoTimeProvider(view.VideoPlayer);
                if (view.LoopTime)
                {
                    view.Timer.Enable(view.SetTime);
                }
            }
            
            
            EditorApplication.update += Update;
        }

        /// <summary>
        /// Called on disable.
        /// </summary>
        private void OnDisable()
        {
            
            EditorApplication.update -= Update;
            
            _viewList.drawElementCallback -= DrawElementCallback;
            _viewList.onAddCallback -= OnAddCallback;
            _viewList.onRemoveCallback -= OnRemoveCallback;
            _viewList.drawHeaderCallback -= OnDrawHeaderCallback;
            _viewList.elementHeightCallback -= ElementHeightCallback;
            
            foreach (var view in PlaybackView.Views)
            {
                view.Timer?.Disable();
            }
        }

        private void Update()
        {
            foreach (var view in PlaybackView.Views)
            {
                if (!view.LoopTime && view.VideoPlayer.isPlaying)
                {
                    view.VideoPlayer.Pause();
                }
                
            }
        }

        /// <summary>
        /// Method called to draw the header of the list.
        /// </summary>
        /// <param name="rect">Rect where to draw the header.</param>
        private static void OnDrawHeaderCallback(Rect rect)
        { 
            EditorGUI.LabelField(rect, UI.ListHeader);
        }

        /// <summary>
        /// Method called to add a new item view to the list.
        /// </summary>
        /// <param name="list">List where to add the item.</param>
        private void OnAddCallback(ReorderableList list)
        {
            var obj = new GameObject(
                string.Format(UI.ViewNameTemplate, list.count)
            );
            var view = obj.AddComponent<PlaybackView>();
            
            view.Timer = new VideoTimeProvider(view.VideoPlayer);
            
            if (view.LoopTime)
            {
                view.Timer.Enable(view.SetTime);
            }
            
            view.transform.SetParent((target as PlaybackViewManager)?.transform);
        }
        
        /// <summary>
        /// Method called to remove an item view from the list.
        /// </summary>
        /// <param name="list">List where to remove the item from.</param>
        private void OnRemoveCallback(ReorderableList list)
        {
            var index = list.index;
            if (index >= PlaybackView.Views.Count) { return; }
            var view = PlaybackView.Views[index];
            
            // Removes the item from the _foldout collection.
            if (_foldout.ContainsKey(view))
            {
                _foldout.Remove(view);
            }

            if ( view.Timer != null)
            {
                view.Timer.Disable();   
            }
            
            // Removes the item from the scene.
            DestroyImmediate(PlaybackView.Views[index].gameObject);
        }
        
        /// <summary>
        /// Returns the height of the element at the given index.
        /// </summary>
        /// <param name="index">Index of the element to get its height.</param>
        /// <returns>Int value that represents the height of the element.
        /// </returns>
        private float ElementHeightCallback(int index)
        {
            var viewProperty = _viewsProperty.GetArrayElementAtIndex(index);
            var view = viewProperty.objectReferenceValue as PlaybackView;
            if (view == null) { return UI.GetHeight(1); }
            if (view.Clip == null) { return UI.GetHeight(2); }
            return _foldout.ContainsKey(view) && _foldout[view]
                ? UI.GetHeight(4)
                : UI.GetHeight(2);
        }
        
        /// <summary>
        /// Method that draws the element at the given index.
        /// </summary>
        /// <param name="rect">Rect where to draw the element.</param>
        /// <param name="index">Index of the element to draw.</param>
        /// <param name="isActive">Whether the element is active.</param>
        /// <param name="isFocused">Whether the element id focused.</param>
        private void DrawElementCallback(
            Rect rect, 
            int index, 
            bool isActive, 
            bool isFocused)
        {
            // Gets the array reference.
            var viewProperty = _viewsProperty.GetArrayElementAtIndex(index);
            var view = viewProperty.objectReferenceValue as PlaybackView;

            if (view == null) { return; }
            

            // Adds a separation to the top of the element.
            rect.y += UI.Separation;

            // Draw the field for the clip.
            if (view.Clip == null)
            {
                // Calculate the rect.
                // ReSharper disable once UseObjectOrCollectionInitializer
                var clipFieldRect = new Rect(rect);
                clipFieldRect.height = UI.SingleLineHeight;

                view.Clip = EditorGUI.ObjectField(
                    clipFieldRect,
                    UI.ClipLabel,
                    view.Clip,
                    typeof(AnimationClip),
                    allowSceneObjects: false
                ) as AnimationClip;
            }

            // Draw the time sliders.
            else
            {
                // Calculate the rect.
                var timeSliderRect = new Rect(rect);
                timeSliderRect.width -= UI.IconButtonWidth + UI.Separation;
                timeSliderRect.height = UI.SingleLineHeight;

                var loopButtonRect = new Rect(rect);
                loopButtonRect.x += rect.width - UI.IconButtonWidth;
                loopButtonRect.height = UI.SingleLineHeight;
                loopButtonRect.width = UI.IconButtonWidth;

                // Draw Loop Slider.
                if (view.LoopTime)
                {
                    // Draws loop on button.
                    if (GUI.Button(loopButtonRect, UI.LoopOn, UI.IconStyle))
                    {
                        view.LoopTime = false;
                        if (view.Timer == null) view.Timer = new VideoTimeProvider(view.VideoPlayer);
                        view.Timer.Disable();
                    }

                    // Calculate the rect.
                    var loopSliderRect = new Rect(timeSliderRect);
                    loopSliderRect.width -= 55;

                    var timeLabelRect = new Rect(timeSliderRect);
                    timeLabelRect.x += timeSliderRect.width - 50;
                    timeLabelRect.width = 50;

                    // Draw time input field.
                    EditorGUI.DelayedFloatField(
                        timeLabelRect, 
                        (float) view.Time
                    );

                    // Calculate the rect.
                    var progress = (float)view.Time / (float)view.VideoPlayer.length;
                    var markerRect = new Rect(loopSliderRect);
                    markerRect.x += (loopSliderRect.width - 9) * progress - 4;
                    markerRect.width = UI.IconButtonWidth;
                    
                    // Draws the slider.
                    var minTime = view.MinTime;
                    var maxTime = view.MaxTime;
                    EditorGUI.MinMaxSlider(
                        loopSliderRect,
                        UI.LoopSlider,
                        ref minTime, 
                        ref maxTime,
                        minLimit: 0, 
                        maxLimit: (float) view.VideoPlayer.length
                    );
                    
                    view.MinTime = minTime;
                    view.MaxTime = maxTime;
                    
                    if (view.Timer == null) view.Timer = new VideoTimeProvider(view.VideoPlayer);
                    
                    view.Timer.MinTime = minTime;
                    view.Timer.MaxTime = maxTime;

                    // Draws the marker.
                    var didEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.LabelField(markerRect, UI.LoopSliderMarker);
                    GUI.enabled = didEnabled;
                }
                
                // Draw time slider.
                else
                {
                    // Draws loop off button.
                    if (GUI.Button(loopButtonRect, UI.LoopOff, UI.IconStyle))
                    {
                        view.LoopTime = true;
                        if (view.Timer == null) view.Timer = new VideoTimeProvider(view.VideoPlayer);
                        if (view.LoopTime)
                        {
                            view.Timer.Enable(view.SetTime);
                        }
                    } 
                    
                    // Draws time slider.
                    EditorGUI.BeginChangeCheck();
                    var time = EditorGUI.Slider(
                        timeSliderRect, 
                        UI.TimeSlider,
                        (float)view.Time, 
                        leftValue: 0,
                        rightValue: (float)view.VideoPlayer.length
                    );

                    if (view.Timer != null)
                    {
                        view.Timer.SetTime(time);
                        view.SetTime(time);
                    }
                    if (EditorGUI.EndChangeCheck()) { view.Update(); }
                }
            }
            
            // Calculates rect.
            var viewRect = new Rect(rect);
            viewRect.y += UI.SingleLineHeight + UI.Separation;
            viewRect.width = UI.IconButtonWidth;
            viewRect.height = UI.SingleLineHeight;
            
            // Draw view enable button.
            if (view.isActiveAndEnabled)
            {
                if (GUI.Button(viewRect, UI.EyeOn, UI.IconStyle))
                {
                    view.gameObject.SetActive(false);
                }
            }
            else
            {
                if (GUI.Button(viewRect, UI.EyeOff, UI.IconStyle))
                {
                    view.enabled = true;
                    view.gameObject.SetActive(true);
                }
            }

            // Draws target display.
            {
                // Calculates rect.
                var displayPopUpRect = new Rect(rect);
                displayPopUpRect.y += UI.SingleLineHeight + UI.Separation;
                displayPopUpRect.x += UI.IconButtonWidth + 2 * UI.Separation;
                displayPopUpRect.width -= UI.GetHeight(2) + UI.Separation;
                displayPopUpRect.height = UI.SingleLineHeight;
                
                // Draws target display.
                view.TargetDisplay = EditorGUI.Popup(
                    displayPopUpRect, 
                    UI.DisplayPopUp,
                    view.TargetDisplay, 
                    UI.DisplaysLabels
                );
            }

            // Do not continue to not show foldout.
            if (view.Clip == null || !_foldout.ContainsKey(view)) return;
            
            // Calculate foldout rect.
            var foldoutRect = new Rect(rect);
            foldoutRect.y += UI.SingleLineHeight + UI.Separation; 
            foldoutRect.x += rect.width - UI.IconButtonWidth; 
            foldoutRect.width = UI.IconButtonWidth;
            foldoutRect.height = UI.SingleLineHeight;
                
            // Draws foldout button.
            if (!_foldout[view])
            {
                if (GUI.Button(foldoutRect, UI.ShowFoldout, UI.IconStyle))
                {
                    _foldout[view] = true;
                }
            }
            else
            {
                if (GUI.Button(foldoutRect, UI.HideFoldout, UI.IconStyle))
                {
                    _foldout[view] = false;
                }

                // Calculate rects.
                var clipRect = new Rect(rect);
                clipRect.y += UI.GetHeight(2);
                clipRect.height = UI.SingleLineHeight;
                clipRect.x += UI.Indentation + UI.IndentedLabelWidth;
                clipRect.width -= UI.Indentation + UI.IndentedLabelWidth;

                var clipLabelRect = new Rect(clipRect);
                clipLabelRect.x -= UI.IndentedLabelWidth;
                clipLabelRect.width = UI.IndentedLabelWidth;
                    
                // Draws clip label.
                EditorGUI.LabelField(clipLabelRect, UI.ClipLabel);

                // Draws clip field.
                EditorGUI.BeginChangeCheck();
                view.Clip = EditorGUI.ObjectField(
                    clipRect, 
                    view.Clip, 
                    typeof(AnimationClip), 
                    allowSceneObjects: false
                ) as AnimationClip;
                if (EditorGUI.EndChangeCheck() && view.Clip == null)
                {
                    _foldout[view] = false;
                }

                // Calculate rects.
                var frustumLabelRect = new Rect(clipLabelRect);
                frustumLabelRect.y += UI.SingleLineHeight + UI.Separation;
                
                var frustumVisibleRect = new Rect(clipRect);
                frustumVisibleRect.y += UI.SingleLineHeight + UI.Separation;
                frustumVisibleRect.width = UI.IconButtonWidth;

                var frustumColorRect = new Rect(clipRect);
                frustumColorRect.y += UI.SingleLineHeight + UI.Separation;
                frustumColorRect.width -= UI.IconButtonWidth + UI.Separation;
                frustumColorRect.x += UI.IconButtonWidth + UI.Separation;
                
                // Draws the frustum label.
                EditorGUI.LabelField(frustumLabelRect, UI.FrustumLabel);
                
                // Draw frustum visibility.
                if (view.DrawFrustum)
                {
                    if (GUI.Button(frustumVisibleRect, UI.EyeOn, UI.IconStyle))
                    {
                        view.DrawFrustum = false;
                        SceneView.RepaintAll();
                    }
                }
                else
                {
                    if (GUI.Button(frustumVisibleRect, UI.EyeOff, UI.IconStyle))
                    {
                        view.DrawFrustum = true;
                        SceneView.RepaintAll();
                    }
                }
                
                // Draw frustum color picker.
                EditorGUI.BeginChangeCheck();
                view.GizmoColor = EditorGUI.ColorField(
                    frustumColorRect, 
                    view.GizmoColor
                );
                if (EditorGUI.EndChangeCheck()) { SceneView.RepaintAll(); }

                viewProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Called by the inspector to draw.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var views = PlaybackView.Views;

            // Clears the elements to view.
            if (_viewsProperty.arraySize != views.Count)
            {
                _viewsProperty.ClearArray();
                for (var i = 0; i < PlaybackView.Views.Count; i++)
                {
                    _viewsProperty.InsertArrayElementAtIndex(i);
                }
            }
            
            // Add elements to draw.
            for (var i = 0; i < _viewsProperty.arraySize; i++)
            {
                var viewProperty = _viewsProperty.GetArrayElementAtIndex(i);
                viewProperty.objectReferenceValue = PlaybackView.Views[i];

                if (!_foldout.ContainsKey(PlaybackView.Views[i]))
                {
                    _foldout.Add(PlaybackView.Views[i], false);
                }
            }

            // Draw the list.
            _viewList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Creates a new Game object with a Playback View Manager.
        /// </summary>
        [MenuItem("GameObject/XR/Playback View Manager")]
        public static void CreateViewManager()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new GameObject("Playback View Manager", typeof(PlaybackViewManager));
        }

#endregion

#region Nested

        /// <summary>
        /// Class that defines the UI elements.
        /// </summary>
        private static class UI
        {
            public const string ViewNameTemplate = "Playback View ({0})";
            
            public const int IconButtonWidth = 20;

            public const int Separation = 4;

            public const int Indentation = 60;

            public const int IndentedLabelWidth = 60;

            public static readonly float SingleLineHeight =
                EditorGUIUtility.singleLineHeight;

            public static readonly GUIStyle IconStyle =
                EditorStyles.centeredGreyMiniLabel;

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

            public static readonly GUIContent EyeOff =
                EditorGUIUtility.IconContent(
                    // ReSharper disable once StringLiteralTypo
                    "animationvisibilitytoggleoff",
                    "Enable visibility."
                );
            public static readonly GUIContent EyeOn =
                EditorGUIUtility.IconContent(
                    // ReSharper disable once StringLiteralTypo
                    "animationvisibilitytoggleon",
                    "Disable visibility."
                );

            public static readonly GUIContent LoopOn =
                EditorGUIUtility.IconContent(
                    "preAudioLoopOn", 
                    "The clip is currently on loop, press to stop it."
                );

            public static readonly GUIContent LoopOff =
                EditorGUIUtility.IconContent(
                    "preAudioLoopOff",
                    "Press to start a loop for this view."
                );

            public static readonly GUIContent LoopSliderMarker = 
                EditorGUIUtility.IconContent(
                    // ReSharper disable once StringLiteralTypo
                    "curvekeyframesemiselectedoverlay"
                );

            public static readonly GUIContent ShowFoldout =
                EditorGUIUtility.IconContent(
                    "_Popup",
                    "Show additional configurations."
                );

            public static readonly GUIContent HideFoldout =
                EditorGUIUtility.IconContent(
                    // ReSharper disable once StringLiteralTypo
                    "d_winbtn_win_close"
                );
            
            public static readonly GUIContent ListHeader = new GUIContent(
                "Views",
                "Available views on scene."
            );

            public static readonly GUIContent ClipLabel = new GUIContent(
                "Clip",
                "Clip to playback."
            );

            public static readonly GUIContent LoopSlider = new GUIContent(
                string.Empty,
                "Select the min time and max time for the loop."
            );

            public static readonly GUIContent TimeSlider = new GUIContent(
                string.Empty,
                "Time of the current view."
            );

            public static readonly GUIContent DisplayPopUp = new GUIContent(
                string.Empty,
                "Display where to show up the view."
            );

            public static readonly GUIContent FrustumLabel = new GUIContent(
                "Frustum",
                "Hide or show the frustum and change its color."
            );
            
            public static float GetHeight(int rowsCount) =>
                rowsCount <= 1
                    ? rowsCount * SingleLineHeight
                    : rowsCount * (SingleLineHeight + Separation);
        }

#endregion
    }
}