using PretiaArCloud;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PretiaArCloudEditor
{
    [CustomEditor(typeof(ARSharedAnchorManager))]
    public class ARSharedAnchorManagerEditor : Editor
    {
        private bool _isAutomaticRelocalizationExpanded;
        private SerializedProperty _runRelocalizationOnStartProp;
        private SerializedProperty _relocalizationTypeProp;
        private SerializedProperty _selectedReferenceImageIndexProp;
        private SerializedProperty _mapSelectionProp;
        private SerializedProperty _trackedImageManagerProp;

        private void OnEnable()
        {
            _runRelocalizationOnStartProp = serializedObject.FindProperty("_runRelocalizationOnStart");
            _relocalizationTypeProp = serializedObject.FindProperty("RelocalizationType");
            _selectedReferenceImageIndexProp = serializedObject.FindProperty("_selectedReferenceImageIndex");
            _mapSelectionProp = serializedObject.FindProperty("_mapSelection");
            _trackedImageManagerProp = serializedObject.FindProperty("_trackedImageManager");
        }

        private void DrawAutomaticRelocalization()
        {
            EditorGUILayout.Separator();

            DrawSplitter();
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);
            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var toggleRect = backgroundRect;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, headerBackground);

            // Title
            using (new EditorGUI.DisabledScope(!_runRelocalizationOnStartProp.boolValue))
                EditorGUI.LabelField(labelRect, "Relocalize On Start", EditorStyles.boldLabel);

            // Checkbox
            _runRelocalizationOnStartProp.boolValue = GUI.Toggle(toggleRect, _runRelocalizationOnStartProp.boolValue, GUIContent.none, new GUIStyle("ShurikenToggle"));

            if (_runRelocalizationOnStartProp.boolValue)
            {
                EditorGUI.indentLevel++;
                // using (new EditorGUI.DisabledScope(!_runRelocalizationOnStartProp.boolValue))
                {
                    DrawPropertiesExcluding(
                        serializedObject,
                        "m_Script",
                        "_runRelocalizationOnStart",
                        "_mapSelection",
                        "_trackedImageManager",
                        "_selectedReferenceImageIndex");
                }
                EditorGUI.indentLevel--;
            }

        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ARSharedAnchorManager)target), typeof(ARSharedAnchorManager), false);

            DrawAutomaticRelocalization();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Map-based Relocalization Default Parameter", EditorStyles.boldLabel);
            _mapSelectionProp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Map Selection", "Map Selection is required when running map-based relocalization. This decides the default map selection implementation to be used when running map-based relocalization. You can create your own implementation by inheriting the AbstractMapSelection class"), _mapSelectionProp.objectReferenceValue, typeof(AbstractMapSelection), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Image-based Relocalization Default Parameter", EditorStyles.boldLabel);
            _trackedImageManagerProp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("AR Tracked Image Manger", "TrackedImageManager is required when running image-based relocalization."), _trackedImageManagerProp.objectReferenceValue, typeof(ARTrackedImageManager), true);

            var mapSelection = _mapSelectionProp.objectReferenceValue as AbstractMapSelection;
            var trackedImageManager = _trackedImageManagerProp.objectReferenceValue as ARTrackedImageManager;

            bool nullReferenceLibrary = trackedImageManager == null || trackedImageManager.referenceLibrary == null;
            bool emptyReferenceLibrary = nullReferenceLibrary || trackedImageManager.referenceLibrary.count == 0;

            using (new EditorGUI.DisabledScope(nullReferenceLibrary || emptyReferenceLibrary)) 
            {
                int imagesCount = emptyReferenceLibrary ? 0 : trackedImageManager.referenceLibrary.count;
                string[] imagesName = new string[imagesCount];
                for (int i = 0; i < imagesCount; i++)
                {
                    imagesName[i] = trackedImageManager.referenceLibrary[i].name;
                }

                _selectedReferenceImageIndexProp.intValue = EditorGUILayout.Popup(new GUIContent("Reference image", "The reference image to use when running image-based relocalization automatically with RunRelocalizationOnStart. You can select from one of the images in the reference image library set in the ARTrackedImageManager"), _selectedReferenceImageIndexProp.intValue, imagesName);
            }

            // Image-based relocalization
            if (_runRelocalizationOnStartProp.boolValue && _relocalizationTypeProp.enumValueIndex == 0 && nullReferenceLibrary)
            {
                EditorGUILayout.HelpBox("The Tracked Image Manager is not set. Please set the ARTrackedImageManager component to run image-based relocalization automatically on start.", MessageType.Error);
            }
            // Cloud map-based relocalization
            else if (_runRelocalizationOnStartProp.boolValue && _relocalizationTypeProp.enumValueIndex == 1 && mapSelection == null)
            {
                EditorGUILayout.HelpBox("The map selection is not set. Please assign a map selection asset in order to run cloud map-based relocalization automatically on start", MessageType.Error);
            }

            if (!nullReferenceLibrary && emptyReferenceLibrary)
            {
                EditorGUILayout.HelpBox("The reference image library does not have any images set. Please add at least one image to the reference image library to run image-based relocalization automatically on start", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws a horizontal split line.
        /// </summary>
        private void DrawSplitter()
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, splitter);
        }

        private Color headerBackground { get { return EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 0.2f) : new Color(1f, 1f, 1f, 0.2f); } }
        private Color splitter { get { return EditorGUIUtility.isProSkin ? new Color(0.12f, 0.12f, 0.12f, 1.333f) : new Color(0.6f, 0.6f, 0.6f, 1.333f); } }
    }
}
