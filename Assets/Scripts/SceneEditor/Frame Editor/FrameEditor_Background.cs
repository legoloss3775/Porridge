using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class FrameEditor_Background : FrameEditor
{
    public static void FrameBackgroundEditing() {
        Action<FrameBackground> backgroundEditing = BackgroundEditing;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        foldouts[EditorType.BackgroundEditor] = EditorGUILayout.Foldout(foldouts[EditorType.BackgroundEditor], "Бэкграунды", EditorStyles.foldoutHeader);
        GUILayout.FlexibleSpace();
        ElementCreation(FrameEditor_CreationWindow.CreationType.FrameBackground);
        GUILayout.EndHorizontal();

        if (foldouts[EditorType.BackgroundEditor]) {
            GUILayout.BeginVertical("HelpBox");
            ElementEditing<FrameBackgroundSO, FrameBackground>(PositioningType.Vertical, EditorType.BackgroundEditor, false, false, backgroundEditing);
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }
    public static void BackgroundEditing(FrameBackground background) {
        var icon = UnityEditor.AssetPreview.GetAssetPreview(background.frameElementObject.prefab);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ElementSelection(background);
        ElementDeletion(background);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label(background.id, EditorStyles.largeLabel);
        GUILayout.Label(icon);
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
#endif