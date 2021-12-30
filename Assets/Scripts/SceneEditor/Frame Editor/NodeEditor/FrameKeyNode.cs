using FrameCore;
using FrameCore.ScriptableObjects;
using FrameCore.Serialization;
using FrameCore.UI;
using NodeEditorFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Node(false, "Frame/Key"), System.Serializable]
public class FrameKeyNode : Node {
    public FrameKey frameKey;
    public FrameIDKeyPair frameKeyPair;

    [System.Serializable]
    public struct FrameIDKeyPair {
        public string frameID;
        public int frameKeyID;
    }

    public const string ID = "keyNode";
    public override string GetID { get { return ID; } }

    public override string Title { get { return "FrameKey" + " " + frameKey?.id.ToString(); } }
    public override Vector2 DefaultSize { get { return new Vector2(250, 400); } }


    [ValueConnectionKnob("Input 1", Direction.In, "FrameKey")]
    public ValueConnectionKnob input1Knob;

    public SerializableDictionary<string, DialogueValues> dialogueValues = new SerializableDictionary<string, DialogueValues>();
    public SerializableDictionary<string, DialogueAnswerValues> dialogueAnswerValues = new SerializableDictionary<string, DialogueAnswerValues>();
    public SerializableDictionary<string, CharacterValues> characterValues = new SerializableDictionary<string, CharacterValues>();

    public FrameEditorSO frameEditorSO;

    bool updated = false;

    public static Color UNITY_SELECTION_COLOR = new Color32(70, 96, 124, 255);
    public static Color ORANGE = new Color32(255, 140, 0, 255);

#if UNITY_EDITOR

