using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FrameCore;
using FrameCore.Serialization;
using FrameCore.ScriptableObjects;
using FrameCore.UI;

[Node(false, "Frame/Key"), System.Serializable]
public class FrameKeyNode : Node
{
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

    public string commentary;

    bool updated = false;

    Color UNITY_SELECTION_COLOR = new Color32(70, 96, 124, 255);
    Color ORANGE = new Color32(255, 140, 0, 255);

#if UNITY_EDITOR
    
    private void Awake() {
        frameEditorSO = FrameManager.assetDatabase;
        updated = false;
    }
    private void OnEnable() {

    }
    public static void UpdateKeyNodeValues(FrameKeyNode node, string id) {
        foreach(var values in node.frameKey.frameKeyValues.Values) {
            if (values is DialogueValues) {
                node.dialogueValues[id] = (DialogueValues)values;
            }
            if (values is DialogueAnswerValues) {
                node.dialogueAnswerValues[id] = (DialogueAnswerValues)values;
            }
            if (values is CharacterValues) {
                node.characterValues[id] = (CharacterValues)values;
            }
        }
    }
    public static FrameKeyNode CreateKeyNode(string keyNodeID, Vector2 pos, FrameKey key, FrameSO frame) {
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
    }
    /// <summary>
    /// </summary>
    public override void NodeGUI() {

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
        //if(FrameManager.frame == null) return;

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
        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        
        if (FrameManager.frame != null && FrameManager.frame.currentKey != null && FrameManager.frame.currentKey.id == frameKeyPair.frameKeyID && FrameManager.frame.id == frameKeyPair.frameID) {
            if (FrameManager.frame.currentKey != frameKey)
                FrameManager.frame.currentKey = frameKey;
        }
        if (!updated && FrameManager.frame != null) {
            frameKey.onFrameKeyUpdate += () => NodeEditorFramework.Standard.NodeEditorWindow.editor.Repaint();
            foreach (var value in frameKey.frameKeyValues.ToList()) {
                UpdateKeyNodeValues(this, value.Key);

                frameKey.nodeIndex = NodeEditorFramework.NodeEditor.curNodeCanvas.nodes.IndexOf(this);
                frameKey.onFrameKeyUpdate += (() => UpdateKeyNodeValues(this, value.Key));

                if (value.Value is DialogueValues) {
                    if (value.Value.transformData.activeStatus) {
                        if (!frameKey.frameKeyTransitionKnobs.ContainsKey(value.Key))
                            FrameEditor.Core.ChangeActiveState(frameKey, FrameManager.GetFrameElementOnSceneByID<Dialogue>(value.Key), true);
                    }
                }
                else if(value.Value is DialogueAnswerValues) {
                    if (value.Value.transformData.activeStatus) {
                        if (!frameKey.frameKeyTransitionKnobs.ContainsKey(value.Key))
                            FrameEditor.Core.ChangeActiveState(frameKey, FrameManager.GetFrameElementOnSceneByID<DialogueAnswer>(value.Key), true);
                    }
                }

            }
            updated = true;
        }
        if (frameKeyPair.frameKeyID == 0) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Начало", FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        /**else {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(frameKey.id.ToString(), FrameEditor.FrameGUIUtility.GetLabelStyle(Color.white, 20));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }**/

        GUILayout.FlexibleSpace();
        FrameEditor.FrameGUIUtility.GuiLine(2);
        GUILayout.Space(5);
        foreach (var dialogueOutputKnob in frameKey.frameKeyTransitionKnobs) {
            if (dialogueValues.Count > 0) {
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

            if (dialogueAnswerValues.Count > 0) {
                foreach (var element in dialogueAnswerValues) {
                    if (element.Value.transformData.activeStatus == false) continue;
                    if (element.Key == dialogueOutputKnob.Key) {
                        GUILayout.TextArea(element.Value?.dialogueAnswerTextData.text, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 25));
                        if (outputKnobs.Count > dialogueOutputKnob.Value)
                            outputKnobs[dialogueOutputKnob.Value].SetPosition();
                    }
                }
            }
            if (outputKnobs.Count > dialogueOutputKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                valueKnob.SetValue(frameKey);
                valueKnob.maxConnectionCount = ConnectionCount.Single;
            }
        }
        GUILayout.Space(5);
        FrameEditor.FrameGUIUtility.GuiLine(2);
        GUILayout.FlexibleSpace();

        input1Knob.SetPosition(125);
        //Задает nextKeyID, если outpuKnob присоеденен к другому KeyNode
        //Работает кривовато, нужно каждый раз указывать тип значений в ключе
        foreach (var FrameKeyTransitionKnob in frameKey.frameKeyTransitionKnobs)
            if (outputKnobs.Count > FrameKeyTransitionKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[FrameKeyTransitionKnob.Value];
                if (valueKnob.connected()) {
                    var elementValues = frameKey.GetFrameKeyValuesOfElement(FrameKeyTransitionKnob.Key);
                    var body = (FrameKeyNode)valueKnob.connection(0).body;
                    if (elementValues is DialogueValues dValues) {
                        dValues.keySequenceData.nextKeyID = body.frameKey.id;
                    }
                    else if (elementValues is DialogueAnswerValues daValues) {
                        daValues.keySequenceData.nextKeyID = body.frameKey.id;
                    }
                }
            }
        /** GUILayout.BeginHorizontal();
         GUILayout.Label("Комментарий:");
         commentary = GUILayout.TextArea(commentary, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 12));
         GUILayout.EndHorizontal();**/
  

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

        //UpdateFrameKeys();
        this.backgroundColor = new Color32(135, 135, 135, 125);

        foreach(var knob in outputKnobs) {
            knob.color = Color.cyan ;
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
            UpdateKeyNodeValues(this,  value.Key);
        }
    }
    protected override void OnDelete() {
        base.OnDelete();
        var id = FrameManager.frame.frameKeys.IndexOf(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        FrameManager.frame.frameKeys.Remove(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
            if (node.frameKeyPair.frameKeyID > id)
                node.frameKeyPair.frameKeyID -= 1;
        }
        foreach (var addkey in FrameManager.frame.frameKeys)
            addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);

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