using FrameCore.ScriptableObjects;
using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FrameEditor {
    /// <summary>
    /// Класс редактора камеры
    /// </summary>
    public class FrameCamera : Core {
        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameCameraEditing() {
            Action<FrameCore.FrameCamera> frameCameraEditing = CameraEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.FrameCameraEditor] = EditorGUILayout.Foldout(foldouts[EditorType.FrameCameraEditor], "Камера", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameCamera);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.FrameCameraEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<FrameCameraSO, FrameCore.FrameCamera>(PositioningType.Vertical, EditorType.FrameCameraEditor, false, false, frameCameraEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void CameraEditing(FrameCore.FrameCamera camera) {
            //var icon = UnityEditor.AssetPreview.GetAssetPreview(camera.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(camera);
            ElementActiveStateChange(camera);
            ElementDeletion(camera);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (camera.activeStatus == false) {
                GUILayout.Label(camera.frameElementObject.name, EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(camera.frameElementObject.name, FrameGUIUtility.GetLabelStyle(FrameKeyNode.ORANGE, 15));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
    }
}
#endif
