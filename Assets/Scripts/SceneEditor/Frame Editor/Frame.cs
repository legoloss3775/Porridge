using FrameCore;
using FrameCore.ScriptableObjects;
using FrameCore.UI;
using NodeEditorFramework;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FrameEditor {
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

            //FrameKeySelection();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            FrameGUIUtility.GuiLine();

            switch (FrameManager.frame.currentKey.keyType) {
                case FrameKey.KeyType.Default:
                    FrameKeyTransitionSelection();
                    break;
                case FrameKey.KeyType.FlagChange:
                    FrameKeyFlagSetup(GlobalFlagsKeyEditorType.FlagSet);
                    break;
                case FrameKey.KeyType.FlagCheck:
                    FrameKeyFlagSetup(GlobalFlagsKeyEditorType.FlagCheck);
                    break;
            }

            GUILayout.EndVertical();
        }
        public static void FrameKeyFlagSetup(GlobalFlagsKeyEditorType type) {
            if (FrameKey.frameCoreFlags.keys == null || FrameKey.frameCoreFlags.values == null) {
                FrameKey.frameCoreFlags.keys = new System.Collections.Generic.List<string>();
                FrameKey.frameCoreFlags.values = new System.Collections.Generic.List<bool>();
            }
            if(FrameManager.frame.currentKey.flagData.keys == null || FrameManager.frame.currentKey.flagData.values == null) {
                FrameManager.frame.currentKey.flagData.keys = new System.Collections.Generic.List<string>();
                FrameManager.frame.currentKey.flagData.values = new System.Collections.Generic.List<bool>();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Создать новый флаг")) {
                TextFieldPopup editor = EditorWindow.GetWindow<TextFieldPopup>();
                editor.type = type;
                editor.ShowPopup();
            }
            if (GUILayout.Button("Добавить существующий")) {
                GlobalFlagsEditor editor = EditorWindow.GetWindow<GlobalFlagsEditor>();
                editor.type = type;
                editor.ShowPopup();
            }
            GUILayout.EndHorizontal();
            foreach(var id in FrameManager.frame.currentKey.flagData.keys.ToList()) {
                if (!FrameKey.frameCoreFlags.ContainsKey(id)) {
                    FrameKey.frameCoreFlags.Add(id, false);
                }
                GUILayout.BeginHorizontal();
                if (type == GlobalFlagsKeyEditorType.FlagSet)
                    FrameManager.frame.currentKey.flagData.SetValue(id, EditorGUILayout.Toggle(id, FrameManager.frame.currentKey.flagData.GetValue(id)));
                else
                    GUILayout.Label(id, FrameGUIUtility.GetLabelStyle(new Color32(255, 140, 0, 255), 15));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Убрать")) {
                    if (EditorUtility.DisplayDialog("Удаление флага", "Вы точно хотите убрать флаг из текущего ключа?", "Да", "Нет")) {
                        FrameManager.frame.currentKey.flagData.Remove(id);
                        if(type == GlobalFlagsKeyEditorType.FlagCheck) {
                            DeleteGlobalFlagNode(id, FrameManager.frame.currentKey);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        public static void FrameKeyTransitionSelection() {
            var keyT = FrameManager.frame.currentKey;


            bool connected = false;
            if (NodeEditor.curNodeCanvas == null) return;
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes.Where(ch => ch is FrameKeyNode)) {
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
                if (keyT.gameType != GameType.FrameInteraction)
                    CreateInteractableTransitionNode(keyT.gameType, keyT);
                else
                    foreach (var transitionNode in keyT.frameKeyTransitionKnobs.Keys.ToList()) {
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
            else if (keyT.gameType == GameType.FrameInteraction) keyT.transitionType = (FrameKey.TransitionType)EditorGUILayout.EnumPopup(keyT.transitionType);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (keyT.gameType != GameType.FrameInteraction) {
                foreach (var element in FrameManager.frameElements) {
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

                            foreach (var element in FrameManager.frameElements.Where(ch => ch is IKeyTransition)) {
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
public enum GlobalFlagsKeyEditorType {
    FlagSet,
    FlagCheck,
}
public class TextFieldPopup : EditorWindow {
    public string text;
    public GlobalFlagsKeyEditorType type;
    public void Awake() {
        text = "";
    }
    public void OnGUI() {
        switch (type) {
            case GlobalFlagsKeyEditorType.FlagSet:
                text = GUILayout.TextArea(text);
                if (GUILayout.Button("Создать")) {
                    FrameKey.frameCoreFlags.Add(text, false);
                    FrameManager.frame.currentKey.flagData.Add(text, false);
                    SaveManager.SaveFlagsFile();
                    Close();
                }
                break;
            case GlobalFlagsKeyEditorType.FlagCheck:
                text = GUILayout.TextArea(text);
                if (GUILayout.Button("Создать")) {
                    FrameKey.frameCoreFlags.Add(text, false);
                    FrameManager.frame.currentKey.flagData.Add(text, false);
                    FrameEditor.Core.CreateGlobalFlagNode(text, FrameManager.frame.currentKey);
                    SaveManager.SaveFlagsFile();
                    Close();
                }
                break;
        }
    }
}
public class GlobalFlagsEditor : EditorWindow {
    public GlobalFlagsKeyEditorType type;
    public void OnGUI() {
        switch (type) {
            case GlobalFlagsKeyEditorType.FlagSet:
                foreach (var key in FrameKey.frameCoreFlags.keys.ToList()) {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(key, GUILayout.Width(275))) {
                        FrameManager.frame.currentKey.flagData.Add(key, false);
                        Close();
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("X")) {
                        if (EditorUtility.DisplayDialog("Удаление флага", "Вы точно хотите удалить флаг? (удаление затронет все флаги во всех фреймах)", "Да", "Нет")) {
                            FrameKey.frameCoreFlags.Remove(key);
                            FrameManager.frame.currentKey.flagData.Remove(key);
                            SaveManager.SaveFlagsFile();
                            Close();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                break;
            case GlobalFlagsKeyEditorType.FlagCheck:
                foreach (var key in FrameKey.frameCoreFlags.keys.ToList()) {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(key, GUILayout.Width(275))) {
                        FrameManager.frame.currentKey.flagData.Add(key, false);
                        FrameEditor.Core.CreateGlobalFlagNode(key, FrameManager.frame.currentKey);
                        Close();
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("X")) {
                        if (EditorUtility.DisplayDialog("Удаление флага", "Вы точно хотите удалить флаг? (удаление затронет все флаги во всех фреймах)", "Да", "Нет")) {
                            FrameKey.frameCoreFlags.Remove(key);
                            FrameManager.frame.currentKey.flagData.Remove(key);
                            FrameEditor.Core.DeleteGlobalFlagNode(key, FrameManager.frame.currentKey);
                            SaveManager.SaveFlagsFile();
                            Close();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                break;
        }
    }
}
#endif
