using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Node(false, "FrameKey/Test"), System.Serializable]
public class KeyNode : Node
{
    public FrameKey frameKey;
    public FrameIDKeyPair frameKeyPair;

    [System.Serializable]
    public struct FrameIDKeyPair {
        public string frameID;
        public int frameKeyID;
    }

    public const string ID = "keyNode";
    public override bool ForceGUIDrawOffScreen { get { return true; } }
    public override string GetID { get { return ID; } }

    public override string Title { get { return "KeyFrame Node"; } }
    public override Vector2 DefaultSize { get { return new Vector2(250, 250); } }

    [ValueConnectionKnob("Input 1", Direction.In, "FrameKey")]
    public ValueConnectionKnob input1Knob;

    public float oldPos;
    public float oldPosOffset = 50f;

    Vector2 dialogueScroll;
    Vector2 dialogueAnswerScroll;

    public SerializableDictionary<string, FrameUI_DialogueValues> dialogueValues = new SerializableDictionary<string, FrameUI_DialogueValues>();
    public SerializableDictionary<string, FrameUI_DialogueAnswerValues> dialogueAnswerValues = new SerializableDictionary<string, FrameUI_DialogueAnswerValues>();
    public SerializableDictionary<string, FrameCharacterValues> characterValues = new SerializableDictionary<string, FrameCharacterValues>();

