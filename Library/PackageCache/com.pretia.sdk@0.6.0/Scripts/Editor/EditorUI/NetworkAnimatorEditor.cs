using System.Collections.Generic;
using PretiaArCloud.Networking;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace PretiaArCloud.NetworkingEditor
{
    [CustomEditor(typeof(NetworkAnimator))]
    public class NetworkAnimatorEditor : Editor
    {
        private SerializedProperty _serializedSynchronizedParameters;
        private AnimatorController _animatorController;

        private bool _foldout;

        private void OnEnable()
        {
            _serializedSynchronizedParameters = serializedObject.FindProperty(nameof(NetworkAnimator.SynchronizedParameters));

            var serializedAnimator = serializedObject.FindProperty(nameof(NetworkAnimator.Animator));
            if (serializedAnimator.objectReferenceValue == null) return;

            _animatorController = (serializedAnimator.objectReferenceValue as Animator).runtimeAnimatorController as AnimatorController;

            if (_animatorController.parameters.Length != _serializedSynchronizedParameters.arraySize)
            {
                _serializedSynchronizedParameters.ClearArray();

                for (int i = 0; i < _animatorController.parameters.Length; i++)
                {
                    _serializedSynchronizedParameters.InsertArrayElementAtIndex(i);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.DrawDefaultInspector();

            if (_animatorController == null) return;

            _foldout = EditorGUILayout.Foldout(_foldout, "Synchronized Parameters", toggleOnLabelClick: true);

            if (_foldout)
            {
                for (int i = 0; i < _animatorController.parameters.Length; i++)
                {
                    var parameter = _animatorController.parameters[i];
                    var serializedParameter = _serializedSynchronizedParameters.GetArrayElementAtIndex(i);
                    serializedParameter.boolValue = EditorGUILayout.ToggleLeft($"{parameter.type} {parameter.name}", serializedParameter.boolValue);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}