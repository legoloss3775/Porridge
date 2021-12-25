using NodeEditorFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameCore;
using FrameCore.ScriptableObjects;
using UnityEditor;
using System.Linq;

[Node(false, "Frame/Flag/Check"), System.Serializable]
public class FlagCheckNode : FrameKeyNode {

    public const string ID = "flagCheck";
    public override string GetID { get { return ID; } }

    public override string Title { get { return "FlagCheck" + " " + frameKey?.id.ToString(); } }
    public override Vector2 DefaultSize { get { return new Vector2(300, 150); } }


#if UNITY_EDITOR
    private void Awake() {
        frameEditorSO = FrameManager.assetDatabase;
    }
    public override void NodeGUI() {

        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;
        if (!NodeEditor.curEditorState.drawing) return;

        UpdateFrameKey();

        InsureFrameKeySelection();

        input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;
        input1Knob.SetPosition(75);

        this.backgroundColor = new Color32(135, 135, 135, 125);

        foreach (var dialogueOutputKnob in frameKey.frameKeyGlobalFlagKnobs) {
            if (dialogueOutputKnob.Key.Contains("true")) {
                GUILayout.Label(dialogueOutputKnob.Key, FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 25));
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                valueKnob.SetPosition();
                GUILayout.Space(25);
                FrameEditor.FrameGUIUtility.GuiLine(2);
                GUILayout.Space(25);
            }
            else {
                GUILayout.Label(dialogueOutputKnob.Key, FrameEditor.FrameGUIUtility.GetLabelStyle(Color.red, 25));
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                valueKnob.SetPosition();
            }
        }

        foreach (var dialogueOutputKnob in frameKey.frameKeyGlobalFlagKnobs) {
            if (outputKnobs.Count > dialogueOutputKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[dialogueOutputKnob.Value];
                valueKnob.SetValue(frameKey);
                valueKnob.maxConnectionCount = ConnectionCount.Single;
            }
        }


        //Задает nextKeyID, если outpuKnob присоеденен к другому KeyNode
        //Работает кривовато, нужно каждый раз указывать тип значений в ключе
        foreach (var FrameKeyTransitionKnob in frameKey.frameKeyGlobalFlagKnobs)
            if (outputKnobs.Count > FrameKeyTransitionKnob.Value) {
                ValueConnectionKnob valueKnob = (ValueConnectionKnob)outputKnobs[FrameKeyTransitionKnob.Value];
                if (valueKnob.connected()) {
                    var elementValues = frameKey.GetFrameKeyValuesOfElement(FrameKeyTransitionKnob.Key);
                    var body = (FrameKeyNode)valueKnob.connection(0).body;
                    if(outputKnobs.IndexOf(valueKnob) == FrameKeyTransitionKnob.Value && FrameKeyTransitionKnob.Key.Contains("true")) {
                        frameKey.flagNextKeyID[0] = body.frameKey.id;
                    }
                    if (outputKnobs.IndexOf(valueKnob) == FrameKeyTransitionKnob.Value && FrameKeyTransitionKnob.Key.Contains("false")) {
                        frameKey.flagNextKeyID[1] = body.frameKey.id;
                    }
                }
            }

        /**if (frameKey.flagData.keys != null) {
            foreach (var flag in frameKey.flagData.keys) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(flag, FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 25));
                GUILayout.FlexibleSpace();
                GUILayout.Label(frameKey.flagData.GetValue(flag).ToString(), FrameEditor.FrameGUIUtility.GetLabelStyle(ORANGE, 25));
                GUILayout.EndHorizontal();
            }
        }**/

        /**if (output1Knob.connected()) {
            var body = (FrameKeyNode)output1Knob.connection(0).body;
            frameKey.flagSequenceData.nextKeyID = body.frameKey.id;
        }**/

        FrameKeySelection();

        foreach (var knob in outputKnobs) {
            knob.color = Color.cyan;
        }

        if (GUI.changed)
            NodeEditor.curNodeCanvas.OnNodeChange(this);
    }
    protected override void OnCreate() {
        var key = new FrameKey();
        FrameManager.frame.frameKeys.Add(key);
        key = FrameManager.frame.frameKeys.Last();
        key.id = FrameManager.frame.frameKeys.IndexOf(key);

        key.keyType = FrameKey.KeyType.FlagCheck;

        this.frameKey = key;
        this.frameKeyPair.frameID = FrameManager.frame.id;
        this.frameKeyPair.frameKeyID = key.id;
        this.input1Knob.maxConnectionCount = NodeEditorFramework.ConnectionCount.Multi;

        //Debug.Log(key.id);

        FrameManager.frame.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.frame.currentKey = key;
        frameEditorSO.selectedKeyIndex = FrameManager.frame.frameKeys.IndexOf(key);
        FrameManager.ChangeFrameKey();

    }
    protected override void OnDelete() {
        var id = FrameManager.frame.frameKeys.IndexOf(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        FrameManager.frame.frameKeys.Remove(FrameManager.frame.frameKeys[frameKeyPair.frameKeyID]);
        foreach (FrameKeyNode node in NodeEditor.curNodeCanvas.nodes) {
            if (node.frameKeyPair.frameKeyID > id)
                node.frameKeyPair.frameKeyID -= 1;
        }
        foreach (var addkey in FrameManager.frame.frameKeys)
            addkey.id = FrameManager.frame.frameKeys.IndexOf(addkey);

    }
#endif
}