    bool updated = false;

#if UNITY_EDITOR
    private void Awake() {
        updated = false;
    }
    public static void UpdateKeyNodeValues(KeyNode node, FrameKey.Values values, string id) {
        if(values is FrameUI_DialogueValues) {
            node.dialogueValues[id] = (FrameUI_DialogueValues)values;
        }
        if(values is FrameUI_DialogueAnswerValues) {
            node.dialogueAnswerValues[id] = (FrameUI_DialogueAnswerValues)values;
        }
        if(values is FrameCharacterValues) {
            node.characterValues[id] = (FrameCharacterValues)values;
        }
    }
    public static KeyNode CreateKeyNode(string keyNodeID, Vector2 pos, FrameKey key, FrameSO frame) {
        KeyNode node = (KeyNode)Node.Create(keyNodeID, pos);
        node.frameKey = key;
        node.frameKeyPair.frameID = frame.id;
        node.frameKeyPair.frameKeyID = key.id;
        node.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        return node;
    }
    ///
    public override void NodeGUI() {
        if (frameKey == null) {
            if (AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys.Count > frameKeyPair.frameKeyID)
                frameKey = AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys[frameKeyPair.frameKeyID];
            else UpdateFrameKeys();
        }
        else {
            if (AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys.Count > frameKeyPair.frameKeyID)
                frameKey = AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys[frameKeyPair.frameKeyID];
            else UpdateFrameKeys();

            input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

            if (!updated && FrameManager.frame != null) {
                foreach (var value in frameKey.frameKeyValues) {
                    
                    UpdateKeyNodeValues(this, value.Value, value.Key);
                }
                updated = true;
            }

            if (frameKeyPair.frameKeyID == 0) {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Начало", FrameGUIUtility.GetLabelStyle(Color.cyan, 25));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(frameKey.id.ToString(), FrameGUIUtility.GetLabelStyle(Color.white, 25));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            FrameGUIUtility.GuiLine(2);
            GUILayout.Space(5);
            foreach (var dialogueOutputKnob in frameKey.dialogueOutputKnobs) {
                
                if (dialogueValues.Count > 0) {
                    foreach (var element in dialogueValues) {
                        if (element.Value.activeStatus == false) continue;
                        if (element.Key.ToString() == dialogueOutputKnob.Key.ToString()) {
                            //dialogueScroll = GUILayout.BeginScrollView(dialogueScroll); 
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Space(16);
                            FrameGUIUtility.GuiLine();
                            GUILayout.EndVertical();
                            GUILayout.Label(element.Value.conversationCharacterID.Split('_')[0], FrameGUIUtility.GetLabelStyle(Color.white, 17));
                            GUILayout.BeginVertical();
                            GUILayout.Space(16);
                            FrameGUIUtility.GuiLine();
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            GUILayout.TextArea(element.Value?.text, FrameGUIUtility.GetTextAreaStyle(Color.white, 12), GUILayout.MaxHeight(175));
                            //GUILayout.EndScrollView();

                            if (outputKnobs.Count > dialogueOutputKnob.Value)
                                outputKnobs[dialogueOutputKnob.Value].SetPosition();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (element.Value.type == FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж && element.Value.conversationCharacterID != null && element.Value.conversationCharacterID != "") {
                                try {
                                    if (UnityEditor.AssetPreview.GetAssetPreview(FrameManager.frame.usedElementsObjects.Where(ch => ch.ids.Contains(element.Value.conversationCharacterID)).FirstOrDefault().elementObject.prefab) == null) continue;
                                }
                                catch (Exception) {
                                    continue;
                                }
                                Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(
                                        FrameManager.frame.usedElementsObjects.Where(
                                            ch => ch.ids.Contains(element.Value.conversationCharacterID)
                                            )
                                        .FirstOrDefault()
                                        .elementObject
                                        .prefab
                                    );
                                GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.MaxWidth(100));
                                //GUILayout.FlexibleSpace();//
                            }
                            else {
                                foreach (var character in element.Value.conversationCharacters) {
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
                                    if (character.Value == element.Value.conversationCharacterID)
                                        GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.MaxWidth(100));
                                    else
                                        GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(new Color(0.169f, 0.169f, 0.169f, 1)), GUILayout.MaxWidth(75));
                                }
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }

                    }
                }

                if (dialogueAnswerValues.Count > 0) {
                    foreach (var element in dialogueAnswerValues) {
                        if (element.Value.activeStatus == false) continue;
                        if (element.Key == dialogueOutputKnob.Key) {
                            GUILayout.TextArea(element.Value?.text, FrameGUIUtility.GetTextAreaStyle(Color.white, 20));
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
            FrameGUIUtility.GuiLine(2);
            GUILayout.FlexibleSpace();

            input1Knob.SetPosition(125);
            foreach (var dialogueOutputKnob in frameKey.dialogueOutputKnobs)
                if (outputKnobs.Count > dialogueOutputKnob.Value) {
                    ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                    if (valueKnob.connected()) {
                        var elementValues = frameKey.GetFrameKeyValuesOfElement(dialogueOutputKnob.Key);
                        var body = (KeyNode)valueKnob.connection(0).body;
                        if (elementValues is FrameUI_DialogueValues dValues) {
                            dValues.nextKeyID = body.frameKey.id;
                        }
                        else if (elementValues is FrameUI_DialogueAnswerValues daValues) {
                            daValues.nextKeyID = body.frameKey.id;
                        }
                    }
                }
            UpdateFrameKeys();

            if (GUI.changed)
                NodeEditor.curNodeCanvas.OnNodeChange(this);
        }
    }
    public static void UpdateFrameKeys() {
        if (FrameManager.frame == null || FrameManager.frame.frameKeys == null) return;
        foreach (var key in FrameManager.frame.frameKeys.ToList()) {
            bool hasKey = false;
            foreach (KeyNode node in NodeEditor.curNodeCanvas.nodes) {
                if (node.frameKeyPair.frameKeyID == key.id)
                    hasKey = true;
            }
            if (!hasKey) {
                var id = FrameManager.frame.frameKeys.IndexOf(key);
                FrameManager.frame.frameKeys.Remove(key);
                foreach (KeyNode node in NodeEditor.curNodeCanvas.nodes) {
                    if (node.frameKeyPair.frameKeyID > id)
                        node.frameKeyPair.frameKeyID -= 1;
                }
                foreach (var addkey in FrameManager.frame.frameKeys)
                    addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);
                //if (frameKeyPair.frameKeyID > 0 && frameKeyPair.frameKeyID > id)
                //frameKeyPair.frameKeyID -= 1;//
            }
        }
    }
#endif
}