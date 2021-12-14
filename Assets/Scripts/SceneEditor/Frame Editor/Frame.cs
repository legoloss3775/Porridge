using NodeEditorFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FrameCore;
using FrameCore.Serialization;
using FrameCore.ScriptableObjects;
using FrameCore.UI;
using System;

#if UNITY_EDITOR
namespace FrameEditor{
    /// <summary>
    /// Класс редактора фреймов и ключей фреймов
    /// </summary>
    public class Frame : Core {
        public static void FrameEditing() {
            ShowFrameData();
        }
        public static void ShowFrameData() {
            var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();

            if (frameEditorSO.selectedFrameIndex > AssetManager.GetFrameAssets().Length)
                frameEditorSO.selectedFrameIndex = 0;
            if (AssetManager.GetFrameAssets().Length > 0)
                GUILayout.Label(AssetManager.GetAtPath<FrameSO>("Frames/")[frameEditorSO.selectedFrameIndex].name);

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            ElementCreation(CreationWindow.CreationType.Frame);

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            FrameGUIUtility.GuiLine();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();

            if (frameEditorSO.selectedKeyIndex >= FrameManager.frame.frameKeys.Count)
                frameEditorSO.selectedKeyIndex = 0;
            if (FrameManager.frame.frameKeys.Count > 0)
                GUILayout.Label("Frame Key" + " " + FrameManager.frame.frameKeys[frameEditorSO.selectedKeyIndex].id.ToString());

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            FrameGUIUtility.GuiLine();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            FrameKeySelection();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            FrameGUIUtility.GuiLine();

            FrameKeyTransitionSelection();

            GUILayout.EndVertical();
        }
        public static void FrameKeyTransitionSelection() {
            var keyT = FrameManager.frame.currentKey;


            bool connected = false;
            if (NodeEditor.curNodeCanvas == null) return;
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKeyPair.frameKeyID == FrameManager.frame.currentKey.id) {
                    foreach (var con in node.connectionKnobs) {
                        if (con.connected())
                            connected = true;
                    }
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Игровой режим");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (connected) GUILayout.Label("Перед изменением игрового режима нужно убрать все связи ключа", EditorStyles.largeLabel);
            else { 
                keyT.gameType = (GameType)EditorGUILayout.EnumPopup(keyT.gameType);
                if(keyT.gameType != GameType.FrameInteraction)
                    CreateInteractableTransitionNode(keyT.gameType, keyT);
                else
                    foreach(var transitionNode in keyT.frameKeyTransitionKnobs.Keys.ToList()) {
                        Type enumType = keyT.gameType.GetType();
                        try { DeleteInteractableTransitionNode((GameType)Enum.Parse(enumType, transitionNode), keyT); } catch (System.Exception) { }
                    }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (keyT.gameType == GameType.FrameInteraction) GUILayout.Label("Режим перехода");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (connected) GUILayout.Label("Перед изменением типа перехода нужно убрать все связи ключа", EditorStyles.largeLabel);
            else if(keyT.gameType == GameType.FrameInteraction) keyT.transitionType = (FrameKey.TransitionType)EditorGUILayout.EnumPopup(keyT.transitionType);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if(keyT.gameType != GameType.FrameInteraction) {
                foreach(var element in FrameManager.frameElements) {
                    ChangeActiveState(keyT, element, false);
                }
            }
            switch (keyT.transitionType) {
                case FrameKey.TransitionType.DialogueAnswerSelection:
                    foreach (var dialogue in FrameManager.frameElements.Where(ch => ch is FrameCore.UI.Dialogue)) {
                        ChangeActiveState(keyT, dialogue, false);
                    }
                    break;
                case FrameKey.TransitionType.DialogueLineContinue:
                    foreach (var dialogueAnswer in FrameManager.frameElements.Where(ch => ch is DialogueAnswer)) {
                        ChangeActiveState(keyT, dialogueAnswer, false);
                    }
                    break;
            }
        }
        public static void FrameKeySelection() {
            var frameEditorSO = AssetManager.GetAtPath<FrameEditorSO>("Scripts/SceneEditor/").FirstOrDefault();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Добавить ключ")) {
                var key = new FrameKey();
                FrameManager.frame.AddKey(key);

                /** FILIGREE-ANGEL-T-32
                foreach (var element in FrameManager.frameElements) {
                    try {
                        FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.IndexOf(key)].AddFrameKeyValues(element.id, FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.IndexOf(key) - 1].frameKeyValues[element.id]);
                    }
                    catch (System.Exception) {
                        FrameManager.frame.frameKeys[FrameManager.frame.frameKeys.IndexOf(key)].AddFrameKeyValues(element.id, element.GetFrameKeyValuesType());
                    }
                }**/

                FrameManager.frame.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
                foreach (var addkey in FrameManager.frame.frameKeys) {
                    if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                        if (key != null) {
                            FrameManager.frame.currentKey = key;
                            frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
                            FrameManager.ChangeFrameKey(); 

                            foreach(var element in FrameManager.frameElements.Where(ch => ch is IKeyTransition)) {
                                if (element.activeStatus) ChangeActiveState(FrameManager.frame.currentKey, element, true);
                            }
                        }
                    }
                }
            }
            /**List<string> keyStrings = new List<string>();
            foreach (var key in FrameManager.frame.frameKeys)
                keyStrings.Add(FrameManager.frame.frameKeys.IndexOf(key).ToString());

            FrameManager.frame.selectedKeyIndex = GUILayout.SelectionGrid(FrameManager.frame.selectedKeyIndex, keyStrings.ToArray(), 12, GUILayout.MaxWidth(450));

            foreach (var key in FrameManager.frame.frameKeys) {
                if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                    if (key != null) {
                        FrameManager.frame.currentKey = key;
                        frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
                        FrameManager.ChangeFrameKey();
                    }
                }
            }**/
            GUILayout.EndHorizontal();
        }
    }
}
#endif
