using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;
using FrameCore.ScriptableObjects;
using System;
using UnityEditor;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора эффектов
    /// </summary>
    public class FrameEffect : Core {

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameEffectEditing() {
            Action<FrameCore.FrameEffect> effectEditing = EffectEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.FrameEffectEditor] = EditorGUILayout.Foldout(foldouts[EditorType.FrameEffectEditor], "Эффекты", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameEffect);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.FrameEffectEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<FrameEffectSO, FrameCore.FrameEffect>(PositioningType.Vertical, EditorType.FrameEffectEditor, false, false, effectEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void EffectEditing(FrameCore.FrameEffect frameEffect) {
            var icon = UnityEditor.AssetPreview.GetAssetPreview(frameEffect.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(frameEffect);
            ElementActiveStateChange(frameEffect);
            ElementDeletion(frameEffect);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (frameEffect.activeStatus == false) {
                GUILayout.Label(frameEffect.GetName(), EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(frameEffect.GetName(), FrameGUIUtility.GetLabelStyle(Color.cyan, 15));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
#endif
