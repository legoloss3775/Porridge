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
    public override Vector2 DefaultSize { get { return new Vector2(400, 400); } }

    [ValueConnectionKnob("Input 1", Direction.In, "FrameKey")]
    public ValueConnectionKnob input1Knob;

    public float oldPos;
    public float oldPosOffset = 50f;

    Vector2 dialogueScroll;
    Vector2 dialogueAnswerScroll;

#if UNITY_EDITOR
    private void OnDisable() {
    }
    public override void NodeGUI() {
        if (AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys.Count > frameKeyPair.frameKeyID)
            frameKey = AssetManager.GetFrameAssets()[Convert.ToInt32(frameKeyPair.frameID.Split('_')[1])].frameKeys[frameKeyPair.frameKeyID];
        else UpdateFrameKeys();

        //frameKey.node = this;
        //FrameEditor_FrameKeу.ShowFrameKeyData(frameKey);
        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;    

        if (frameKeyPair.frameKeyID == 0) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Начало", FrameGUIUtility.GetTextStyle(Color.green, 25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(frameKey.id.ToString(), FrameGUIUtility.GetTextStyle(Color.white, 25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        /*foreach(var background in FrameManager.frameElements.Where(ch => ch is FrameBackground)) {
            if (background.activeStatus == false) continue;

            var icon = UnityEditor.AssetPreview.GetAssetPreview(background.frameElementObject.prefab);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.Width(600));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }*/
        GUILayout.FlexibleSpace();
        FrameGUIUtility.GuiLine(2);
        GUILayout.Space(5);
        foreach (var dialogueOutputKnob in frameKey.dialogueOutputKnobs) {
            var dialogueValues = frameKey.frameKeyValues.Where(ch => ch.Value is FrameUI_DialogueValues);
            var answerValues = frameKey.frameKeyValues.Where(ch => ch.Value is FrameUI_DialogueAnswerValues);

            //GUILayout.Label(frameKey.keySequence.previousKey?.id.ToString());

            if (dialogueValues != null) {
                foreach (var element in dialogueValues) {
                    var dValues = (FrameUI_DialogueValues)element.Value;
                    if (dValues.activeStatus == false) continue;

                    if (element.Key == dialogueOutputKnob.Key) {
                        FrameUI_DialogueValues dialogue = (FrameUI_DialogueValues)element.Value;
                        dialogueScroll = GUILayout.BeginScrollView(dialogueScroll); 
                        GUILayout.TextArea(dialogue?.text, FrameGUIUtility.GetTextStyle(Color.white, 20), GUILayout.MaxHeight(50));
                        GUILayout.EndScrollView();

                        if (outputKnobs.Count > dialogueOutputKnob.Value)
                            outputKnobs[dialogueOutputKnob.Value].SetPosition();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (dialogue.type == FrameUI_Dialogue.FrameDialogueElementType.Одинᅠперсонаж && dialogue.conversationCharacterID != null && dialogue.conversationCharacterID != "") {
                            try {
                                if (UnityEditor.AssetPreview.GetAssetPreview(FrameManager.frame.usedElementsObjects.Where(ch => ch.ids.Contains(dialogue.conversationCharacterID)).FirstOrDefault().elementObject.prefab) == null) continue;
                            }
                            catch (Exception) {
                                continue;
                            }
                            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(
                                    FrameManager.frame.usedElementsObjects.Where(
                                        ch => ch.ids.Contains(dialogue.conversationCharacterID)
                                        )
                                    .FirstOrDefault()
                                    .elementObject
                                    .prefab
                                );
                            GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.Width(600));
                            //GUILayout.FlexibleSpace();//
                        }
                        else {
                            foreach (var character in dialogue.conversationCharacters) {
                                var icon = UnityEditor.AssetPreview.GetAssetPreview(
                                    FrameManager.frame.usedElementsObjects.Where(
                                        ch => ch.ids.Contains(character.Value)
                                        )
                                    .FirstOrDefault()
                                    .elementObject
                                    .prefab
                                 );
                                if (character.Value == dialogue.conversationCharacterID)
                                    GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.gray), GUILayout.MaxWidth(200));
                                else
                                    GUILayout.Label(icon, FrameGUIUtility.SetLabelIconColor(Color.black), GUILayout.MaxWidth(200));
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                }
            }
            
            if (answerValues != null) {
                foreach (var element in answerValues) {
                    var adValues = (FrameUI_DialogueAnswerValues)element.Value;
                    if (adValues.activeStatus == false) continue;

                    if (element.Key == dialogueOutputKnob.Key) {
                        FrameUI_DialogueAnswerValues dialogue = (FrameUI_DialogueAnswerValues)element.Value;
                        GUILayout.TextArea(dialogue?.text, FrameGUIUtility.GetTextStyle(Color.white, 20));
                        if (outputKnobs.Count > dialogueOutputKnob.Value)
                            outputKnobs[dialogueOutputKnob.Value].SetPosition();
                    }
                }
                if (outputKnobs.Count > dialogueOutputKnob.Value) {
                    ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                    valueKnob.SetValue(frameKey);
                    valueKnob.maxConnectionCount = ConnectionCount.Single;
                }
            }
        }
        GUILayout.Space(5);
        FrameGUIUtility.GuiLine(2);
        GUILayout.FlexibleSpace();

        input1Knob.SetPosition(200);

        if (input1Knob.connected()) {
            //frameKey.keySequence.previousKey = input1Knob.GetValue<FrameKey>();
            //input1Knob.GetValue<FrameKey>().keySequence.nextKey = frameKey;
        }

        UpdateFrameKeys();
    }
    private void UpdateFrameKeys() {
        if (FrameManager.frame == null || FrameManager.frame.frameKeys == null) return;
        foreach (var key in FrameManager.frame.frameKeys.ToList()) {
            bool hasKey = false;
            foreach (KeyNode node in canvas.nodes) {
                if (node.frameKeyPair.frameKeyID == key.id)
                    hasKey = true;
            }
            if (!hasKey) {
                var id = FrameManager.frame.frameKeys.IndexOf(key);
                FrameManager.frame.frameKeys.Remove(key);
                foreach (KeyNode node in canvas.nodes) {
                    if (node.frameKeyPair.frameKeyID > id)
                        node.frameKeyPair.frameKeyID -= 1;
                }
                //if (frameKeyPair.frameKeyID > 0 && frameKeyPair.frameKeyID > id)
                   //frameKeyPair.frameKeyID -= 1;//
            }
        }
    }
#endif
}