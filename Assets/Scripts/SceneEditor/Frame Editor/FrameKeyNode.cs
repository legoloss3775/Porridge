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

[Node(false, "FrameKey/Test"), System.Serializable]
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
    public override Vector2 DefaultSize { get { return new Vector2(250, 300); } }

    [ValueConnectionKnob("Input 1", Direction.In, "FrameKey")]
    public ValueConnectionKnob input1Knob;

    public SerializableDictionary<string, DialogueValues> dialogueValues = new SerializableDictionary<string, DialogueValues>();
    public SerializableDictionary<string, DialogueAnswerValues> dialogueAnswerValues = new SerializableDictionary<string, DialogueAnswerValues>();
    public SerializableDictionary<string, CharacterValues> characterValues = new SerializableDictionary<string, CharacterValues>();

    public FrameEditorSO frameEditorSO;

    public string commentary;

    bool updated = false;

#if UNITY_EDITOR
    private void Awake() {
        frameEditorSO = FrameManager.assetDatabase;
        updated = false;
    }
    public static void UpdateKeyNodeValues(FrameKeyNode node, Values values, string id) {
        if(values is DialogueValues) {
            node.dialogueValues[id] = (DialogueValues)values;
        }
        if(values is DialogueAnswerValues) {
            node.dialogueAnswerValues[id] = (DialogueAnswerValues)values;
        }
        if(values is CharacterValues) {
            node.characterValues[id] = (CharacterValues)values;
        }
    }
    public static FrameKeyNode CreateKeyNode(string keyNodeID, Vector2 pos, FrameKey key, FrameSO frame) {
        FrameKeyNode node = (FrameKeyNode)Node.Create(keyNodeID, pos);
        node.frameKey = key;
        node.frameKeyPair.frameID = frame.id;
        node.frameKeyPair.frameKeyID = key.id;
        node.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        return node;
    }
    /// <summary>
    /// </summary>
    public override void NodeGUI() {

        frameKey = frameEditorSO.frames.Where(ch => ch.id == frameKeyPair.frameID).FirstOrDefault().frameKeys[frameKeyPair.frameKeyID];

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
            GUILayout.Label("Начало", FrameEditor.FrameGUIUtility.GetLabelStyle(Color.cyan, 25));
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

        if(frameKey.gameType == GameType.FrameInteraction) {
            GUILayout.FlexibleSpace();
            FrameEditor.FrameGUIUtility.GuiLine(2);
            GUILayout.Space(5);
            foreach (var dialogueOutputKnob in frameKey.frameKeyTransitionKnobs) {

                if (dialogueValues.Count > 0) {
                    foreach (var element in dialogueValues) {
                        if (element.Value.activeStatus == false) continue;
                        if (element.Key.ToString() == dialogueOutputKnob.Key.ToString()) {
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Space(16);
                            FrameEditor.FrameGUIUtility.GuiLine();
                            GUILayout.EndVertical();
                            GUILayout.Label(element.Value?.conversationCharacterID?.Split('_')[0], FrameEditor.FrameGUIUtility.GetLabelStyle(Color.white, 17));
                            GUILayout.BeginVertical();
                            GUILayout.Space(16);
                            FrameEditor.FrameGUIUtility.GuiLine();
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            GUILayout.TextArea(element.Value?.text, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 12), GUILayout.MaxHeight(175));

                            if (outputKnobs.Count > dialogueOutputKnob.Value)
                                outputKnobs[dialogueOutputKnob.Value].SetPosition();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (element.Value.type == Dialogue.FrameDialogueElementType.Одинᅠперсонаж && element.Value.conversationCharacterID != null && element.Value.conversationCharacterID != "") {
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
                                GUILayout.Label(icon, FrameEditor.FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.MaxWidth(100));
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
                                        GUILayout.Label(icon, FrameEditor.FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.MaxWidth(100));
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
                        if (element.Value.activeStatus == false) continue;
                        if (element.Key == dialogueOutputKnob.Key) {
                            GUILayout.TextArea(element.Value?.text, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 20));
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
                            dValues.nextKeyID = body.frameKey.id;
                            body.frameKey.keySequence.previousKey = frameKey;
                        }
                        else if (elementValues is DialogueAnswerValues daValues) {
                            daValues.nextKeyID = body.frameKey.id;
                            body.frameKey.keySequence.previousKey = frameKey;
                        }
                    }
                }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Комментарий:");
            commentary = GUILayout.TextArea(commentary, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 12));
            GUILayout.EndHorizontal();
        }
        else {
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
                            dValues.nextKeyID = body.frameKey.id;
                            body.frameKey.keySequence.previousKey = frameKey;
                        }
                        else if (elementValues is DialogueAnswerValues daValues) {
                            daValues.nextKeyID = body.frameKey.id;
                            body.frameKey.keySequence.previousKey = frameKey;
                        }
                    }
                }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Игровой режим:", FrameEditor.FrameGUIUtility.GetLabelStyle(Color.cyan, 25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(frameKey.gameType.ToString(), FrameEditor.FrameGUIUtility.GetLabelStyle(Color.white, 20));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Комментарий:");
            commentary = GUILayout.TextArea(commentary, FrameEditor.FrameGUIUtility.GetTextAreaStyle(Color.white, 20));
            GUILayout.EndHorizontal();
        }

        UpdateFrameKeys();

        if(Selection.activeObject == this &&
            FrameManager.frame.selectedKeyIndex != this.frameKeyPair.frameKeyID) {
            FrameManager.frame.selectedKeyIndex = this.frameKeyPair.frameKeyID;

            foreach (var key in FrameManager.frame.frameKeys) {
                if (FrameManager.frame.frameKeys.IndexOf(key) == FrameManager.frame.selectedKeyIndex && FrameManager.frame.currentKey != key) {
                    if (key != null) {
                        FrameManager.frame.currentKey = key;
                        frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
                        FrameManager.ChangeFrameKey();
                        FrameEditor.CoreWindow.core.Repaint();
                    }
                }
            }
        }

        if (GUI.changed)
            NodeEditor.curNodeCanvas.OnNodeChange(this);
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