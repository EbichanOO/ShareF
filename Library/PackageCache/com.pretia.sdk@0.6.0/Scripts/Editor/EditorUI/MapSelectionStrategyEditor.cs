using System;
using System.Collections.Generic;
using System.Linq;
using PretiaArCloud;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace PretiaArCloudEditor
{
    [CustomEditor(typeof(MapSelectionStrategy))]
    public class MapSelectionStrategyEditor : Editor
    {
        private ReorderableList _selectionList;
        private SerializedProperty _serializedSelections;

        private MapSelectionStrategy _mapSelectionStrategy => target as MapSelectionStrategy;

        private class MapSelectionDropdown : AdvancedDropdown
        {
            private MapSelectionStrategy _mapSelectionStrategy;
            private Dictionary<int, Type> _selectionDict;

            public MapSelectionDropdown(AdvancedDropdownState state, MapSelectionStrategy mapSelectionStrategy)
                : base(state)
            {
                _mapSelectionStrategy = mapSelectionStrategy;
                _selectionDict = new Dictionary<int, Type>();
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Map Selection Strategy");

                var selectionTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(AbstractMapSelection)));

                _selectionDict.Clear();
                foreach (var type in selectionTypes)
                {
                    var child = new AdvancedDropdownItem(type.Name);
                    root.AddChild(child);
                    _selectionDict.Add(child.id, type);
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                var selectionType = _selectionDict[item.id];
                var selection = ScriptableObject.CreateInstance(selectionType);
                selection.name = selectionType.Name;

                _mapSelectionStrategy.Selections.Add(selection as AbstractMapSelection);

                AssetDatabase.AddObjectToAsset(selection, _mapSelectionStrategy);
                AssetDatabase.SaveAssets();

                Selection.activeObject = selection;
            }
        }

        private void OnEnable()
        {
            _serializedSelections = serializedObject.FindProperty(nameof(MapSelectionStrategy.Selections));
            if (_selectionList == null)
            {
                _selectionList = new ReorderableList(
                    serializedObject: serializedObject,
                    elements: _serializedSelections,
                    draggable: true,
                    displayHeader: false,
                    displayAddButton: false,
                    displayRemoveButton: true);
            }

            _selectionList.drawHeaderCallback += DrawHeader;
            _selectionList.drawElementCallback += DrawSelectionStrategy;
            _selectionList.onRemoveCallback += RemoveSelectionStrategy;
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Selection Strategies");
        }

        private void DrawSelectionStrategy(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.yMin += 1;
            rect.yMax -= 1;

            var element = _serializedSelections.GetArrayElementAtIndex(index);

            EditorGUI.LabelField(rect, new GUIContent("Strategy: "));

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(
                new Rect(
                    rect.x + rect.width / 2,
                    rect.y,
                    rect.width / 2,
                    EditorGUIUtility.singleLineHeight),
                element,
                new GUIContent(""));
            EditorGUI.EndDisabledGroup();
        }

        private void RemoveSelectionStrategy(ReorderableList list)
        {
            var selection = _mapSelectionStrategy.Selections[list.index];
            _mapSelectionStrategy.Selections.RemoveAt(list.index);

            AssetDatabase.RemoveObjectFromAsset(selection);
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _selectionList.DoLayoutList();

            var dropdownRect = GUILayoutUtility.GetRect(
                new GUIContent("Add Strategy"),
                EditorStyles.toolbarButton);

            if (GUI.Button(dropdownRect, new GUIContent("Add Strategy"), EditorStyles.miniButton))
            {
                var dropdown = new MapSelectionDropdown(new AdvancedDropdownState(), _mapSelectionStrategy);
                dropdown.Show(dropdownRect);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}