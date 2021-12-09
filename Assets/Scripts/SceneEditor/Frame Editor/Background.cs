using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FrameCore;
using FrameCore.ScriptableObjects;
using FrameCore.Serialization;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора бэкграундов
    /// </summary>
    public class Background : Core {

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameBackgroundEditing() {
            Action<FrameCore.Background> backgroundEditing = BackgroundEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.BackgroundEditor] = EditorGUILayout.Foldout(foldouts[EditorType.BackgroundEditor], "Бэкграунды", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameBackground);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.BackgroundEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<BackgroundSO, FrameCore.Background>(PositioningType.Vertical, EditorType.BackgroundEditor, false, false, backgroundEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void BackgroundEditing(FrameCore.Background background) {
            var icon = UnityEditor.AssetPreview.GetAssetPreview(background.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(background);
            ElementActiveStateChange(background);
            ElementDeletion(background);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (background.activeStatus == false) {
                GUILayout.Label(background.id, EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(background.id, EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label(icon);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
#endif