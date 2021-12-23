using FrameCore.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора эффектов
    /// </summary>
    public class FrameLight : Core {

        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameLightEditing() {
            Action<FrameCore.FrameLight> effectEditing = LightEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.FrameLightEditor] = EditorGUILayout.Foldout(foldouts[EditorType.FrameLightEditor], "Свет", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameLight);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.FrameLightEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<FrameLightSO, FrameCore.FrameLight>(PositioningType.Vertical, EditorType.FrameLightEditor, false, false, effectEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void LightEditing(FrameCore.FrameLight frameLight) {
            var keyValues = frameLight.GetFrameKeyValues<FrameCore.Serialization.FrameLightValues>();
            string id = frameLight.id;

            var icon = UnityEditor.AssetPreview.GetAssetPreview(frameLight.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(frameLight);
            ElementActiveStateChange(frameLight);
            ElementDeletion(frameLight);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (frameLight.activeStatus == false) {
                GUILayout.Label(frameLight.GetName(), EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(frameLight.GetName(), FrameGUIUtility.GetLabelStyle(Color.cyan, 15));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

           /** GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Параметры:", EditorStyles.boldLabel, GUILayout.MaxWidth(80));
            GUILayout.BeginVertical();
            GUILayout.Space(9);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();**/

            /**if (FrameCore.FrameManager.GetFrameElementOnSceneByID<FrameCore.FrameLight>(id) != null) {
                if (!CoreWindow.core.lightEditor.ContainsKey(frameLight.id)) {
                    CoreWindow.core.lightEditor.Add(frameLight.id, CreateEditor(frameLight));
                }
                else {
                    if(CoreWindow.core.lightEditor[frameLight.id] != null) {
                        CoreWindow.core.lightEditor[frameLight.id].OnInspectorGUI();
                    }
                    else {
                        CoreWindow.core.lightEditor[frameLight.id] = CreateEditor(frameLight);
                    }
                }
            }
            else {
                foreach (var editor in CoreWindow.core.lightEditor) {
                    if (editor.Key == id) {
                        if(editor.Value != null)
                            DestroyImmediate(editor.Value);
                    }
                }
            }**/

            //GUILayout.EndVertical();
        }
        /**public static Editor ResycleInspectior(UnityEngine.Object target, string id) {
            Editor editor = null; 
            if (CoreWindow.core.lightEditor[id] != null) DestroyImmediate(CoreWindow.core.lightEditor[id]);
            else {editor = Editor.CreateEditor(target);
                CoreWindow.core.lightEditor[id] = editor; }
            return editor;
        }**/
    }
}
#endif