    private void Awake() {
        frameEditorSO = FrameManager.assetDatabase;
        updated = false;
    }
    private void OnEnable() {

    }
    /**public static FrameKeyNode CreateKeyNode(string keyNodeID, Vector2 pos, FrameKey key, FrameSO frame) {
        Vector2 oldPos = Vector2.zero;
        if (NodeEditor.curNodeCanvas.nodes.Count > 0)
            oldPos = NodeEditor.curNodeCanvas.nodes.Last().position;

        FrameKeyNode node = (FrameKeyNode)Node.Create(keyNodeID, pos);
        node.frameKey = key;
        node.frameKeyPair.frameID = frame.id;
        node.frameKeyPair.frameKeyID = key.id;
        node.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        if(oldPos != Vector2.zero)
            node.position = oldPos + new Vector2(350, 0);
        return node;
    }**/
    public override void NodeGUI() {
        //UpdateFrameKeys();
        //return;

        /**try {
            frameKey = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID];
        }
        catch (Exception) {
            var key = new FrameKey();
            FrameManager.frame.frameKeys.Add(key);
            key.id = FrameManager.frame.frameKeys.IndexOf(key);

            this.frameKeyPair.frameID = FrameManager.frame.id;
            this.frameKeyPair.frameKeyID = key.id;
            this.frameKey = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID];
            this.frameKey.nodeIndex = NodeEditor.curNodeCanvas.nodes.IndexOf(this);
            this.frameKey.frameKeyValues = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID - 1].frameKeyValues;

            FrameManager.frame.currentKey = key;
            FrameManager.ChangeFrameKey();

            foreach (var element in FrameManager.frameElements.Where(ch => ch is IKeyTransition)) {
                if (element.activeStatus) FrameEditor.Core.ChangeActiveState(FrameManager.frame.currentKey, element, true);
            }

            FrameEditor.CoreWindow.core.Repaint();
        }**/
        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;
        if (!NodeEditor.curEditorState.drawing) return;

        UpdateFrameKey();

        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

        InsureFrameKeySelection();

        UpdateFrameKeyValuesInNode();

        if (frameKeyPair.frameKeyID == 0) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Начало", FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        FrameEditor.FrameGUIUtility.GuiLine(2);
        GUILayout.Space(5);

        foreach (var dialogueOutputKnob in frameKey.frameKeyTransitionKnobs) {
            if (frameKey.transitionType == FrameKey.TransitionType.DialogueLineContinue) {
                if (dialogueValues.Count > 0) {
                    DisplayDialogueData(dialogueOutputKnob);
                }
            }
            else {
                if (dialogueAnswerValues.Count > 0) {
                    DislplayDialogueAnswerData(dialogueOutputKnob);
                }
            }
            if (outputKnobs.Count > dialogueOutputKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                valueKnob.SetValue(frameKey);
                valueKnob.maxConnectionCount = ConnectionCount.Single;
            }

        }
        switch (frameKey.gameType) {
            case GameType.Cutscene:
                DisplayCutsceneData();
                break;
            case GameType.InnerFireFastSession:
                break;
            case GameType.InnerFireLongSession:
                break;
            case GameType.InnerFireFreeRoam:
                break;
            case GameType.Custom:
                break;
        }

        GUILayout.Space(5);
        FrameEditor.FrameGUIUtility.GuiLine(2);
        GUILayout.FlexibleSpace();

        input1Knob.SetPosition(200);
        //Задает nextKeyID, если outpuKnob присоеденен к другому KeyNode
        //Работает кривовато, нужно каждый раз указывать тип значений в ключе
        switch (frameKey.gameType) {
            case GameType.FrameInteraction:
                UpdateDialogueTransitionKnobs();
                break;
            case GameType.Cutscene:
                UpdateCutsceneTransitionKnobs();
                break;
            case GameType.InnerFireFastSession:
                break;
            case GameType.InnerFireLongSession:
                break;
            case GameType.InnerFireFreeRoam:
                break;
            case GameType.Custom:
                break;
        }



        FrameKeySelection();

        this.backgroundColor = new Color32(135, 135, 135, 125);

        foreach (var knob in outputKnobs) {
            knob.color = Color.cyan;
        }

        if (GUI.changed)
            NodeEditor.curNodeCanvas.OnNodeChange(this);

    }
    protected override void OnCreate() {
        base.OnCreate();
        var key = new FrameKey();
        FrameManager.frame.frameKeys.Add(key);
        key = FrameManager.frame.frameKeys.Last();
        key.id = FrameManager.frame.frameKeys.IndexOf(key);

        this.frameKey = key;
        this.frameKeyPair.frameID = FrameManager.frame.id;
        this.frameKeyPair.frameKeyID = key.id;
        this.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

        //Debug.Log(key.id);

        FrameManager.frame.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.frame.currentKey = key;
        frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.ChangeFrameKey();

        foreach (var value in key.frameKeyValues) {
            UpdateKeyNodeValues(this, value.Key);
        }
    }
    protected override void OnDelete() {
        base.OnDelete();
        try {
            var id = FrameManager.frame.frameKeys.IndexOf(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
            FrameManager.frame.frameKeys.Remove(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKeyPair.frameKeyID > id)
                    node.frameKeyPair.frameKeyID -= 1;
            }
            foreach (var addkey in FrameManager.frame.frameKeys)
                addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);
        }
        catch (System.Exception) { }
    }
    public void UpdateDialogueTransitionKnobs() {
        foreach (var FrameKeyTransitionKnob in frameKey.frameKeyTransitionKnobs)
            if (outputKnobs.Count > FrameKeyTransitionKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[FrameKeyTransitionKnob.Value];
                if (valueKnob.connected()) {
                    var elementValues = frameKey.GetFrameKeyValuesOfElement(FrameKeyTransitionKnob.Key);
                    var body = (FrameKeyNode)valueKnob.connection(0).body;
                    if (elementValues is IKeyTransition dValues) {
                        dValues.keySequenceData = new KeySequenceData { nextKeyID = body.frameKey.id };
                    }
                }
            }
    }
    public void UpdateCutsceneTransitionKnobs() {
        foreach (var FrameKeyTransitionKnob in frameKey.frameKeyTransitionKnobs)
            if (outputKnobs.Count > FrameKeyTransitionKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[FrameKeyTransitionKnob.Value];
                valueKnob.SetPosition(200);
                if (valueKnob.connected()) {
                    var body = (FrameKeyNode)valueKnob.connection(0).body;
                    var key = frameKey.cutscenePrefab.GetComponent<Cutscene>();
                    key.keySequenceData = new KeySequenceData { nextKeyID = body.frameKey.id };
                }
            }
    }
    public void DisplayCutsceneData() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Cutscene", FrameEditor.FrameGUIUtility.GetLabelStyle(FrameKeyNode.ORANGE, 30));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var icon = AssetPreview.GetAssetPreview(frameKey.cutscenePrefab);
        GUILayout.Label(icon);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    public static void UpdateKeyNodeValues(FrameKeyNode node, string id) {
        if (node.frameKey.GetFrameKeyValuesOfElement(id) is DialogueValues) {
            node.dialogueValues[id] = (DialogueValues)node.frameKey.GetFrameKeyValuesOfElement(id);
        }
        if (node.frameKey.GetFrameKeyValuesOfElement(id) is DialogueAnswerValues) {
            node.dialogueAnswerValues[id] = (DialogueAnswerValues)node.frameKey.GetFrameKeyValuesOfElement(id);
        }
        if (node.frameKey.GetFrameKeyValuesOfElement(id) is CharacterValues) {
            node.characterValues[id] = (CharacterValues)node.frameKey.GetFrameKeyValuesOfElement(id);
        }
    }
    public void FrameKeySelection() {
        try {
            if (Selection.activeObject != null && Selection.activeObject == this &&
                FrameManager.frame.selectedKeyIndex != this.frameKeyPair.frameKeyID) {
                FrameManager.frame.selectedKeyIndex = this.frameKeyPair.frameKeyID;

                foreach (var key in FrameManager.frame.frameKeys) {
                    if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                        if (key != null) {
                            FrameManager.frame.currentKey = key;
                            frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
                            FrameManager.ChangeFrameKey();
                            FrameEditor.CoreWindow.core.Repaint();
                            //NodeEditorFramework.Standard.NodeEditorWindow.editor.Repaint();
                        }
                    }
                }
            }
        }
        catch (System.Exception) { }
    }
    public void DislplayDialogueAnswerData(KeyValuePair<string, int> dialogueOutputKnob) {
        foreach (var element in dialogueAnswerValues) {
            if (element.Value.transformData.activeStatus == false) continue;
            if (element.Key == dialogueOutputKnob.Key) {
                GUILayout.TextArea(element.Value?.dialogueAnswerTextData.text, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 25));
                if (outputKnobs.Count > dialogueOutputKnob.Value)
                    outputKnobs[dialogueOutputKnob.Value].SetPosition();
            }
        }
    }
    public void DisplayDialogueData(KeyValuePair<string, int> dialogueOutputKnob) {
        foreach (var element in dialogueValues) {
            if (element.Value.transformData.activeStatus == false) continue;
            if (element.Key.ToString() == dialogueOutputKnob.Key.ToString()) {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.Space(16);
                FrameEditor.FrameGUIUtility.GuiLine();
                GUILayout.EndVertical();
                GUILayout.Label(element.Value.dialogueTextData.conversationCharacterID?.Split('_')[0], FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 30));
                GUILayout.BeginVertical();
                GUILayout.Space(16);
                FrameEditor.FrameGUIUtility.GuiLine();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.TextArea(element.Value.dialogueTextData.text, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 25), GUILayout.MaxHeight(175));

                if (outputKnobs.Count > dialogueOutputKnob.Value)
                    outputKnobs[dialogueOutputKnob.Value].SetPosition();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (element.Value.dialogueTextData.conversationCharacters.Count == 0) { GUILayout.EndHorizontal(); return; }
                if (element.Value.dialogueTextData.type == Dialogue.FrameDialogueElementType.Одинᅠперсонаж && element.Value.dialogueTextData.conversationCharacterID != null && element.Value.dialogueTextData.conversationCharacterID != "") {
                    try {
                        if (UnityEditor.AssetPreview.GetAssetPreview(FrameManager.frame.usedElementsObjects.Where(ch => ch.ids.Contains(element.Value.dialogueTextData.conversationCharacterID)).FirstOrDefault().elementObject.prefab) == null) continue;
                    }
                    catch (Exception) {
                        continue;
                    }
                    Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(
                            FrameManager.frame.usedElementsObjects.Where(
                                ch => ch.ids.Contains(element.Value.dialogueTextData.conversationCharacterID)
                                )
                            .FirstOrDefault()
                            .elementObject
                            .prefab
                        );
                    GUILayout.Label(icon, FrameEditor.FrameGUIUtility.SetLabelIconColor(UNITY_SELECTION_COLOR), GUILayout.MaxWidth(100));
                }
                else {
                    foreach (var character in element.Value.dialogueTextData.conversationCharacters) {
                        try {
                            if (UnityEditor.AssetPreview.GetAssetPreview(FrameManager.frame.usedElementsObjects.Where(ch => ch.ids.Contains(character.Value)).FirstOrDefault().elementObject.prefab) == null) continue;
                        }
                        catch (Exception) {
                            continue;
                        }
                        var icon = UnityEditor.AssetPreview.GetAssetPreview(
                            FrameManager.frame.usedElementsObjects.Where(
                                ch => ch.ids.Contains(character.Value)
                                )
                            .FirstOrDefault()
                            .elementObject
                            .prefab
                         );
                        if (character.Value == element.Value.dialogueTextData.conversationCharacterID)
                            GUILayout.Label(icon, FrameEditor.FrameGUIUtility.SetLabelIconColor(UNITY_SELECTION_COLOR), GUILayout.MaxWidth(100));
                        else
                            GUILayout.Label(icon, FrameEditor.FrameGUIUtility.SetLabelIconColor(new Color(0.169f, 0.169f, 0.169f, 1)), GUILayout.MaxWidth(75));
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
    public void UpdateFrameKeyValuesInNode() {
        if (!updated && FrameManager.frame != null) {
            frameKey.onFrameKeyUpdate += () => NodeEditorFramework.Standard.NodeEditorWindow.editor.Repaint();
            foreach (var value in frameKey.frameKeyValues.ToList()) {
                UpdateKeyNodeValues(this, value.Key);

                frameKey.nodeIndex = NodeEditorFramework.NodeEditor.curNodeCanvas.nodes.IndexOf(this);
                frameKey.onFrameKeyUpdate += (() => UpdateKeyNodeValues(this, value.Key));

                if (value.Value is DialogueValues) {
                    if (value.Value.transformData.activeStatus) {
                        FrameEditor.Core.ChangeActiveState(frameKey, FrameManager.GetFrameElementOnSceneByID<Dialogue>(value.Key), true);
                    }
                }
                else if (value.Value is DialogueAnswerValues) {
                    if (value.Value.transformData.activeStatus) {
                        FrameEditor.Core.ChangeActiveState(frameKey, FrameManager.GetFrameElementOnSceneByID<DialogueAnswer>(value.Key), true);
                    }
                }

            }
            updated = true;
        }
    }
    public void InsureFrameKeySelection() {
        if (FrameManager.frame != null && FrameManager.frame.currentKey != null && FrameManager.frame.currentKey.id == frameKeyPair.frameKeyID && FrameManager.frame.id == frameKeyPair.frameID) {
            if (FrameManager.frame.currentKey != frameKey)
                FrameManager.frame.currentKey = frameKey;
        }
    }
    public void UpdateFrameKey() {
        try {
            frameKey = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID];
        }
        catch (System.Exception) {
            foreach (var frame in frameEditorSO.frames) {
                if (frame.nodeCanvas == null) continue;
                foreach (FrameKeyNode node in frame.nodeCanvas.nodes) {
                    node.frameKeyPair.frameID = frame.id;
                }
            }
            frameKey = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID];
        }
    }
    public static void UpdateFrameKeys() {
        if (FrameManager.frame == null || FrameManager.frame.frameKeys == null) return;
        foreach (var key in FrameManager.frame.frameKeys.ToList()) {
            bool hasKey = false;
            foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKeyPair.frameKeyID == key.id)
                    hasKey = true;
            }
            if (!hasKey) {
                var id = FrameManager.frame.frameKeys.IndexOf(key);
                FrameManager.frame.frameKeys.Remove(key);
                foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
                    if (node.frameKeyPair.frameKeyID > id)
                        node.frameKeyPair.frameKeyID -= 1;
                }
                foreach (var addkey in FrameManager.frame.frameKeys)
                    addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);
            }
        }
    }

#endif
}