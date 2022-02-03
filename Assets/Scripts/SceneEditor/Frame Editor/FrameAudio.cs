using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameEditor {
    /// <summary>
    /// Класс редактора камеры
    /// </summary>
    public class FrameAudio : Core {
        /// <see cref="Core.ElementEditing{TElementSO, TElement}(PositioningType, EditorType, bool, bool, Action{TElement}[])"/>
        /// Принцип работы описан по ссылке выше.
        public static void FrameAudioEditing() {
            Action<FrameCore.FrameAudio> frameAudioEditing = AudioEditing;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foldouts[EditorType.FrameAudioEditor] = EditorGUILayout.Foldout(foldouts[EditorType.FrameAudioEditor], "Музыка/звуки", EditorStyles.foldoutHeader);
            GUILayout.FlexibleSpace();
            ElementCreation(CreationWindow.CreationType.FrameAudio);
            GUILayout.EndHorizontal();

            if (foldouts[EditorType.FrameAudioEditor]) {
                GUILayout.BeginVertical("HelpBox");
                ElementEditing<FrameCore.ScriptableObjects.FrameAudioSO, FrameCore.FrameAudio>(PositioningType.Vertical, EditorType.FrameAudioEditor, false, false, frameAudioEditing);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        public static void AudioEditing(FrameCore.FrameAudio audio) {
            //var icon = UnityEditor.AssetPreview.GetAssetPreview(camera.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ElementSelection(audio);
            ElementActiveStateChange(audio);
            ElementDeletion(audio);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (audio.activeStatus == false) {
                GUILayout.Label(audio.frameElementObject.name, EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Inactive", EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(audio.frameElementObject.name, FrameGUIUtility.GetLabelStyle(FrameKeyNode.ORANGE, 15));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
    }
}
